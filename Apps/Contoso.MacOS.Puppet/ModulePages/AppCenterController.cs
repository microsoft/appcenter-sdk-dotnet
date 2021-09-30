using System;
using Foundation;
using AppKit;

namespace Contoso.MacOS.Puppet.ModulePages
{
    public partial class AppCenterController : AppKit.NSViewController
    {
        private const string TAG = "XamarinMacOS";

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

        partial void isAppCenterEnabled(NSSwitch sender)
        {
            var isAppCenterEnabled = sender.AccessibilityValue.ToLower().Equals("on");
            Microsoft.AppCenter.AppCenter.SetEnabledAsync(isAppCenterEnabled).Wait();
        }

        partial void isNetworkRequestsAllowed(NSSwitch sender)
        {
            var isNetworkAllowed = sender.AccessibilityValue.ToLower().Equals("on");
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
                System.Diagnostics.Debug.WriteLine(TAG, "Wrong storage size value.");
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
