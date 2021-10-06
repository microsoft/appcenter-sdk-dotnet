// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

		[Action ("HasTrackErrorProperties:")]
		partial void HasTrackErrorProperties (AppKit.NSButton sender);

		[Action ("SendTrackEvent:")]
		partial void SendTrackEvent (AppKit.NSButton sender);
		
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
