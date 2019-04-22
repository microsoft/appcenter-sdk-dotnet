using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Data.
    /// </summary>
    public partial class Data
    {
        /// <summary>
        /// Platforms the is enabled async.
        /// </summary>
        /// <returns>The is enabled async.</returns>
        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Platforms the set enabled async.
        /// </summary>
        /// <returns>The set enabled async.</returns>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// Platforms the set rum key.
        /// </summary>
        /// <param name="rumKey">Rum key.</param>
        static void PlatformSetRumKey(string rumKey)
        {
        }
    }
}