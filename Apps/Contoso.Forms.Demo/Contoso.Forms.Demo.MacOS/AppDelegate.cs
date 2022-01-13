// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using AppKit;
using Foundation;
using Xamarin.Forms.Platform.MacOS;

namespace Contoso.Forms.Demo.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate, IClearCrashClick, IAppConfiguration
    {
        private const string CrashesUserConfirmationStorageKey = "MSAppCenterCrashesUserConfirmation";
        private const string ApplicationCrashOnExceptionsKey = "NSApplicationCrashOnExceptions";
        private static string AppCenterSecret;

        NSPanel window;
        public AppDelegate()
        {
            AppCenterSecret = Environment.GetEnvironmentVariable("XAMARIN_FORMS_MACOS_PROD");
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

        public override void DidFinishLaunching(NSNotification notification)
        {
            Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public void ClearCrashButton()
        {
            NSUserDefaults.StandardUserDefaults.RemoveObject(CrashesUserConfirmationStorageKey);
        }

        public string GetAppSecret()
        {
            return AppCenterSecret;
        }

        public string GetTargetToken()
        {
            return "";
        }
    }
}
