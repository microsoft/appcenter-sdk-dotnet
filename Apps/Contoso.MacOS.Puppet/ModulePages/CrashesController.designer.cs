// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using System.CodeDom.Compiler;

namespace Contoso.MacOS.Puppet.ModulePages
{
	[Register ("CrashesController")]
	partial class CrashesController
	{
		[Outlet]
		AppKit.NSSwitch IsCrashesEnabledSwitch { get; set; }

		[Action ("CatchNullReferenceException:")]
		partial void CatchNullReferenceException (AppKit.NSButton sender);

		[Action ("CrashAsync:")]
		partial void CrashAsync (AppKit.NSButton sender);

		[Action ("CrashWithAggregateException:")]
		partial void CrashWithAggregateException (AppKit.NSButton sender);

		[Action ("CrashWithNullReferenceException:")]
		partial void CrashWithNullReferenceException (AppKit.NSButton sender);

		[Action ("DivideByZero:")]
		partial void DivideByZero (AppKit.NSButton sender);

		[Action ("IsCrashesEnabled:")]
		partial void IsCrashesEnabled (AppKit.NSSwitch sender);

		[Action ("NativeCrash:")]
		partial void NativeCrash (AppKit.NSButton sender);

		[Action ("TestCrash:")]
		partial void TestCrash (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (IsCrashesEnabledSwitch != null) {
				IsCrashesEnabledSwitch.Dispose ();
				IsCrashesEnabledSwitch = null;
			}
		}
	}
}
