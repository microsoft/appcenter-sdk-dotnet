using System;
using ObjCRuntime;

namespace Microsoft.AppCenter.MacOS.Bindings
{
    [Native]
    public enum MSACLogLevel : ulong
    {
        Verbose = 2,
        Debug = 3,
        Info = 4,
        Warning = 5,
        Error = 6,
        Assert = 7,
        None = 99
    }
}