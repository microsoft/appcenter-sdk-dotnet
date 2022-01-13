// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version

using System;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using UIKit;

namespace Contoso.iOS.Puppet
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window
        {
            get;
            set;
        }

        static bool _didTapNotification;
        const string LogTag = "AppCenterXamarinPuppet";

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {

            // Configure App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            Distribute.SetInstallUrl("https://install.portal-server-core-integration.dev.avalanch.es");
            Distribute.SetApiUrl("https://api-gateway-core-integration.dev.avalanch.es/v0.1");
            Distribute.DontCheckForUpdatesInDebug();
            Distribute.WillExitApp = OnWillExitApp;
            Distribute.NoReleaseAvailable = OnNoReleaseAvailable;
            var plist = NSUserDefaults.StandardUserDefaults;
            var storageSizeValue = plist.IntForKey(Constants.StorageSizeKey);
            if (storageSizeValue > 0)
            {
                AppCenter.SetMaxStorageSizeAsync(storageSizeValue);
            }
            if (plist.BoolForKey(Constants.EnableManualSessionTrackerKey)) {
                Analytics.EnableManualSessionTracker();
            }
            var appCenterSecret = Environment.GetEnvironmentVariable("XAMARIN_IOS_INT");
            AppCenter.Start(appCenterSecret, typeof(Analytics), typeof(Crashes), typeof(Distribute));
            return true;
        }

        void OnWillExitApp()
        {
            AppCenterLog.Info(LogTag, "App will close callback invoked.");
        }

        void OnNoReleaseAvailable()
        {
            AppCenterLog.Info(LogTag, "No release available callback invoked.");
        }
    }
}

