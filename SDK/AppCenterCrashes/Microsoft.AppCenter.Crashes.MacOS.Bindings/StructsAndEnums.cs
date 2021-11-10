using System;
using ObjCRuntime;

namespace Microsoft.AppCenter.Crashes.MacOS.Bindings
{
    [Native]
    public enum MSACErrorLogSetting : ulong
    {
        Disabled = 0,
        AlwaysAsk = 1,
        AutoSend = 2
    }

    [Native]
    public enum MSACUserConfirmation : ulong
    {
        DontSend = 0,
        Send = 1,
        Always = 2
    }
}