using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace Contoso.MacOS.Puppet.ModulePages
{
    public partial class AnalyticsController : AppKit.NSViewController
    {
        private bool hasTrackEventPropery = false;
        private const string On = "1";
        private const string Off = "0";

        #region Constructors

        // Called when created from unmanaged code
        public AnalyticsController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public AnalyticsController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public AnalyticsController() : base("Analytics", NSBundle.MainBundle)
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
            isAnalyticsEnabledSwitch.StringValue = Microsoft.AppCenter.Analytics.Analytics.IsEnabledAsync().Result ? On : Off;
            isAnalyticsEnabledSwitch.Enabled = Microsoft.AppCenter.AppCenter.IsEnabledAsync().Result ? true : false;

        }

        partial void AnalyticsSwitchEnabled(NSSwitch sender)
        {
            var isAnalyticsEnabled = sender.AccessibilityValue.ToLower().Equals("on");
            Microsoft.AppCenter.Analytics.Analytics.SetEnabledAsync(isAnalyticsEnabled).Wait();
            isAnalyticsEnabledSwitch.StringValue = Microsoft.AppCenter.Analytics.Analytics.IsEnabledAsync().Result ? On : Off;
        }

        partial void hasTrackErrorProperties(NSButton sender)
        {
            hasTrackEventPropery = !hasTrackEventPropery;
        }

        partial void sendTrackEvent(NSButton sender)
        {
            var trackEvent = trackEventName.AccessibilityValue;
            if (hasTrackEventPropery)
            {
                var properties = new Dictionary<string, string>();
                properties.Add("properties1", "key1");
                properties.Add("properties2", "key2");
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent(trackEvent, properties);
                return;
            }
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(trackEvent);
        }

        //strongly typed view accessor
        public new Analytics View
        {
            get
            {
                return (Analytics)base.View;
            }
        }
    }
}
