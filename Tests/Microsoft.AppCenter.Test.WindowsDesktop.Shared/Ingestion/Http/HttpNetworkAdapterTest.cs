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
#if NET461
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11;

            // Init http adapter.
            var httpAdapter = new HttpNetworkAdapter();

            // Check protocol was added, not the whole value overridden.
            Assert.Equal(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12, ServicePointManager.SecurityProtocol);

            // Dispose http datapter.
            httpAdapter.Dispose();
#endif
        }

        [Fact]
        public void EnableTls12WhenAlreadyEnabled()
        {
#if NET461
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            // Init http adapter.
            var httpAdapter = new HttpNetworkAdapter();

            // Just check no side effect.
            Assert.Equal(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12, ServicePointManager.SecurityProtocol);

            // Dispose http datapter.
            httpAdapter.Dispose();
#endif
        }
    }
}
