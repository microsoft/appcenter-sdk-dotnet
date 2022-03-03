// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.FileHelpers&version=3.0.0
#addin nuget:?package=Cake.Git&version=0.18.0
#addin nuget:?package=Cake.Incubator&version=2.0.2
#addin nuget:?package=Cake.Xamarin
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
var IosExternals = $"{ExternalsDirectory}/ios";
var MacosExternals = $"{ExternalsDirectory}/macos";

var SdkStorageUrl = "https://mobilecentersdkdev.blob.core.windows.net/sdk/";

// Need to read versions before setting url values
VersionReader.ReadVersions();
var AndroidUrl = $"{SdkStorageUrl}AppCenter-SDK-Android-{VersionReader.AndroidVersion}.zip";
var AppleUrl = $"{SdkStorageUrl}AppCenter-SDK-Apple-{VersionReader.AppleVersion}.zip";

// Task Target for build
var Target = Argument("Target", Argument("t", "Default"));

var NuspecFolder = "nuget";

// Path to folders and projects. 
var configuration = "Release";
var solutionDir = ".";
var outputPath = "bin/" + configuration;
var utilClassLibraryProject = "Apps/Contoso.UtilClassLibrary/Contoso.UtilClassLibrary.csproj";
var commonProject = "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo/Contoso.Forms.Demo.csproj";
var androidProject = "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.Droid/Contoso.Forms.Demo.Droid.csproj";
var iosProject = "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.iOS/Contoso.Forms.Demo.iOS.csproj";

// Android apk configuration.
var androidSigningKeyStore = EnvironmentVariable("PATH_TO_ANDROID_KEYSTORE_FILE") ?? "";
var androidSigningKeyAlias = EnvironmentVariable("ANDROID_KEYSTORE_ALIAS") ?? "";
var androidSigningStorePass = EnvironmentVariable("ANDROID_KEYSTORE_PASSWORD") ?? "";
var androidSigningKeyPass = EnvironmentVariable("ANDROID_KEYSTORE_ALIAS_PASSWORD") ?? "";

// iOS ipa configuration.
var serverAddress = EnvironmentVariable("NAME_OF_YOUR_MAC") ?? "";
var serverUser = EnvironmentVariable("NAME_OF_YOUR_MAC_USER") ?? "";
var serverPassword = EnvironmentVariable("PASSWORD_OF_YOUR_MAC_USER") ?? "";

// Application outputs.
var applicationOutput = "applicationOutput/";
var androidApkPath = "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.Droid/bin/Release/*.apk";
var iosIpsPath = "Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.iOS/bin/Release/*.ipa";

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
    var buildGroup = new BuildGroup(platformId);
    buildGroup.ExecuteBuilds();
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
Task("Externals-Ios")
    .Does(() =>
{
    CleanDirectory(IosExternals);
    var zipFile = System.IO.Path.Combine(IosExternals, "ios.zip");

    // Download zip file containing AppCenter frameworks
    DownloadFile(AppleUrl, zipFile);
    Unzip(zipFile, IosExternals);
    var frameworksLocation = System.IO.Path.Combine(IosExternals, "AppCenter-SDK-Apple/iOS");

    // Copy the AppCenter binaries directly from the frameworks and add the ".a" extension
    var files = GetFiles($"{frameworksLocation}/*.framework/AppCenter*");
    foreach (var file in files)
    {
        var filename = file.GetFilename();
        MoveFile(file, $"{IosExternals}/{filename}.a");
    }
    
    // Copy Distribute resource bundle and copy it to the externals directory.
    var distributeBundle = "AppCenterDistributeResources.bundle";
    if(DirectoryExists($"{frameworksLocation}/{distributeBundle}"))
    {
        MoveDirectory($"{frameworksLocation}/{distributeBundle}", $"{IosExternals}/{distributeBundle}");
    }
}).OnError(HandleError);

// Downloading macOS binaries.
Task("Externals-MacOS")
    .Does(() =>
{
    CleanDirectory(MacosExternals);
    CopyDirectory($"{IosExternals}/AppCenter-SDK-Apple", $"{MacosExternals}/AppCenter-SDK-Apple");

    var frameworksLocation = System.IO.Path.Combine(MacosExternals, "AppCenter-SDK-Apple/macOS");

    // Copy the AppCenter binaries directly from the frameworks and add the ".a" extension
    var files = GetFiles($"{frameworksLocation}/*.framework/Versions/A/AppCenter*");
    foreach (var file in files)
    {
        var filename = file.GetFilename();
        CopyFile(file, $"{MacosExternals}/{filename}.a");
    }

    //generate correct .framework directories
    CopyDirectory($"{frameworksLocation}/AppCenter.framework/Versions/A", $"{MacosExternals}/AppCenter.framework");
    CopyDirectory($"{frameworksLocation}/AppCenterAnalytics.framework/Versions/A", $"{MacosExternals}/AppCenterAnalytics.framework");
    CopyDirectory($"{frameworksLocation}/AppCenterCrashes.framework/Versions/A", $"{MacosExternals}/AppCenterCrashes.framework");
}).OnError(HandleError);

