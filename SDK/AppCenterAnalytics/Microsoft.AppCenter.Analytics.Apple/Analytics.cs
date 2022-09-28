// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Analytics.Apple.Bindings;

namespace Microsoft.AppCenter.Analytics
{
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
        /// The Apple SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(MSACAnalytics);

        /// <summary>
        /// Check whether the Analytics service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return Task.FromResult(MSACAnalytics.IsEnabled());
        }

        /// <summary>
        /// Enable or disable the Analytics service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            MSACAnalytics.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// Pause the Analytics service.
        /// </summary>
        internal static void Pause()
        {
            MSACAnalytics.Pause();
        }

        /// <summary>
        /// Resume the Analytics service.
        /// </summary>
        internal static void Resume()
        {
            MSACAnalytics.Resume();
        }

        ///// <summary>
        ///// Enable or disable automatic page tracking.
        ///// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        ///// </summary>
        //public static bool AutoPageTrackingEnabled
        //{
        //	get { return MSACAnalytics.IsAutoPageTrackingEnabled(); }
        //	set { MSACAnalytics.SetAutoPageTrackingEnabled(value); }
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
                MSACAnalytics.TrackEvent(name, StringDictToNSDict(properties));
                return;
            }
            MSACAnalytics.TrackEvent(name);
        }

        /// <summary>
        ///  Enable manual session tracker.
        /// </summary>
        public static void EnableManualSessionTracker()
        {
            MSACAnalytics.EnableManualSessionTracker();
        }

        /// <summary>
        /// Start a new session if manual session tracker is enabled, otherwise do nothing.
        /// </summary>
        public static void StartSession()
        {
            MSACAnalytics.StartSession();
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
        //		MSACAnalytics.TrackPage(name, StringDictToNSDict(properties));
        //		return;
        //	}
        //	MSACAnalytics.TrackPage(name);
        //}

        internal static void UnsetInstance()
        {
            MSACAnalytics.ResetSharedInstance();
        }

        private static NSDictionary StringDictToNSDict(IDictionary<string, string> dict)
        {
            return NSDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
        }
    }
}
