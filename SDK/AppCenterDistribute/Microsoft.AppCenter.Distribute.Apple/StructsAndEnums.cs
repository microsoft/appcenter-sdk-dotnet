// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using ObjCRuntime;

namespace Microsoft.AppCenter.Distribute.Apple.Bindings
{
	[Native]
	public enum MSACUpdateAction : ulong
	{
        Update = 0,
        Postpone = 1
	}

	[Native]
	public enum MSACUpdateTrack : ulong
	{
        Public = 1,
        Private = 2
	}
}
