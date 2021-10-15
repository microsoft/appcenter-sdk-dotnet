// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        private NSUserDefaults plist = NSUserDefaults.StandardUserDefaults;

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
            IsAppCenterEnabledSwitch.StringValue = Microsoft.AppCenter.AppCenter.IsEnabledAsync().Result ? On : Off;
            isNetworkRequestAllowedSwitch.StringValue = Microsoft.AppCenter.AppCenter.IsNetworkRequestsAllowed ? On : Off;

            // Set max storage size value.
            plist = NSUserDefaults.StandardUserDefaults;
            var storageSizeValue = plist.IntForKey(Constants.StorageSizeKey);
            if (storageSizeValue > 0)
            {
                MaxStorageSizeText.StringValue = storageSizeValue.ToString();
            }
        }

        partial void IsAppCenterEnabled(NSSwitch sender)
        {
            var IsAppCenterEnabled = sender.StringValue.ToLower().Equals(On);
            Microsoft.AppCenter.AppCenter.SetEnabledAsync(IsAppCenterEnabled).Wait();
            IsAppCenterEnabledSwitch.StringValue = Microsoft.AppCenter.AppCenter.IsEnabledAsync().Result ? On : Off;
        }

        partial void IsNetworkRequestsAllowed(NSSwitch sender)
        {
            var isNetworkAllowed = sender.StringValue.ToLower().Equals(On);
            Microsoft.AppCenter.AppCenter.IsNetworkRequestsAllowed = isNetworkAllowed;
        }

        partial void SaveMaxStorageSizeText(NSButton sender)
        {
            var size = MaxStorageSizeText.AccessibilityValue;
            int.TryParse(size, out var result);
            if (result != 0)
            {
                Microsoft.AppCenter.AppCenter.SetMaxStorageSizeAsync(result).Wait();
                plist.SetInt(result, Constants.StorageSizeKey);
            }
            else
            {
                Microsoft.AppCenter.AppCenterLog.Error(LogTag, "Wrong number value for the max storage size.");
            }
        }

        partial void UserIdTextChanged(NSTextField sender)
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
