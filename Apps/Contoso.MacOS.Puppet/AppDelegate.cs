// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AppKit;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Contoso.MacOS.Puppet
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        const string LogTag = "AppCenterXamarinPuppet";

        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Configure App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            var plist = NSUserDefaults.StandardUserDefaults;
            var storageSizeValue = plist.IntForKey(Constants.StorageSizeKey);
            if (storageSizeValue > 0)
            {
                AppCenter.SetMaxStorageSizeAsync(storageSizeValue);
            }
            AppCenter.Start("2b06eb3f-70c9-4b31-b74b-a84fd2d01f51", typeof(Analytics), typeof(Crashes));
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
