// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;

namespace Microsoft.AppCenter.Distribute
{
    public static partial class Distribute
    {
        [Preserve]
        public static Type BindingType => typeof(Android.Distribute);

        static Task<bool> PlatformIsEnabledAsync()
        {
            var future = Android.Distribute.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = Android.Distribute.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        static void PlatformSetInstallUrl(string installUrl)
        {
            Android.Distribute.SetInstallUrl(installUrl);
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
            Android.Distribute.SetApiUrl(apiUrl);
        }

        private static void PlatformUnsetInstance()
        {
            Android.Distribute.UnsetInstance();
        }

        static void HandleUpdateAction(UpdateAction updateAction)
        {
            /* Xamarin does not bind interface integers, cannot use AndroidUpdateAction */
            switch (updateAction)
            {
                case UpdateAction.Update:
                    Android.Distribute.NotifyUpdateAction(-1);
                    break;

                case UpdateAction.Postpone:
                    Android.Distribute.NotifyUpdateAction(-2);
                    break;
            }
        }

        static void SetUpdateTrack(UpdateTrack updateTrack)
        {
            Android.Distribute.UpdateTrack = (int)updateTrack;
        }

        static UpdateTrack GetUpdateTrack()
        {
            return (UpdateTrack)Android.Distribute.UpdateTrack;
        }

        static void PlatformCheckForUpdate()
        {
            Android.Distribute.CheckForUpdate();
        }

        static void PlatformDisableAutomaticCheckForUpdate()
        {
            Android.Distribute.DisableAutomaticCheckForUpdate();
        }

        /// <summary>
        /// Set whether the distribute service can be used within a debuggable build.
        /// </summary>
        /// <param name="enabled"><c>true</c> to enable, <c>false</c> to disable (the initial default value is <c>false</c>).</param>
        public static void SetEnabledForDebuggableBuild(bool enabled)
        {
            Android.Distribute.SetEnabledForDebuggableBuild(enabled);
        }

        static Listener _listener;

        static ReleaseAvailableCallback _releaseAvailableCallback;

        static NoReleaseAvailableCallback _noReleaseAvailableCallback;

        static void SetReleaseAvailableCallback(ReleaseAvailableCallback releaseAvailableCallback)
        {
            lock (typeof(Distribute))
            {
                _releaseAvailableCallback = releaseAvailableCallback;
                if (_listener == null && _releaseAvailableCallback != null)
                {
                    _listener = new Listener();
                    Android.Distribute.SetListener(_listener);
                }
            }
        }

        static void SetWillExitAppCallback(WillExitAppCallback willExitAppCallback)
        {
        }

        static void SetNoReleaseAvailable(NoReleaseAvailableCallback noReleaseAvailable)
        {
            lock (typeof(Distribute))
            {
                _noReleaseAvailableCallback = noReleaseAvailable;
                if (_listener == null && _noReleaseAvailableCallback != null)
                {
                    _listener = new Listener();
                    Android.Distribute.SetListener(_listener);
                }
            }
        }

        class Listener : Java.Lang.Object, Android.IDistributeListener
        {
            public bool OnReleaseAvailable(Activity activity, Android.ReleaseDetails androidReleaseDetails)
            {
                if (_releaseAvailableCallback != null)
                {
                    Uri releaseNotesUrl = null;
                    if (androidReleaseDetails.ReleaseNotesUrl != null)
                    {
                        releaseNotesUrl = new Uri(androidReleaseDetails.ReleaseNotesUrl.ToString());
                    }
                    var releaseDetails = new ReleaseDetails
                    {
                        Id = androidReleaseDetails.Id,
                        ShortVersion = androidReleaseDetails.ShortVersion,
                        Version = androidReleaseDetails.Version.ToString(),
                        ReleaseNotes = androidReleaseDetails.ReleaseNotes,
                        ReleaseNotesUrl = releaseNotesUrl,
                        MandatoryUpdate = androidReleaseDetails.IsMandatoryUpdate
                    };
                    return _releaseAvailableCallback(releaseDetails);
                }
                return false;
            }

            public void OnNoReleaseAvailable(Activity activity)
            {
                _noReleaseAvailableCallback?.Invoke();
            }
        }
    }
}
