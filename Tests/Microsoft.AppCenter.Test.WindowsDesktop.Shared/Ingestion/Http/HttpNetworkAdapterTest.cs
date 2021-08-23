using Microsoft.AppCenter.Ingestion.Http;
using System.Net;
using Xunit;

namespace Microsoft.AppCenter.Test.WindowsDesktop.Ingestion.Http
{
    public class HttpNetworkAdapterTest
    {
        [Fact]
        public void EnableTls12WhenDisabled()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11;

            // Init http adapter.
            new HttpNetworkAdapter();

            // Check protocol was added, not the whole value overridden.
            Assert.Equal(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12, ServicePointManager.SecurityProtocol);
        }

        [Fact]
        public void EnableTls12WhenAlreadyEnabled()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            // Init http adapter.
            new HttpNetworkAdapter();

            // Just check no side effect.
            Assert.Equal(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12, ServicePointManager.SecurityProtocol);
        }
    }
}
