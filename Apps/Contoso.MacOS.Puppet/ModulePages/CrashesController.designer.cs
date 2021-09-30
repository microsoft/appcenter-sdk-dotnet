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

		[Action ("isCrashesEnabled:")]
		partial void isCrashesEnabled (AppKit.NSSwitch sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (isCrashesEnabledSwitch != null) {
				isCrashesEnabledSwitch.Dispose ();
				isCrashesEnabledSwitch = null;
			}
		}
	}
}
