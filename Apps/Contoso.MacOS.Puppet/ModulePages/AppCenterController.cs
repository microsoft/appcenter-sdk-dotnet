using System;
using Foundation;
using AppKit;

namespace Contoso.MacOS.Puppet.ModulePages
{
    public partial class AppCenterController : AppKit.NSViewController
    {
        private const string LogTag = "XamarinMacOS";
        private const string On = "1";
        private const string Off = "0";

        #region Constructors

        // Called when created from unmanaged code
        public AppCenterController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public AppCenterController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public AppCenterController() : base("AppCenter", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            isAppCenterEnabledSwitch.StringValue = Microsoft.AppCenter.AppCenter.IsEnabledAsync().Result ? On : Off;
            isNetworkRequestAllowedSwitch.StringValue = Microsoft.AppCenter.AppCenter.IsNetworkRequestsAllowed ? On : Off;

            // Set max storage size value.
            var plist = NSUserDefaults.StandardUserDefaults;
            var storageSizeValue = plist.IntForKey(Constants.StorageSizeKey);
            if (storageSizeValue > 0)
            {
                MaxStorageSizeText.StringValue = storageSizeValue.ToString();
            }
        }

        partial void isAppCenterEnabled(NSSwitch sender)
        {
            var isAppCenterEnabled = sender.StringValue.ToLower().Equals(On);
            Microsoft.AppCenter.AppCenter.SetEnabledAsync(isAppCenterEnabled).Wait();
            isAppCenterEnabledSwitch.StringValue = Microsoft.AppCenter.AppCenter.IsEnabledAsync().Result ? On : Off;
        }

        partial void isNetworkRequestsAllowed(NSSwitch sender)
        {
            var isNetworkAllowed = sender.StringValue.ToLower().Equals(On);
            Microsoft.AppCenter.AppCenter.IsNetworkRequestsAllowed = isNetworkAllowed;
        }

        partial void saveMaxStorageSizeText(NSButton sender)
        {
            var size = MaxStorageSizeText.AccessibilityValue;
            long.TryParse(size, out var result);
            if (result != 0)
            {
                Microsoft.AppCenter.AppCenter.SetMaxStorageSizeAsync(result).Wait();
            }
            else
            {
                Microsoft.AppCenter.AppCenterLog.Error(LogTag, "Wrong number value for the max storage size.");
            }
        }

        partial void userIdTextChanged(NSTextField sender)
        {
            var userId = string.IsNullOrEmpty(sender.AccessibilityValue) ? null : sender.AccessibilityValue;
            Microsoft.AppCenter.AppCenter.SetUserId(userId);
        }

        //strongly typed view accessor
        public new AppCenter View
        {
            get
            {
                return (AppCenter)base.View;
            }
        }
    }
}
