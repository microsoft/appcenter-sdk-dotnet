using ObjCRuntime;

[assembly: LinkWith("AppCenter.framework", ForceLoad = true, LinkerFlags = "-lsqlite3")]