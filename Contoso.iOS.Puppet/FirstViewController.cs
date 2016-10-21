using System;
using Microsoft.Sonoma.Analytics;
using Microsoft.Sonoma.Core;

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

			Sonoma.SetServerUrl("http://in-integration.dev.avalanch.es:8081");
			Sonoma.Initialize("e7eb534d-58b7-461b-a888-ec250c983e08");
			Sonoma.LogLevel = LogLevel.Verbose;
			LogLevel l = Sonoma.LogLevel;

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
