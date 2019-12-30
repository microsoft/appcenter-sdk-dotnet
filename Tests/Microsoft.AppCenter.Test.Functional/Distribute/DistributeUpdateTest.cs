using System;
using System.Threading.Tasks;
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
            // Reset instance.
            AppCenter.UnsetInstance();
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            // Distribute.UnsetInstance();

            // Prepare data.
            var url = $"appcenter://updates/?request_id={"test"}?distribution_group_id={DistributionGroupId}?update_token={"test"}?update_setup_failed={"test"}?tester_app_update_setup_failed={"test"}";

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Distribute));

            // Wait when Distribute wil be start.
            await Distribute.IsEnabledAsync();

            // Open deep link uri.
            Task.Run(() => { Xamarin.Forms.Device.OpenUri(new Uri(url)); }).Wait(1000);
            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("GET", httpNetworkAdapter.Method);
            Assert.Contains(DistributionGroupId, httpNetworkAdapter.Uri);
            // Verify distribution group id value.
            //   Assert.True(Xamarin.Forms.Application.Current.Properties.ContainsKey(PrefKeyDistributionGroupId));
            // Assert.Equal(Xamarin.Forms.Application.Current.Properties[PrefKeyDistributionGroupId] as string, "test");
        }
    }
}
