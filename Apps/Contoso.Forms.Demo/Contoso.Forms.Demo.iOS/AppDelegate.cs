// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Contoso.Forms.Demo.iOS;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics.iOS.Bindings;
using Microsoft.AppCenter.Distribute;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppDelegate))]
namespace Contoso.Forms.Demo.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IClearCrashClick, IAppConfiguration
    {
        private const string CrashesUserConfirmationStorageKey = "MSAppCenterCrashesUserConfirmation";

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();
            Distribute.DontCheckForUpdatesInDebug();
            MSACAnalytics.SetDelegate(new AnalyticsDelegate());
            LoadApplication(new App());
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            base.WillEnterForeground(uiApplication);
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public void ClearCrashButton()
        {
            NSUserDefaults.StandardUserDefaults.RemoveObject(CrashesUserConfirmationStorageKey);
        }

        public string GetAppSecret()
        {
            return Environment.GetEnvironmentVariable("XAMARIN_FORMS_IOS_PROD");
        }

        public string GetTargetToken()
        {
            return Environment.GetEnvironmentVariable("XAMARIN_FORMS_IOS_TARGET_TOKEN_PROD");
        }
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
