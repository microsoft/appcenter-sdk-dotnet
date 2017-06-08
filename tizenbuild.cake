#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers
#addin "Cake.AzureStorage"
#addin nuget:?package=Cake.Git

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

// MobileCenter module class definition.
class MobileCenterModule {
	public string DotNetModule { get; set; }
	public string NuGetVersion { get; set; }
	public string PackageId { get; set; }
	public string MainNuspecFilename { get; set; }
	public string NuGetPackageName
	{
		get
		{
			return PackageId + "." + NuGetVersion + ".nupkg";
		}
	}
	public string TizenNuspecFilename
	{
		get { return  "Tizen" + MainNuspecFilename; }
	}
	public MobileCenterModule(string dotnet, string mainNuspecFilename) {
		DotNetModule = dotnet;
		MainNuspecFilename = mainNuspecFilename;
	}
}

// Prefix for temporary intermediates that are created by this script
var TEMPORARY_PREFIX = "CAKE_SCRIPT_TEMP";

var DOWNLOADED_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "DownloadedAssemblies";
var WINDOWS_ASSEMBLIES_ZIP = TEMPORARY_PREFIX + "WindowsAssemblies.zip";

// Assembly folders
var PCL_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "PCLAssemblies";

// Native SDK versions

var PLATFORM_PATHS = new PlatformPaths();

// URLs for downloading binaries.
/*
 * Read this: http://www.mono-project.com/docs/faq/security/.
 * On Windows,
 *     you have to do additional steps for SSL connection to download files.
 *     http://stackoverflow.com/questions/4926676/mono-webrequest-fails-with-https
 *     By running mozroots and install part of Mozilla's root certificates can make it work. 
 */

var SDK_STORAGE_URL = "https://mobilecentersdkdev.blob.core.windows.net/sdk/";
var WINDOWS_ASSEMBLIES_URL = SDK_STORAGE_URL + WINDOWS_ASSEMBLIES_ZIP;

// Available MobileCenter modules.
var MOBILECENTER_MODULES = new [] {
	new MobileCenterModule("SDK/MobileCenter/Microsoft.Azure.Mobile", "MobileCenter.nuspec"),
	new MobileCenterModule("SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics", "MobileCenterAnalytics.nuspec"),
	new MobileCenterModule("SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes", "MobileCenterCrashes.nuspec"),
};

// Task TARGET for build
var TARGET = Argument("target", Argument("t", "Default"));

// Storage id to append to upload and download file names in storage
var STORAGE_ID = Argument("StorageId", Argument("storage-id", ""));

class AssemblyGroup
{
	public string[] AssemblyPaths {get; set;}
	public string AssemblyFolder {get; set;}
}

// This class contains the assembly folder paths and other platform dependent paths involved in preparing assemblies for VSTS and Azure storage.
// When a new platform is supported, an AssemblyGroup must be created and added to the proper {OS}UploadAssemblyGroups array. Also, its 
// AssemblyFolder must be added to the correct platform's "DownloadAssemblyFolders" array.
class PlatformPaths
{
	public PlatformPaths()
	{
		UploadAssemblyGroups = new List<AssemblyGroup>();
		DownloadAssemblyFolders = new List<string>();
	}

	// Folders for the assemblies that the current platform must create and upload
	public List<AssemblyGroup> UploadAssemblyGroups {get; set;}

	// The name of the zip file to upload
	public string UploadAssembliesZip {get; set;}

	// The name of the zip file to download
	public string DownloadAssembliesZip {get; set;}
	// The paths of downloaded assembly folders
	public List<string> DownloadAssemblyFolders {get; set;}

	// The URL to download files from
	public string DownloadUrl {get; set;}
}

