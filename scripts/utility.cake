// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// This file contains various utilities that are used or can be used by multiple cake scripts.

// Static variables defined outside of a class can cause issues.

#addin nuget:?package=Newtonsoft.Json&version=13.0.2

using Newtonsoft.Json.Linq;

public class Statics
{
    // Cake context.
    public static ICakeContext Context { get; set; }
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

// Clean up files/directories.
Task("clean")
    .Does(() =>
{
    DeleteDirectoryIfExists("externals");
    DeleteDirectoryIfExists("output");
    DeleteFiles("./nuget/*.temp.nuspec");
    DeleteFiles("./*.nupkg");
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
});

JObject GetResponseJson(HttpWebRequest request)
{
    return GetHandledResponseJson(JObject.Parse, request);
}

JArray GetResponseJsonArray(HttpWebRequest request)
{
    return GetHandledResponseJson(JArray.Parse, request);
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

private T GetHandledResponseJson<T>(Func<string, T> responseHandler, HttpWebRequest request) where T : JContainer
{
    using (var response = request.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream()))
    {
        return responseHandler(reader.ReadToEnd());
    }
}