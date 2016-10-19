using System;
using ObjCRuntime;

namespace Microsoft.Sonoma.Core.iOS.Bindings
{
	[Native]
	public enum SNMLogLevel : ulong //was given as nuint, but i had to change to compile. not sure what is correct
	{
		None = 0,
		Assert = 1,
		Error = 2,
		Warning = 3,
		Debug = 4,
		Verbose = 5
	}
}
