using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Distribute
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;

    public class DistributeUpdateTest
    {
        private readonly string _appSecret = Guid.NewGuid().ToString();

        private static string DistributionGroupId = "test123";

        [Fact]
        public async Task FreshInstallAsync()
        {
            // Prepare data.
            // TODO add url for iOS.
            var url = "";

            // Setup network adapter.
            // distributionStartSession
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "startSession");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Reset instance.
            // TODO add UnsetInstance to Distribute.
            // Distribute.UnsetInstance();
            AppCenter.UnsetInstance();
            
            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Distribute));

            // Wait when Distribute wil be start.
            await Distribute.IsEnabledAsync();

            // Open deep link uri.
            Task.Run(() => { Xamarin.Forms.Device.OpenUri(new Uri(url)); }).Wait();
            Task.Delay(5000).Wait();

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'distributionStartSession')]").ToList();
            Assert.Equal(1, eventLogs.Count());
        }
    }
}
