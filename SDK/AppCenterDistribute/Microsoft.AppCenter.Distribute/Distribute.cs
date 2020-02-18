// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Microsoft.AppCenter.Distribute
{
    public static partial class Distribute
    {
        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }

        static void PlatformSetInstallUrl(string installUrl)
        {
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
        }

        static void SetReleaseAvailableCallback(ReleaseAvailableCallback releaseAvailableCallback)
        {
        }

        static void HandleUpdateAction(UpdateAction updateAction)
        {
        }

        static void SetUpdateTrack(UpdateTrack updateTrack)
        {
        }

        static UpdateTrack GetUpdateTrack()
        {
            return UpdateTrack.Public;
        }

        static void PlatformCheckForUpdate()
        {
        }

        static void PlatformDisableAutomaticCheckForUpdate()
        {
        }

        private static void PlatformUnsetInstance()
        {
        }
    }
}
