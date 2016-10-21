using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;

namespace Microsoft.Sonoma.Analytics
{
	using iOSAnalytics = iOS.Bindings.SNMAnalytics;

	/// <summary>
	/// Analytics feature.
	/// </summary>
	public static class Analytics
	{
		/// <summary>
		/// Internal SDK property not intended for public use.
		/// </summary>
		/// <value>
		/// The Android SDK Analytics bindings type.
		/// </value>
		public static Type BindingType => typeof(iOSAnalytics);

		/// <summary>
		/// Enable or disable Analytics module.
		/// </summary>
		public static bool Enabled
		{
			get { return iOSAnalytics.Enabled; }
			set { iOSAnalytics.Enabled = value; }
		}

		/// <summary>
		/// Enable or disable automatic page tracking.
		/// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
		/// </summary>
		public static bool AutoPageTrackingEnabled
		{
			get { return iOSAnalytics.IsAutoPageTrackingEnabled(); }
			set { iOSAnalytics.SetAutoPageTrackingEnabled(value); }
		}

		/// <summary>
		/// Track a custom event.
		/// </summary>
		/// <param name="name">An event name.</param>
		/// <param name="properties">Optional properties.</param>
		public static void TrackEvent(string name, [Optional] IDictionary<string, string> properties)
		{
			if (properties != null)
			{
				iOSAnalytics.TrackEvent(name, StringDictToNSDict(properties));
				return;
			}
			iOSAnalytics.TrackEvent(name);
		}

		/// <summary>
		/// Track a custom page.
		/// </summary>
		/// <param name="name">A page name.</param>
		/// <param name="properties">Optional properties.</param>
		public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
		{
			if (properties != null)
			{
				iOSAnalytics.TrackPage(name, StringDictToNSDict(properties));
				return;
			}
			iOSAnalytics.TrackPage(name);
		}

		//TODO move this to some kind of utility file?
		private static NSDictionary StringDictToNSDict(IDictionary<string, string> dict)
		{
			NSDictionary nsdict = new NSDictionary();
			
			foreach (var pair in dict)
			{
				NSObject NSObjValue = NSObject.FromObject(pair.Value);
				NSString NSStringKey = (NSString)NSObject.FromObject(pair.Key);
				nsdict.SetValueForKey(NSObjValue, NSStringKey);
			}

			return nsdict;
		}
	}
}
