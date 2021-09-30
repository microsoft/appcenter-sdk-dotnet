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
	[Register ("CrashesController")]
	partial class CrashesController
	{
		[Outlet]
		AppKit.NSSwitch isCrashesEnabledSwitch { get; set; }

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

		[Action ("isCrashesEnabled:")]
		partial void isCrashesEnabled (AppKit.NSSwitch sender);

		[Action ("NativeCrash:")]
		partial void NativeCrash (AppKit.NSButton sender);

		[Action ("TestCrash:")]
		partial void TestCrash (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (isCrashesEnabledSwitch != null) {
				isCrashesEnabledSwitch.Dispose ();
				isCrashesEnabledSwitch = null;
			}
		}
	}
}