// Prepare the platform paths for downloading, uploading, and preparing assemblies
Setup(context =>
{
	
		var uwpAnyCpuAssemblyGroup = new AssemblyGroup {
			AssemblyFolder = UWP_ASSEMBLIES_FOLDER,
			AssemblyPaths = new string[] { "nuget/Microsoft.Azure.Mobile.Crashes.targets",
								"SDK/MobileCenter/Microsoft.Azure.Mobile.UWP/bin/Release/Microsoft.Azure.Mobile.dll",
								"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.UWP/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
								"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/Reference/Microsoft.Azure.Mobile.Crashes.dll",
								"SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push.UWP/bin/Release/Microsoft.Azure.Mobile.Push.dll" }
		};
		var uwpX86AssemblyGroup = new AssemblyGroup {
			AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/x86",
			AssemblyPaths = new string[] { 	"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/x86/Release/Microsoft.Azure.Mobile.Crashes.dll",
    							"Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
   								"Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
		};
		var uwpX64AssemblyGroup = new AssemblyGroup {
			AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/x64",
			AssemblyPaths =  new string[] {	"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/x64/Release/Microsoft.Azure.Mobile.Crashes.dll",
   									"x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
   									"x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
		};
		var uwpArmAssemblyGroup = new AssemblyGroup {
			AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/ARM",
			AssemblyPaths =  new string[] {  "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/ARM/Release/Microsoft.Azure.Mobile.Crashes.dll",
									"ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
									"ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
		};
		PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpAnyCpuAssemblyGroup);
		PLATFORM_PATHS.DownloadAssemblyFolders.Add(PCL_ASSEMBLIES_FOLDER);
		PLATFORM_PATHS.UploadAssembliesZip = WINDOWS_ASSEMBLIES_ZIP + STORAGE_ID;

});

// Versioning task.
Task("Version")
	.Does(() =>
{
	var assemblyInfo = ParseAssemblyInfo("./" + "SDK/MobileCenter/Microsoft.Azure.Mobile/Properties/AssemblyInfo.cs");
	var version = assemblyInfo.AssemblyInformationalVersion;
	// Read AssemblyInfo.cs and extract versions for modules.
	foreach (var module in MOBILECENTER_MODULES)
	{
		module.NuGetVersion = version;
	}
});

// Package id task
Task("PackageId")
	.Does(() =>
{
	// Read AssemblyInfo.cs and extract package ids for modules.
	foreach (var module in MOBILECENTER_MODULES)
	{
		var nuspecText = FileReadText("./nuget/" + module.MainNuspecFilename);
		var startTag = "<id>";
		var endTag = "</id>";
		int startIndex = nuspecText.IndexOf(startTag) + startTag.Length;
		int length = nuspecText.IndexOf(endTag) - startIndex;
		var id = nuspecText.Substring(startIndex, length);
		Information("id = " + id);
		module.PackageId = id;
	}
});

//Task("Build").IsDependentOn("MacBuild").IsDependentOn("WindowsBuild");

/*Task("MacBuild")
	.WithCriteria(() => IsRunningOnUnix())
	.Does(() => 
{
	// Run externals here instead of using dependency so that this doesn't get called on windows
	RunTarget("Externals");
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Mac.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Mac.sln", c => c.Configuration = "Release");
}).OnError(HandleError);*/

// Building Windows code task
Task("WindowsBuild")
	.WithCriteria(() => !IsRunningOnUnix())
	.Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Tizen.sln");
	//DotNetBuild("./MobileCenter-SDK-Build-Tizen.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x86"));
	//DotNetBuild("./MobileCenter-SDK-Build-Tizen.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x64"));
	//DotNetBuild("./MobileCenter-SDK-Build-Tizen.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "ARM"));
	DotNetBuild("./MobileCenter-SDK-Build-Tizen.sln", settings => settings.SetConfiguration("Release")); // any cpu
	//DotNetBuild("./MobileCenter-SDK-Build-Tizen.sln", settings => settings.SetConfiguration("Reference")); // any cpu
}).OnError(HandleError);

Task("PrepareAssemblies").IsDependentOn("Build").Does(()=>
{
	foreach (var assemblyGroup in PLATFORM_PATHS.UploadAssemblyGroups)
	{
		CopyFiles(assemblyGroup.AssemblyPaths, assemblyGroup.AssemblyFolder);
	}
}).OnError(HandleError);


// Create a common externals task depending on platform specific ones
Task("Externals").IsDependentOn("Externals-Ios").IsDependentOn("Externals-Android");

// Main Task.
Task("Default").IsDependentOn("NuGet").IsDependentOn("RemoveTemporaries");

// Build tests
Task("UITest").IsDependentOn("RestoreTestPackages").Does(() =>
{
	DotNetBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
});

