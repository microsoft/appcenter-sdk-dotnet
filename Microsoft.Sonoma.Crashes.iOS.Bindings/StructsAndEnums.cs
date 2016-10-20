using System;
using ObjCRuntime;

namespace Microsoft.Sonoma.Crashes.iOS.Bindings {
	//TODO the ulong was actually given as a nuint
	[Native]
	public enum SNMErrorLogSetting : ulong
	{
		Disabled = 0,
		AlwaysAsk = 1,
		AutoSend = 2
	}

	//TODO the ulong was actually given as a nuint
	[Native]
	public enum SNMUserConfirmation : ulong
	{
		DontSend = 0,
		Send = 1,
		Always = 2
	}
}