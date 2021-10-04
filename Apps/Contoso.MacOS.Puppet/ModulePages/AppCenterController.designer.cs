// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using System.CodeDom.Compiler;

namespace Contoso.MacOS.Puppet.ModulePages
{
	[Register ("AppCenterController")]
	partial class AppCenterController
	{
		[Outlet]
		AppKit.NSSwitch isAppCenterEnabledSwitch { get; set; }

		[Outlet]
		AppKit.NSSwitch isNetworkRequestAllowedSwitch { get; set; }

		[Outlet]
		AppKit.NSTextField MaxStorageSizeText { get; set; }

		[Outlet]
		AppKit.NSTextField UserIdText { get; set; }

		[Action ("isAppCenterEnabled:")]
		partial void isAppCenterEnabled (AppKit.NSSwitch sender);

		[Action ("isNetworkRequestsAllowed:")]
		partial void isNetworkRequestsAllowed (AppKit.NSSwitch sender);

		[Action ("saveMaxStorageSizeText:")]
		partial void saveMaxStorageSizeText (AppKit.NSButton sender);

		[Action ("userIdTextChanged:")]
		partial void userIdTextChanged (AppKit.NSTextField sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (isAppCenterEnabledSwitch != null) {
				isAppCenterEnabledSwitch.Dispose ();
				isAppCenterEnabledSwitch = null;
			}

			if (isNetworkRequestAllowedSwitch != null) {
				isNetworkRequestAllowedSwitch.Dispose ();
				isNetworkRequestAllowedSwitch = null;
			}

			if (MaxStorageSizeText != null) {
				MaxStorageSizeText.Dispose ();
				MaxStorageSizeText = null;
			}

			if (UserIdText != null) {
				UserIdText.Dispose ();
				UserIdText = null;
			}
		}
	}
}
