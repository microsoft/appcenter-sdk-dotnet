// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using AppKit;
using Contoso.Forms.Puppet.MacOS;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics.MacOS.Bindings;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppDelegate))]
namespace Contoso.Forms.Puppet.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.MacOS.FormsApplicationDelegate, IClearCrashClick
    {
        private const string CrashesUserConfirmationStorageKey = "MSAppCenterCrashesUserConfirmation";
        private const string ApplicationCrashOnExceptionsKey = "NSApplicationCrashOnExceptions";

        NSPanel window;
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            window = new NSPanel(rect, style, NSBackingStore.Buffered, false);
            window.Title = "App Center Xamarin.Forms on Mac!";
            window.TitleVisibility = NSWindowTitleVisibility.Hidden;
            var plist = NSUserDefaults.StandardUserDefaults;
            var dictionary = new NSDictionary<NSObject, NSObject>(NSObject.FromObject(true), new NSString(ApplicationCrashOnExceptionsKey));
            plist.RegisterDefaults(dictionary);
        }

        public override NSWindow MainWindow
        {
            get { return window; }
        }

        public void ClearCrashButton()
        {
            NSUserDefaults.StandardUserDefaults.RemoveObject(CrashesUserConfirmationStorageKey);
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            Xamarin.Forms.Forms.Init();
            MSACAnalytics.SetDelegate(new AnalyticsDelegate());
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public class AnalyticsDelegate : MSACAnalyticsDelegate
        {
            public override void WillSendEventLog(MSACAnalytics analytics, MSACEventLog eventLog)
            {
                AppCenterLog.Debug(App.LogTag, "Will send event");
            }

            public override void DidSucceedSendingEventLog(MSACAnalytics analytics, MSACEventLog eventLog)
            {
                AppCenterLog.Debug(App.LogTag, "Did send event");
            }

            public override void DidFailSendingEventLog(MSACAnalytics analytics, MSACEventLog eventLog, NSError error)
            {
                AppCenterLog.Debug(App.LogTag, "Failed to send event with error: " + error);
            }
        }
    }
}
