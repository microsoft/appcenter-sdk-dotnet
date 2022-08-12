// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if NET462
using System.Net;
#else
using System.Net.Http;
using System.Security.Authentication;
#endif

namespace Microsoft.AppCenter.Ingestion.Http
{
    /// <inheritdoc />
    public sealed partial class HttpNetworkAdapter
    {
        // Static initializer specific to windows desktop platforms.
        static HttpNetworkAdapter()
        {
#if NET462
            // ReSharper disable once InvertIf
            if ((ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) != SecurityProtocolType.Tls12)
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                AppCenterLog.Debug(AppCenterLog.LogTag, "Enabled TLS 1.2 explicitly as it was disabled.");
            }
#else
            HttpMessageHandlerOverride = () => new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12
            };
#endif
        }
    }
}