// Pack NuGets for appropriate platform
Task("NuGet")
	.IsDependentOn("Build")
	.IsDependentOn("Version")
	.Does(()=>
{
	// NuGet on mac trims out the first ./ so adding it twice works around
	//var basePath = IsRunningOnUnix() ? (System.IO.Directory.GetCurrentDirectory().ToString() + @"/.") : "./";
	var basePath = "./";
	CleanDirectory("output");

	// Packaging NuGets.
	foreach (var module in MOBILECENTER_MODULES)
	{
		//var nuspecFilename = IsRunningOnUnix() ? module.MacNuspecFilename : module.TizenNuspecFilename;
		var nuspecFilename = module.TizenNuspecFilename;
		var spec = GetFiles("./nuget/" + nuspecFilename);
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			BasePath = basePath,
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});
	}
	MoveFiles("Microsoft.Azure.Mobile*.nupkg", "output");
}).OnError(HandleError);

// Add version to nuspecs for vsts (the release definition does not have the solutions and thus cannot extract a version from them)
Task("PrepareNuspecsForVSTS").IsDependentOn("Version").Does(()=>
{
	foreach (var module in MOBILECENTER_MODULES)
	{
		ReplaceTextInFiles("./nuget/" + module.MainNuspecFilename, "$version$", module.NuGetVersion);
	}
});


// Upload assemblies to Azure storage
Task("UploadAssemblies")
	.IsDependentOn("PrepareAssemblies")
	.Does(()=>
{
	// The environment variables below must be set for this task to succeed
	var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
	var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");

	foreach (var assemblyGroup in PLATFORM_PATHS.UploadAssemblyGroups)
	{
		var destinationFolder =  DOWNLOADED_ASSEMBLIES_FOLDER + "/" + assemblyGroup.AssemblyFolder;
		CleanDirectory(destinationFolder);
		CopyFiles(assemblyGroup.AssemblyPaths, destinationFolder);
	}

	Information("Uploading to blob " + PLATFORM_PATHS.UploadAssembliesZip);
	Zip(DOWNLOADED_ASSEMBLIES_FOLDER, PLATFORM_PATHS.UploadAssembliesZip);
	AzureStorage.UploadFileToBlob(new AzureStorageSettings
	{
		AccountName = accountName,
		ContainerName = "sdk",
		BlobName = PLATFORM_PATHS.UploadAssembliesZip,
		Key = apiKey,
		UseHttps = true
	}, PLATFORM_PATHS.UploadAssembliesZip);

}).OnError(HandleError).Finally(()=>RunTarget("RemoveTemporaries"));

// Download assemblies from azure storage
Task("DownloadAssemblies").Does(()=>
{
	Information("Fetching assemblies from url: " + PLATFORM_PATHS.DownloadUrl);
	CleanDirectory(DOWNLOADED_ASSEMBLIES_FOLDER);
	DownloadFile(PLATFORM_PATHS.DownloadUrl, PLATFORM_PATHS.DownloadAssembliesZip);
	Unzip(PLATFORM_PATHS.DownloadAssembliesZip, DOWNLOADED_ASSEMBLIES_FOLDER);
	DeleteFiles(PLATFORM_PATHS.DownloadAssembliesZip);
	Information("Successfully downloaded assemblies.");
}).OnError(HandleError);

Task("MergeAssemblies")
	.IsDependentOn("PrepareAssemblies")
	.IsDependentOn("DownloadAssemblies")
	.IsDependentOn("Version")
	.Does(()=>
{
		CleanDirectory("output");

	Information("Beginning complete package creation...");
	var specCopyName = TEMPORARY_PREFIX + "spec_copy.nuspec";
	
	foreach (var assemblyFolder in PLATFORM_PATHS.DownloadAssemblyFolders)
	{
		CleanDirectory(assemblyFolder);
		var files = GetFiles(DOWNLOADED_ASSEMBLIES_FOLDER + "/" + assemblyFolder + "/*");
		CopyFiles(files, assemblyFolder);
	}
	foreach (var module in MOBILECENTER_MODULES)
	{
		// Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
		CopyFile("nuget/" + module.MainNuspecFilename, specCopyName);
		ReplaceTextInFiles(specCopyName, "$pcl_dir$", PCL_ASSEMBLIES_FOLDER);

		var spec = GetFiles(specCopyName);

		// Create the NuGet package
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});

		// Clean up
		DeleteFiles(specCopyName);
	}

	DeleteDirectory(PCL_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(DOWNLOADED_ASSEMBLIES_FOLDER, true);
	CleanDirectory("output");
	MoveFiles("*.nupkg", "output");
}).OnError(HandleError);

