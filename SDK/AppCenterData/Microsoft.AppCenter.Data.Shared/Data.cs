using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Data
    {

        /// <summary>
        /// Check whether the RealUserMeasurements service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return PlatformIsEnabledAsync();
        }

        /// <summary>
        /// Enable or disable the RealUserMeasurements service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
        }

        /// <summary>
        /// Configure key.
        /// </summary>
        public static void SetRumKey(string rumKey)
        {
            PlatformSetRumKey(rumKey);
        }
    }
}