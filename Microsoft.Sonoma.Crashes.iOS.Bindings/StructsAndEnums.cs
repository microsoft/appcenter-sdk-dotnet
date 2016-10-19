using System;
using ObjCRuntime;

[Native]
public enum SNMErrorLogSetting : nuint
{
	Disabled = 0,
	AlwaysAsk = 1,
	AutoSend = 2
}

[Native]
public enum SNMUserConfirmation : nuint
{
	DontSend = 0,
	Send = 1,
	Always = 2
}
