// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#addin nuget:?package=Cake.FileHelpers&version=5.0.0
#addin nuget:?package=Cake.Incubator&version=7.0.0
#load "scripts/utility.cake"
#load "scripts/configuration/config-parser.cake"

using System.Net;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

// Contains all assembly paths and how to use them
IList<AssemblyGroup> AssemblyGroups = null;

// Available AppCenter modules.
IList<AppCenterModule> AppCenterModules = null;

// URLs for downloading binaries.
/*
 * Read this: http://www.mono-project.com/docs/faq/security/.
 * On Windows,
 *     you have to do additional steps for SSL connection to download files.
 *     http://stackoverflow.com/questions/4926676/mono-webrequest-fails-with-https
 *     By running mozroots and install part of Mozilla's root certificates can make it work.
 */

var ExternalsDirectory = "externals";
var AndroidExternals = $"{ExternalsDirectory}/android";
var AppleExternals = $"{ExternalsDirectory}/apple";

var SdkStorageUrl = "https://mobilecentersdkdev.blob.core.windows.net/sdk/";

// Need to read versions before setting url values
VersionReader.ReadVersions();
var AndroidUrl = $"{SdkStorageUrl}AppCenter-SDK-Android-{VersionReader.AndroidVersion}.zip";
var AppleUrl = $"{SdkStorageUrl}AppCenter-SDK-Apple-{VersionReader.AppleVersion}.zip";

// Task Target for build
var Target = Argument("Target", Argument("t", "Default"));

var NuspecFolder = "nuget";

// Prepare the platform paths for downloading, uploading, and preparing assemblies
Setup(context =>
{
    AssemblyGroups = AssemblyGroup.ReadAssemblyGroups();
    AppCenterModules = AppCenterModule.ReadAppCenterModules(NuspecFolder, VersionReader.SdkVersion);
});

Task("Build")
    .IsDependentOn("Externals")
    .Does(() =>
{
    var platformId = IsRunningOnUnix() ? "mac" : "windows";
    var buildGroups = BuildGroup.ReadBuildGroups();
    foreach (var buildGroup in buildGroups)
    {
        buildGroup.ExecuteBuilds();
    }
}).OnError(HandleError);

Task("PrepareAssemblies")
    .IsDependentOn("Build")
    .Does(() =>
{
    foreach (var assemblyGroup in AssemblyGroups)
    {
        if (assemblyGroup.Download)
        {
            continue;
        }
        // Clean all directories before copying. Doing so before each operation
        // could cause subdirectories that are created first to be deleted.
        CleanDirectory(assemblyGroup.Folder);
        CopyFiles(assemblyGroup.AssemblyPaths.Where(i => FileExists(i)), assemblyGroup.Folder, false);
    }
}).OnError(HandleError);

// Downloading Android binaries.
Task("Externals-Android")
    .Does(() =>
{
    var zipFile = System.IO.Path.Combine(AndroidExternals, "android.zip");
    if (FileExists(zipFile))
    {
        return;
    }
    CleanDirectory(AndroidExternals);

    // Download zip file.
    DownloadFile(AndroidUrl, zipFile);
    Unzip(zipFile, AndroidExternals);

    // Move binaries to externals/android so that linked files don't have versions
    // in their paths
    var files = GetFiles($"{AndroidExternals}/*/*");
    CopyFiles(files, AndroidExternals);
}).OnError(HandleError);

