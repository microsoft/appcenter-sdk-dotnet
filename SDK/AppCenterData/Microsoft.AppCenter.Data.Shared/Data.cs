// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Data<T>
    {

        /// <summary>
        /// Check whether the Data service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            throw new Exception();
        }

        /// <summary>
        /// Enable or disable the Data service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            throw new Exception();
        }

        /// <summary>
        /// Configure key.
        /// </summary>
        public static void SetRumKey(string rumKey)
        {
            throw new Exception();
        }
    }
}