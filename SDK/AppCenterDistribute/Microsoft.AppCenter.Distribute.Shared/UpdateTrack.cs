// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
namespace Microsoft.AppCenter.Distribute
{
    public enum UpdateTrack
    {
        /// <summary>
        /// Releases from the public group that don't require authentication.
        /// </summary>
        Public = 1,

        /// <summary>
        /// Releases from private groups that require authentication, also contain public releases.
        /// </summary>
        Private = 2
	}
}
