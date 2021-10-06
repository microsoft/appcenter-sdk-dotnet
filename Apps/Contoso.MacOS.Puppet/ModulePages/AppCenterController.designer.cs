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
		AppKit.NSSwitch IsAppCenterEnabledSwitch { get; set; }

		[Outlet]
		AppKit.NSSwitch isNetworkRequestAllowedSwitch { get; set; }

		[Outlet]
		AppKit.NSTextField MaxStorageSizeText { get; set; }

		[Outlet]
		AppKit.NSTextField UserIdText { get; set; }

		[Action ("IsAppCenterEnabled:")]
		partial void IsAppCenterEnabled (AppKit.NSSwitch sender);

		[Action ("IsNetworkRequestsAllowed:")]
		partial void IsNetworkRequestsAllowed (AppKit.NSSwitch sender);

		[Action ("SaveMaxStorageSizeText:")]
		partial void SaveMaxStorageSizeText (AppKit.NSButton sender);

		[Action ("UserIdTextChanged:")]
		partial void UserIdTextChanged (AppKit.NSTextField sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (IsAppCenterEnabledSwitch != null) {
				IsAppCenterEnabledSwitch.Dispose ();
				IsAppCenterEnabledSwitch = null;
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
