using System;
using ObjCRuntime;

[Native]
public enum SNMLogLevel : nuint
{
	None = 0,
	Assert = 1,
	Error = 2,
	Warning = 3,
	Debug = 4,
	Verbose = 5
}
