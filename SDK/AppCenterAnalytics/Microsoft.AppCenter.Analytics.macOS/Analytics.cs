using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Foundation;

namespace Microsoft.AppCenter.Analytics
{
    using System.Linq;
    using System.Threading.Tasks;
    using macOSAnalytics = macOS.Bindings.MSAnalytics;

    /// <summary>
    /// Analytics feature.
    /// </summary>
    public class Analytics : AppCenterService
    {
        internal Analytics()
        {
        }

        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The macOS SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(macOSAnalytics);

        /// <summary>
        /// Check whether the Analytics service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(macOSAnalytics.IsEnabled());
        }

        /// <summary>
        /// Enable or disable the Analytics service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            macOSAnalytics.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// Enable or disable automatic page tracking.
        /// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        /// </summary>
        //public static bool AutoPageTrackingEnabled
        //{
        //	get { return macOSAnalytics.IsAutoPageTrackingEnabled(); }
        //	set { macOSAnalytics.SetAutoPageTrackingEnabled(value); }
        //}

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, [Optional] IDictionary<string, string> properties)
        {
            if (properties != null)
            {
                macOSAnalytics.TrackEvent(name, StringDictToNSDict(properties));
                return;
            }
            macOSAnalytics.TrackEvent(name);
        }

        ///// <summary>
        ///// Track a custom page.
        ///// </summary>
        ///// <param name="name">A page name.</param>
        ///// <param name="properties">Optional properties.</param>
        //public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        //{
        //	if (properties != null)
        //	{
        //		macOSAnalytics.TrackPage(name, StringDictToNSDict(properties));
        //		return;
        //	}
        //	macOSAnalytics.TrackPage(name);
        //}

        private static NSDictionary StringDictToNSDict(IDictionary<string, string> dict)
        {
            return NSDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
        }
    }
}
