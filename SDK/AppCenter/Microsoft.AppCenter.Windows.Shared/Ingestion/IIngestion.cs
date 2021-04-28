// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion
{
    /// <summary>
    /// Interface to send logs to the Ingestion service.
    /// </summary>
    public interface IIngestion : IDisposable
    {
        /// <summary>
        /// Update log URL.
        /// </summary>
        /// <param name="logUrl"></param>
        void SetLogUrl(string logUrl);

        /// <summary>
        /// Enable or disable channel with deleting logs.
        /// </summary>
        /// <param name="enabled">Value indicating whether channel should be enabled or disabled</param>
        /// <param name="deleteLogs">True if logs should be deleted, false otherwise.</param>
        void SetEnabled(bool enabled, bool deleteLogs);

        /// <summary>
        /// Gets value indicating whether the ingestion is enabled.
        /// </summary>
        /// <returns>True if enabled, false otherwise.</returns>
        bool IsEnabled();

        /// <summary>
        /// Send logs to the Ingestion service.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application</param>
        /// <param name="installId">Install identifier</param>
        /// <param name="logs">Payload</param>
        IServiceCall Call(string appSecret, Guid installId, IList<Log> logs);

        /// <summary>
        /// Close all current calls.
        /// </summary>
        void Close();
    }
}
