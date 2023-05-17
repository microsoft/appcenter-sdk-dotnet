// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    /// <summary>
    /// Contains information about App Center .NET SDK.
    /// </summary>
    public partial class WrapperSdk
    {
        /// <summary>
        /// Name of SDK reported in logs.
        /// </summary>
        public const string Name = "appcenter.xamarin";

        /* We can't use reflection for assemblyInformationalVersion on iOS with "Link All" optimization. */
        internal const string Version = "5.0.2-SNAPSHOT";
    }
}
