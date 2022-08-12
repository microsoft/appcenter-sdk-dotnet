// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Runtime;

namespace Microsoft.AppCenter.Analytics
{
    /// <summary>
    /// Analytics service.
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
        /// The Android SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(Android.Analytics);

        /// <summary>
        /// Check whether the Analytics service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            var future = Android.Analytics.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        /// <summary>
        /// Enable or disable the Analytics service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            var future = Android.Analytics.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        /// <summary>
        /// Pause the Analytics service.
        /// </summary>
        internal static void Pause()
        {
            Android.Analytics.Pause();
        }

        /// <summary>
        /// Resume the Analytics service.
        /// </summary>
        internal static void Resume()
        {
            Android.Analytics.Resume();
        }

        ///// <summary>
        ///// Enable or disable automatic page tracking.
        ///// Set this to false to if you plan to call <see cref="TrackPage"/> manually.
        ///// </summary>
        //public static bool AutoPageTrackingEnabled
        //{
        //    get { return Android.Analytics.AutoPageTrackingEnabled; }
        //    set { Android.Analytics.AutoPageTrackingEnabled = value; }
        //}

        /// <summary>
        /// Track a custom event.
        /// </summary>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, IDictionary<string, string> properties = null)
        {
            Android.Analytics.TrackEvent(name, properties);
        }

        /// <summary>
        ///  Enable manual session tracker.
        /// </summary>
        public static void EnableManualSessionTracker()
        {
            Android.Analytics.EnableManualSessionTracker();
        }

        /// <summary>
        /// Start a new session if manual session tracker is enabled, otherwise do nothing.
        /// </summary>
        public static void StartSession()
        {
            Android.Analytics.StartSession();
        }

        ///// <summary>
        ///// Track a custom page.
        ///// </summary>
        ///// <param name="name">A page name.</param>
        ///// <param name="properties">Optional properties.</param>
        //public static void TrackPage(string name, [Optional] IDictionary<string, string> properties)
        //{
        //    Android.Analytics.TrackPage(name, properties);
        //}

        internal static void UnsetInstance()
        {
            Android.Analytics.UnsetInstance();
        }
    }
}
