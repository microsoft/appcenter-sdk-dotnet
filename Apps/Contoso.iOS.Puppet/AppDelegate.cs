// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#pragma warning disable XI0002 // Notifies you from using newer Apple APIs when targeting an older OS version

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

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            Distribute.SetInstallUrl("https://install.portal-server-core-integration.dev.avalanch.es");
            Distribute.SetApiUrl("https://api-gateway-core-integration.dev.avalanch.es/v0.1");
            Distribute.DontCheckForUpdatesInDebug();
            AppCenter.Start("e94aaff4-e80d-4fee-9a5f-a84eb6e688fc", typeof(Analytics), typeof(Crashes), typeof(Distribute));
            return true;
        }
    }
}

