using AppKit;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics.MacOS.Bindings;
using Xamarin.Forms;
using Xamarin.Forms.Platform.MacOS;

namespace Contoso.Forms.Puppet.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate, IClearCrashClick
    {
        private const string CrashesUserConfirmationStorageKey = "MSAppCenterCrashesUserConfirmation";

        NSWindow window;
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            window.Title = "App Center Xamarin.Forms on Mac!"; // choose your own Title here
            window.TitleVisibility = NSWindowTitleVisibility.Hidden;
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
            AppCenter.Start("2b06eb3f-70c9-4b31-b74b-a84fd2d01f51");
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
