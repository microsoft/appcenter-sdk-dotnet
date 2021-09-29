using ObjCRuntime;

[assembly: LinkWith("AppCenterCrashes.framework", LinkerFlags = "-lc++", ForceLoad = true, SmartLink = false, IsCxx = true)]