Task("TestApps").IsDependentOn("UITest").Does(() =>
{
	// Build tests and package the applications
	// It is important that the entire solution is built before rebuilding the iOS and Android versions
	// due to an apparent bug that causes improper linking of the forms application to iOS
	DotNetBuild("./MobileCenter-SDK-Test.sln", c => c.Configuration = "Release");
	DotNetBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
}).OnError(HandleError);

Task("RestoreTestPackages").Does(() =>
{
	NuGetRestore("./MobileCenter-SDK-Test.sln");
	NuGetUpdate("./Tests/Contoso.Forms.Test/packages.config");
}).OnError(HandleError);

// Remove any uploaded nugets from azure storage
Task("CleanAzureStorage").Does(()=>
{
	var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
	var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");

	try
	{
		AzureStorage.DeleteBlob(new AzureStorageSettings
		{
			AccountName = accountName,
			ContainerName = "sdk",
			BlobName = MAC_ASSEMBLIES_ZIP + STORAGE_ID,
			Key = apiKey,
			UseHttps = true
		});
	
		AzureStorage.DeleteBlob(new AzureStorageSettings
		{
			AccountName = accountName,
			ContainerName = "sdk",
			BlobName = WINDOWS_ASSEMBLIES_ZIP + STORAGE_ID,
			Key = apiKey,
			UseHttps = true
		});
	}
	catch
	{
		// not an error if the blob is not found
	}

}).OnError(HandleError);

// Remove all temporary files and folders
Task("RemoveTemporaries").Does(()=>
{
	DeleteFiles(TEMPORARY_PREFIX + "*");
	var dirs = GetDirectories(TEMPORARY_PREFIX + "*");
	foreach (var directory in dirs)
	{
		DeleteDirectory(directory, true);
	}
	DeleteFiles("./nuget/*.temp.nuspec");
});

// Clean up files/directories.
Task("clean")
	.IsDependentOn("RemoveTemporaries")
	.Does(() =>
{
	DeleteDirectoryIfExists("externals");
	DeleteDirectoryIfExists("output");
	DeleteFiles("./*.nupkg");
	CleanDirectories("./**/bin");
	CleanDirectories("./**/obj");
});

Task("PrepareAssemblyPathsVSTS").Does(()=>
{
		var pclAssemblies = EnvironmentVariable("PCL_ASSEMBLY_PATH_NUSPEC");
		var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
		
		foreach (var module in MOBILECENTER_MODULES)
		{
			ReplaceTextInFiles(nuspecPathPrefix + module.MainNuspecFilename, "$pcl_dir$", pclAssemblies);
		}
}).OnError(HandleError);

Task("NugetPackVSTS").Does(()=>
{
	var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
	foreach (var module in MOBILECENTER_MODULES)
	{
		var spec = GetFiles(nuspecPathPrefix + module.MainNuspecFilename);
		// Create the NuGet packages
		Information("Building a NuGet package for " + module.MainNuspecFilename);
		NuGetPack(spec, new NuGetPackSettings {
			Verbosity = NuGetVerbosity.Detailed,
		});
	}
}).OnError(HandleError);

// Copy files to a clean directory using string names instead of FilePath[] and DirectoryPath
void CopyFiles(IEnumerable<string> files, string targetDirectory, bool clean = true)
{
	if (clean)
	{
		CleanDirectory(targetDirectory);
	}
	foreach (var file in files)
	{
		CopyFile(file, targetDirectory + "/" + System.IO.Path.GetFileName(file));
	}
}

void DeleteDirectoryIfExists(string directoryName)
{
	if (DirectoryExists(directoryName))
	{
		DeleteDirectory(directoryName, true);	
	}
}

void CleanDirectory(string directoryName)
{
	DeleteDirectoryIfExists(directoryName);
	CreateDirectory(directoryName);
}

void HandleError(Exception exception)
{
	RunTarget("clean");
	throw exception;
}

RunTarget(TARGET);
