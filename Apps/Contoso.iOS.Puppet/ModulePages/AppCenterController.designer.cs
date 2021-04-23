// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Contoso.iOS.Puppet
{
	[Register ("AppCenterController")]
	partial class AppCenterController
	{
		[Outlet]
		UIKit.UISwitch AppCenterEnabledSwitch { get; set; }

		[Outlet]
		UIKit.UISwitch AppCenterNetworkRequestAllowedSwitch { get; set; }

		[Outlet]
		UIKit.UILabel LogLevelLabel { get; set; }

		[Outlet]
		UIKit.UILabel LogWriteLevelLabel { get; set; }

		[Outlet]
		UIKit.UITextField LogWriteMessage { get; set; }

		[Outlet]
		UIKit.UITextField LogWriteTag { get; set; }

		[Outlet]
		UIKit.UITextField StorageSizeText { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField UserIdTextField { get; set; }

		[Action ("NetworkRequestAllowedSwitch")]
		partial void NetworkRequestAllowedSwitch ();

		[Action ("SaveStorageSize:")]
		partial void SaveStorageSize (Foundation.NSObject sender);

		[Action ("UpdateEnabled")]
		partial void UpdateEnabled ();

		[Action ("UpdateUserId:")]
		partial void UpdateUserId (UIKit.UITextField sender);

		[Action ("WriteLog")]
		partial void WriteLog ();
		
		void ReleaseDesignerOutlets ()
		{
			if (AppCenterEnabledSwitch != null) {
				AppCenterEnabledSwitch.Dispose ();
				AppCenterEnabledSwitch = null;
			}

			if (AppCenterNetworkRequestAllowedSwitch != null) {
				AppCenterNetworkRequestAllowedSwitch.Dispose ();
				AppCenterNetworkRequestAllowedSwitch = null;
			}

			if (LogLevelLabel != null) {
				LogLevelLabel.Dispose ();
				LogLevelLabel = null;
			}

			if (LogWriteLevelLabel != null) {
				LogWriteLevelLabel.Dispose ();
				LogWriteLevelLabel = null;
			}

			if (LogWriteMessage != null) {
				LogWriteMessage.Dispose ();
				LogWriteMessage = null;
			}

			if (StorageSizeText != null) {
				StorageSizeText.Dispose ();
				StorageSizeText = null;
			}

			if (LogWriteTag != null) {
				LogWriteTag.Dispose ();
				LogWriteTag = null;
			}

			if (UserIdTextField != null) {
				UserIdTextField.Dispose ();
				UserIdTextField = null;
			}
		}
	}
}
