using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Sonoma.Crashes.Shared;

namespace Microsoft.Sonoma.Crashes
{
	using iOSCrashes = iOS.Bindings.SNMCrashes;

	class PlatformCrashes : PlatformCrashesBase
	{
		public override Type BindingType => typeof(iOSCrashes);

		public override bool Enabled
		{
			get { return iOSCrashes.Enabled; }
			set { iOSCrashes.Enabled = value; }
		}

		public override bool HasCrashedInLastSession => iOSCrashes.HasCrashedInLastSession;

		public override void TrackException(Exception exception)
		{
			throw new NotImplementedException();
		}

		static PlatformCrashes()
		{
			//TODO implement me


		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}