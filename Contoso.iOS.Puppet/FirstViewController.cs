using System;
using Microsoft.Sonoma.Analytics;
using Microsoft.Sonoma.Core;
using Microsoft.Sonoma.Crashes;
using UIKit;

namespace Contoso.iOS.Puppet
{
	public partial class FirstViewController : UIViewController
	{
		protected FirstViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//TODO i know this should be in app delegate, here just for some very early debugging
			//Sonoma.LogLevel = LogLevel.Verbose;
			//LogLevel l = Sonoma.LogLevel;
			Sonoma.LogLevel = LogLevel.Verbose;
			Sonoma.SetServerUrl("http://in-integration.dev.avalanch.es:8081");
			Sonoma.Start("e7eb534d-58b7-461b-a888-ec250c983e08", typeof(Crashes));
			if (!Crashes.HasCrashedInLastSession)
			{
				Crashes.GenerateTestCrash();
			}
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
