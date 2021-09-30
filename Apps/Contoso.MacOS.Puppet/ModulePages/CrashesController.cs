using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using System.IO;

namespace Contoso.MacOS.Puppet.ModulePages
{
    public partial class CrashesController : AppKit.NSViewController
    {
        private const string On = "1";
        private const string Off = "0";

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

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            isCrashesEnabledSwitch.StringValue = Microsoft.AppCenter.Crashes.Crashes.IsEnabledAsync().Result ? On : Off;
            isCrashesEnabledSwitch.Enabled = Microsoft.AppCenter.AppCenter.IsEnabledAsync().Result ? true : false;
        }

        partial void isCrashesEnabled(NSSwitch sender)
        {
            var isAnalyticsEnabled = sender.AccessibilityValue.ToLower().Equals("on");
            Microsoft.AppCenter.Crashes.Crashes.SetEnabledAsync(isAnalyticsEnabled).Wait();
            isCrashesEnabledSwitch.StringValue = Microsoft.AppCenter.Crashes.Crashes.IsEnabledAsync().Result ? On : Off;
        }

        partial void TestCrash(NSButton sender)
        {
            Microsoft.AppCenter.Crashes.Crashes.GenerateTestCrash();
        }

        partial void DivideByZero(NSButton sender)
        {
            /* This is supposed to cause a crash, so we don't care that the variable 'x' is never used */
#pragma warning disable CS0219
            int x = (42 / int.Parse("0"));
#pragma warning restore CS0219
        }

        partial void CatchNullReferenceException(NSButton sender)
        {
            try
            {
                TriggerNullReferenceException();
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("null reference exception");
            }
        }

        partial void CrashWithNullReferenceException(NSButton sender)
        {
            TriggerNullReferenceException();
        }

        void TriggerNullReferenceException()
        {
            string[] values = { "one", null, "two" };
            for (int ctr = 0; ctr <= values.GetUpperBound(0); ctr++)
            {
                var val = values[ctr].Trim();
                var separator = ctr == values.GetUpperBound(0) ? "" : ", ";
                System.Diagnostics.Debug.WriteLine("{0}{1}", val, separator);
            }
            System.Diagnostics.Debug.WriteLine("");
        }

        partial void CrashWithAggregateException(NSButton sender)
        {
            throw PrepareException();
        }

        static Exception PrepareException()
        {
            try
            {
                throw new AggregateException(SendHttp(), new ArgumentException("Invalid parameter", ValidateLength()));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        static Exception SendHttp()
        {
            try
            {
                throw new IOException("Network down");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        static Exception ValidateLength()
        {
            try
            {
                throw new ArgumentOutOfRangeException(null, "It's over 9000!");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        async partial void CrashAsync(NSButton sender)
        {
            await FakeService.DoStuffInBackground();
        }

        partial void NativeCrash(NSButton sender)
        {
            try
            {
                NSNull.Null.PerformSelector(new ObjCRuntime.Selector("isEqualToString:"));
            }
            catch (Exception ex)
            {
                Microsoft.AppCenter.Crashes.Crashes.TrackError(ex);
            }
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
