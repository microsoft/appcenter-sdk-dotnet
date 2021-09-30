using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace Contoso.MacOS.Puppet.ModulePages
{
    public partial class CrashesController : AppKit.NSViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public CrashesController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public CrashesController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public CrashesController() : base("Crashes", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        partial void isCrashesEnabled(NSSwitch sender)
        {
            var isAnalyticsEnabled = sender.AccessibilityValue.ToLower().Equals("on");
            Microsoft.AppCenter.Crashes.Crashes.SetEnabledAsync(isAnalyticsEnabled).Wait();
        }

        //strongly typed view accessor
        public new Crashes View
        {
            get
            {
                return (Crashes)base.View;
            }
        }
    }
}
