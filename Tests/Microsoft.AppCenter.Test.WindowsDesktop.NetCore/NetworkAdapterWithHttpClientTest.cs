using Microsoft.AppCenter.Ingestion.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using Xunit;

namespace Microsoft.AppCenter.Test.WindowsDesktop
{
    public class NetworkAdapterWithHttpClientTest
    {
        [Fact]
        public void EnableTls12WhenServicePointManagerSetToSystemDefault()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            var httpClientHandler = HttpNetworkAdapter.HttpMessageHandlerOverride() as HttpClientHandler;

            // Check HTTP handler protocol.
            Assert.NotNull(httpClientHandler);
            Assert.Equal(SslProtocols.Tls12, httpClientHandler.SslProtocols);

            // Just check no side effect.
            Assert.Equal(SecurityProtocolType.SystemDefault, ServicePointManager.SecurityProtocol);
        }

        [Fact]
        public void EnableTls12WhenDisabled()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
            var httpClientHandler = HttpNetworkAdapter.HttpMessageHandlerOverride() as HttpClientHandler;
            Assert.NotNull(httpClientHandler);

            // Check protocol was added, not the whole value overridden.
            Assert.Equal(SslProtocols.Tls12, httpClientHandler.SslProtocols);
            Assert.Equal(ServicePointManager.SecurityProtocol, SecurityProtocolType.Tls | SecurityProtocolType.Tls11);
        }

        [Fact]
        public void EnableTls12WhenAlreadyEnabled()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var httpClientHandler = HttpNetworkAdapter.HttpMessageHandlerOverride() as HttpClientHandler;

            // Check HTTP handler protocol.
            Assert.NotNull(httpClientHandler);
            Assert.Equal(SslProtocols.Tls12, httpClientHandler.SslProtocols);

            // Just check no side effect.
            Assert.Equal(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12, ServicePointManager.SecurityProtocol);
        }
    }
}
