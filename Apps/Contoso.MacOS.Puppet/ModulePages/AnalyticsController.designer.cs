// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Contoso.MacOS.Puppet.ModulePages
{
	[Register ("AnalyticsController")]
	partial class AnalyticsController
	{
		[Outlet]
		AppKit.NSSwitch isAnalyticsEnabledSwitch { get; set; }

		[Outlet]
		AppKit.NSTextField trackEventName { get; set; }

		[Action ("AnalyticsSwitchEnabled:")]
		partial void AnalyticsSwitchEnabled (AppKit.NSSwitch sender);

		[Action ("hasTrackErrorProperties:")]
		partial void hasTrackErrorProperties (AppKit.NSButton sender);

		[Action ("sendTrackEvent:")]
		partial void sendTrackEvent (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (isAnalyticsEnabledSwitch != null) {
				isAnalyticsEnabledSwitch.Dispose ();
				isAnalyticsEnabledSwitch = null;
			}

			if (trackEventName != null) {
				trackEventName.Dispose ();
				trackEventName = null;
			}
		}
	}
}
