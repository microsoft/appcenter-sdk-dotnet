using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Distribute;
using Xamarin.Forms;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Distribute
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;

    public class DistributeUpdateTest
    {
        private readonly string _appSecret = "e94aaff4-e80d-4fee-9a5f-a84eb6e688fc";

        private readonly string _distributionGroupId = Guid.NewGuid().ToString();

        private static string _requestId = "b627efb5-dbf7-4350-92e4-b6ac4dbd09b0";

        private static string _package = "com.contoso.test.functional";

        [Fact]
        public async Task FreshInstallAsync()
        {
            // Prepare data.
            var prefs = Application.Current.Properties["Distribute.request_id"] = _requestId;
            var androidUrl = $"appcenter://updates/?#Intent;scheme=appcenter;package={_package};distribution_group_id={_distributionGroupId};request_id={_requestId};end";
            //var androidUrl = $"intent://updates/#Intent;scheme=appcenter;package={_package};S.distribution_group_id={_distributionGroupId};S.request_id={_requestId};end";
            //var androidUrl = $"appcenter://updates?request_id={_requestId}&distribution_group_id={_distributionGroupId}";
            var iosUrl = $"appcenter-{_appSecret}://?request_id=${_requestId}&distribution_group_id={_distributionGroupId}";

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "distributionStartSession");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Reset instance.
            // TODO add UnsetInstance to Distribute.
            // Distribute.UnsetInstance();
            AppCenter.UnsetInstance();

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;

            // Wait when Distribute wil be start.
            await Distribute.IsEnabledAsync();

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Xamarin.Essentials.Browser.OpenAsync(new Uri(Xamarin.Forms.Device.RuntimePlatform == "iOS" ? iosUrl : androidUrl));
            });

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'distributionStartSession')]").ToList();
            Assert.Equal(1, eventLogs.Count());
        }
    }
}