// Downloading iOS binaries.
Task("Externals-Apple")
    .WithCriteria(() => IsRunningOnUnix())
    .Does(() =>
{
    var zipFile = System.IO.Path.Combine(AppleExternals, "apple.zip");
    if (FileExists(zipFile))
    {
        return;
    }
    CleanDirectory(AppleExternals);

    // Download zip file.
    DownloadFile(AppleUrl, zipFile);
    using(var process = StartAndReturnProcess("unzip",
        new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append(zipFile)
                .Append("-d")
                .Append(AppleExternals),
            RedirectStandardOutput = true,
        }))
    {
        process.WaitForExit(10000);
        if (process.GetExitCode() != 0)
        {
            throw new Exception($"Failed to unzip {zipFile}");
        }
    }

    var iosFrameworksLocation = System.IO.Path.Combine(AppleExternals, "AppCenter-SDK-Apple/iOS");
    var macosFrameworksLocation = System.IO.Path.Combine(AppleExternals, "AppCenter-SDK-Apple/macOS");

    // Move iOS frameworks.
    var iosExternals = System.IO.Path.Combine(AppleExternals, "ios");
    CleanDirectory(iosExternals);

    // Copy the AppCenter binaries directly from the frameworks and add the ".a" extension.s
    var files = GetFiles($"{iosFrameworksLocation}/*.framework/AppCenter*");
    foreach (var file in files)
    {
        var filename = file.GetFilename();
        MoveFile(file, $"{iosExternals}/{filename}.a");
    }
    
    // Copy Distribute resource bundle and copy it to the externals directory.
    var distributeBundle = "AppCenterDistributeResources.bundle";
    if(DirectoryExists($"{iosFrameworksLocation}/{distributeBundle}"))
    {
        MoveDirectory($"{iosFrameworksLocation}/{distributeBundle}", $"{iosExternals}/{distributeBundle}");
    }

    // Move macOS frameworks.
    var macosExternals = System.IO.Path.Combine(AppleExternals, "macos");
    CleanDirectory(macosExternals);
    var frameworks = GetDirectories($"{macosFrameworksLocation}/*.framework");
    foreach (var frameworkDir in frameworks)
    {
        var dirName = frameworkDir.GetDirectoryName();
        MoveDirectory(frameworkDir, $"{macosExternals}/{dirName}");
    }
}).OnError(HandleError);

// Create a common externals task depending on platform specific ones
Task("Externals").IsDependentOn("Externals-Apple").IsDependentOn("Externals-Android");

// Main Task.
Task("Default").IsDependentOn("NuGet").IsDependentOn("RemoveTemporaries");

// Pack NuGets for appropriate platform
Task("NuGet")
    .IsDependentOn("PrepareAssemblies")
    .Does(()=>
{
    CleanDirectory("output");
    var specCopyName = Statics.TemporaryPrefix + "spec_copy.nuspec";

    // Package NuGets.
    foreach (var module in AppCenterModules)
    {
        var nuspecFilename = (IsRunningOnUnix() ? module.MacNuspecFilename : module.WindowsNuspecFilename);
        var nuspecPath = System.IO.Path.Combine(NuspecFolder, nuspecFilename);

        // Skip modules that don't have nuspecs.
        if (!FileExists(nuspecPath))
        {
            continue;
        }

        // Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
        CopyFile(nuspecPath, specCopyName);
        ReplaceAssemblyPathsInNuspecs(specCopyName);
        Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
        NuGetPack(File(specCopyName), new NuGetPackSettings {
            Verbosity = NuGetVerbosity.Detailed,
            Version = module.NuGetVersion,
            RequireLicenseAcceptance = true
        });

        // Clean up
        DeleteFiles(specCopyName);
    }
    MoveFiles("Microsoft.AppCenter*.nupkg", "output");
}).OnError(HandleError);

Task("NuGetPackAzDO").Does(()=>
{
    var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
    foreach (var module in AppCenterModules)
    {
        var nuspecPath = System.IO.Path.Combine(nuspecPathPrefix, module.MainNuspecFilename);
        ReplaceTextInFiles(nuspecPath, "$version$", module.NuGetVersion);
        ReplaceAssemblyPathsInNuspecs(nuspecPath);

        var spec = GetFiles(nuspecPath);

        // Create the NuGet packages.
        Information("Building a NuGet package for " + module.MainNuspecFilename);
        NuGetPack(spec, new NuGetPackSettings {
            Verbosity = NuGetVerbosity.Detailed,
            RequireLicenseAcceptance = true
        });
    }
}).OnError(HandleError);

// In AzDO, the assembly path environment variable names should be in the format
// "{uppercase group id}_ASSEMBLY_PATH_NUSPEC"
void ReplaceAssemblyPathsInNuspecs(string nuspecPath)
{
    // For the Tuples, Item1 is variable name, Item2 is variable value.
    var assemblyPathVars = new List<Tuple<string, string>>();
    foreach (var group in AssemblyGroups)
    {
        if (group.NuspecKey == null)
        {
            continue;
        }
        var environmentVariableName = group.Id.ToUpper() + "_ASSEMBLY_PATH_NUSPEC";
        var assemblyPath = EnvironmentVariable(environmentVariableName, group.Folder);
        var tuple = Tuple.Create(group.NuspecKey, assemblyPath);
        assemblyPathVars.Add(tuple);
    }
    foreach (var assemblyPathVar in assemblyPathVars)
    {
        ReplaceTextInFiles(nuspecPath, assemblyPathVar.Item1, assemblyPathVar.Item2);
    }
}

RunTarget(Target);
