// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Contoso.iOS.Puppet
{
	[Register ("AnalyticsController")]
	partial class AnalyticsController
	{
		[Outlet]
		UIKit.UISwitch AnalyticsEnabledSwitch { get; set; }

		[Outlet]
		UIKit.UISwitch EnableManualSessionTrackerSwitch { get; set; }

		[Outlet]
		UIKit.UITextField EventName { get; set; }

		[Outlet]
		UIKit.UILabel NumPropertiesLabel { get; set; }

		[Action ("AddProperty")]
		partial void AddProperty ();

		[Action ("SessionGenerationUpdate:")]
		partial void SessionGenerationUpdate (Foundation.NSObject sender);

		[Action ("StartSession:")]
		partial void StartSession (Foundation.NSObject sender);

		[Action ("TrackEvent")]
		partial void TrackEvent ();

		[Action ("UpdateEnabled")]
		partial void UpdateEnabled ();
		
		void ReleaseDesignerOutlets ()
		{
			if (EnableManualSessionTrackerSwitch != null) {
				EnableManualSessionTrackerSwitch.Dispose ();
				EnableManualSessionTrackerSwitch = null;
			}

			if (AnalyticsEnabledSwitch != null) {
				AnalyticsEnabledSwitch.Dispose ();
				AnalyticsEnabledSwitch = null;
			}

			if (EventName != null) {
				EventName.Dispose ();
				EventName = null;
			}

			if (NumPropertiesLabel != null) {
				NumPropertiesLabel.Dispose ();
				NumPropertiesLabel = null;
			}
		}
	}
}