// Create a common externals task depending on platform specific ones
Task("Externals").IsDependentOn("Externals-Ios").IsDependentOn("Externals-MacOS").IsDependentOn("Externals-Android");

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

Task("WindowsXamarinFormsBuildApps")
    .Does(()=>
{
    CreateOutputApplication();

    // Build applications.
    BuildUwpApk();
}).OnError(HandleError);

Task("MacXamarinFormsBuildApps")
    .Does(()=>
{
    CreateOutputApplication();

    // Build applications.
    BuildAndroidApk();
    BuildIosIpa();
}).OnError(HandleError);

public MSBuildSettings CreateDefaultBuildSettings(string buildConfig, string outputPath, string solutionDir)
{
    var result = new MSBuildSettings();
    result.Verbosity = Verbosity.Minimal;
    result.Configuration = buildConfig;
    result.WithTarget("Clean");
    result.WithTarget("Build");   
    result.WithProperty("OutputPath", outputPath);
 	result.WithProperty("SolutionDir", solutionDir);
    return result;
}

public void CreateOutputApplication() {

    // Create directory if not exists.
    if (!DirectoryExists(applicationOutput))
    {
        CreateDirectory(applicationOutput);
    }
    CleanDirectory(applicationOutput);
}

public void BuildUwpApk() {

}

public void BuildAndroidApk() {

    // Restore NuGet.
    NuGetRestore(utilClassLibraryProject, new NuGetRestoreSettings { NoCache = true });
    NuGetRestore(commonProject, new NuGetRestoreSettings { NoCache = true });
    NuGetRestore(androidProject, new NuGetRestoreSettings { NoCache = true });

    // Configure build settings.
    var buildSettings = CreateDefaultBuildSettings(configuration, outputPath, solutionDir);
    buildSettings.WithProperty("AndroidUseSharedRuntime",  "false" );
    buildSettings.WithProperty("EmbedAssembliesIntoApk", "true");
    buildSettings.WithProperty("BundleAssemblies", "true");
    buildSettings.WithProperty("AotAssemblies", "true");
    buildSettings.WithProperty("EnableLLVM", "true");
    buildSettings.WithProperty("AndroidLinkMode", "SdkOnly");
    buildSettings.WithProperty("AndroidSigningKeyStore", androidSigningKeyStore);
    buildSettings.WithProperty("AndroidSigningKeyAlias", androidSigningKeyAlias);
    buildSettings.WithProperty("AndroidSigningStorePass", androidSigningStorePass);
    buildSettings.WithProperty("AndroidSigningKeyPass", androidSigningKeyPass);         
    buildSettings.Targets.Clear();
    buildSettings.WithTarget("SignAndroidPackage");
    buildSettings.WithProperty("AndroidSupportedAbis", "armeabi-v7a");
    
    // Build apk.
    MSBuild(androidProject, buildSettings);

    // Move apk to output file.
    var files = GetFiles(androidApkPath);
    CopyFiles(files, applicationOutput);
}

public void BuildIosIpa() {

    // Restore NuGet.
    NuGetRestore(commonProject, new NuGetRestoreSettings { NoCache = true });
    NuGetRestore(iosProject, new NuGetRestoreSettings { NoCache = true });

    // Configure build settings.
    var buildSettings = CreateDefaultBuildSettings(configuration, outputPath, solutionDir);
    buildSettings.WithTarget("Build");
    buildSettings.WithProperty("Platform", "iPhone");
    buildSettings.WithProperty("BuildIpa", "true");
    // buildSettings.WithProperty("ServerAddress", serverAddress)
    // buildSettings.WithProperty("ServerUser", serverUser)
    // buildSettings.WithProperty("ServerPassword", serverPassword)
    buildSettings.WithProperty("TreatWarningsAsErrors", "false");

    // Build ipa.
    MSBuild(iosProject, buildSettings);

    // Move ipa to output file.
    var files = GetFiles(iosIpsPath);
    CopyFiles(files, applicationOutput);
}

RunTarget(Target);
