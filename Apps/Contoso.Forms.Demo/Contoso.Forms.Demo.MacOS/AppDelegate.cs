using AppKit;
using Foundation;
using Xamarin.Forms.Platform.MacOS;

namespace Contoso.Forms.Demo.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate, IClearCrashClick
    {
        private const string CrashesUserConfirmationStorageKey = "MSAppCenterCrashesUserConfirmation";
        NSPanel window;
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
            var rect = new CoreGraphics.CGRect(200, 1000, 1024, 768);
            window = new NSPanel(rect, style, NSBackingStore.Buffered, false);
            window.Title = "App Center Xamarin.Forms on Mac!"; // choose your own Title here
            window.TitleVisibility = NSWindowTitleVisibility.Hidden;
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
