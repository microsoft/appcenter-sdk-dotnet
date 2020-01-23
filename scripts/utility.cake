// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// This file contains various utilities that are used or can be used by multiple cake scripts.

// Static variables defined outside of a class can cause issues.

using Newtonsoft.Json.Linq;

public class Statics
{
    // Cake context.
    public static ICakeContext Context { get; set; }

    // Prefix for temporary intermediates that are created by this script.
    public const string TemporaryPrefix = "CAKE_SCRIPT_TEMP";
}

// Can't reference Context within the class, so set value outside
Statics.Context = Context;

// Copy files to a clean directory using string names instead of FilePath[] and DirectoryPath
void CopyFiles(IEnumerable<string> files, string targetDirectory, bool clean = true)
{
    if (clean)
    {
        CleanDirectory(targetDirectory);
    }
    else if (!DirectoryExists(targetDirectory))
    {
        CreateDirectory(targetDirectory);
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
        DeleteDirectory(directoryName, new DeleteDirectorySettings { Force = true, Recursive = true });
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

// Remove all temporary files and folders
Task("RemoveTemporaries").Does(()=>
{
    DeleteFiles(Statics.TemporaryPrefix + "*");
    var dirs = GetDirectories(Statics.TemporaryPrefix + "*");
    foreach (var directory in dirs)
    {
        DeleteDirectoryIfExists(directory.ToString());
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

JObject GetResponseJson(HttpWebRequest request)
{
    using (var response = request.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream()))
    {
        return JObject.Parse(reader.ReadToEnd());
    }
}

// Gets the latest repository release version. 
// repoName is a repository name, including owner: <owner>/<name>.
string GetLatestGitHubReleaseVersion(string repoName) 
{
    var request = CreateGitHubRequest($"repos/{repoName}/releases/latest");
    var release = GetResponseJson(request);
    return release["tag_name"].ToString();
}

// Creates a valid GitHub request and fills the headers needed to make a request to GH.
// path is a part of the url without the base part, doesn't need to start with "/".
HttpWebRequest CreateGitHubRequest(string path) 
{
    var url = $"https://api.github.com/{path}";
    var request = (HttpWebRequest)WebRequest.Create (url);
    request.Accept = "application/vnd.github.v3+json";
    request.UserAgent = "Microsoft";
    return request;
}