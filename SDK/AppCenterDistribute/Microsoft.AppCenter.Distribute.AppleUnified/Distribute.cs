// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Distribute.Apple.Bindings;

namespace Microsoft.AppCenter.Distribute
{
    public static partial class Distribute
    {
        static Type _internalBindingType = typeof(MSACDistribute);

        [Preserve]
        public static Type BindingType
        {
            get
            {
                return _internalBindingType;
            }
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSACDistribute.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSACDistribute.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static void PlatformSetInstallUrl(string installUrl)
        {
            MSACDistribute.SetInstallUrl(installUrl);
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
            MSACDistribute.SetApiUrl(apiUrl);
        }

        /// <summary>
        /// Process URL request for the Distribute service.
        /// Place this method call into app delegate openUrl method.
        /// </summary>
        /// <param name="url">The url with parameters.</param>
        public static void OpenUrl(NSUrl url)
        {
            MSACDistribute.OpenUrl(url);
        }

        /// <summary>
        /// Do not check for updates in case the app is launched with a debug configuration.
        /// In case you want to use in-app updated, place this method call into your
        /// app delegate's FinishedLaunching method BEFORE you call AppCenter.Start(...)
        /// or before you init the forms application object if you use Xamarin Forms.
        /// </summary>
        /// <remarks>
        /// This method is required because the SDK cannot detect an attached debugger, nor can it detect
        /// a release configuration at runtime. If this method is not called, the browser will appear and try to
        /// setup in-app updates.
        /// </remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DontCheckForUpdatesInDebug()
        {
            _internalBindingType = null;
        }

        static Delegate _delegate;

        static ReleaseAvailableCallback _releaseAvailableCallback;

        static WillExitAppCallback _willExitAppCallback;
        
        static NoReleaseAvailableCallback _noReleaseAvailableCallback;

        static void SetReleaseAvailableCallback(ReleaseAvailableCallback releaseAvailableCallback)
        {
            lock (typeof(Distribute))
            {
                _releaseAvailableCallback = releaseAvailableCallback;
                if (_delegate == null && _releaseAvailableCallback != null)
                {
                    _delegate = new Delegate();
                    MSACDistribute.SetDelegate(_delegate);
                }
            }
        }

        static void SetWillExitAppCallback(WillExitAppCallback willExitAppCallback)
        {
            lock (typeof(Distribute))
            {
                _willExitAppCallback = willExitAppCallback;
                if (_delegate == null && _willExitAppCallback != null)
                {
                    _delegate = new Delegate();
                    MSACDistribute.SetDelegate(_delegate);
                }
            }
        }

        static void SetNoReleaseAvailable(NoReleaseAvailableCallback noReleaseAvailable)
        {
            lock (typeof(Distribute))
            {
                _noReleaseAvailableCallback = noReleaseAvailable;
                if (_delegate == null && _noReleaseAvailableCallback != null)
                {
                    _delegate = new Delegate();
                    MSACDistribute.SetDelegate(_delegate);
                }
            }
        }

        static void HandleUpdateAction(UpdateAction updateAction)
        {
            switch (updateAction)
            {
                case UpdateAction.Update:
                    MSACDistribute.NotifyUpdateAction(MSACUpdateAction.Update);
                    break;

                case UpdateAction.Postpone:
                    MSACDistribute.NotifyUpdateAction(MSACUpdateAction.Postpone);
                    break;
            }
        }

        static void SetUpdateTrack(UpdateTrack updateTrack)
        {
            var updateTrackValue = (int)updateTrack;
            MSACDistribute.SetUpdateTrack((MSACUpdateTrack)updateTrackValue);
        }

        static UpdateTrack GetUpdateTrack()
        {
            var updateTrackValue = (int)MSACDistribute.GetUpdateTrack();
            return (UpdateTrack)updateTrackValue;
        }

        static void PlatformCheckForUpdate()
        {
            MSACDistribute.CheckForUpdate();
        }

        static void PlatformDisableAutomaticCheckForUpdate()
        {
            MSACDistribute.DisableAutomaticCheckForUpdate();
        }

        private static void PlatformUnsetInstance()
        {
            MSACDistribute.ResetSharedInstance();
        }

        public class Delegate : MSACDistributeDelegate
        {
            public override bool OnReleaseAvailable(MSACDistribute distribute, MSACReleaseDetails details)
            {
                if (_releaseAvailableCallback != null)
                {
                    Uri releaseNotesUrl = null;
                    if (details.ReleaseNotesUrl != null)
                    {
                        releaseNotesUrl = new Uri(details.ReleaseNotesUrl.ToString());
                    }
                    var releaseDetails = new ReleaseDetails
                    {
                        Id = details.Id,
                        ShortVersion = details.ShortVersion,
                        Version = details.Version,
                        ReleaseNotes = details.ReleaseNotes,
                        ReleaseNotesUrl = releaseNotesUrl,
                        MandatoryUpdate = details.MandatoryUpdate
                    };
                    return _releaseAvailableCallback(releaseDetails);
                }
                return false;
            }

            public override void WillExitApp(MSACDistribute distribute)
            {
                _willExitAppCallback?.Invoke();
            }

            public override void OnNoReleaseAvailable(MSACDistribute distribute)
            {
                _noReleaseAvailableCallback?.Invoke();
            }
        }
    }
}
