// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AppKit;
using Foundation;
using Xamarin.Forms.Platform.MacOS;
using Xamarin.Forms;
using Contoso.Forms.Demo.MacOS;

[assembly: Dependency(typeof(AppDelegate))]
namespace Contoso.Forms.Demo.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate, IClearCrashClick
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
    }
}
