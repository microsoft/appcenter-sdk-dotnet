﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Distribute
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;

    public class DistributeUpdateTest
    {
        private readonly string _appSecret = "e94aaff4-e80d-4fee-9a5f-a84eb6e688fc";

        private readonly string _installId = Guid.NewGuid().ToString();

        private readonly string _distributionGroupId = Guid.NewGuid().ToString();

        private static string _requestId = "b627efb5-dbf7-4350-92e4-b6ac4dbd09b0";

        private static string _package = "com.contoso.test.functional";

        [Fact]
        public async Task FreshInstallAsync()
        {
            // Prepare data.
            var prefs = Application.Current.Properties["Distribute.request_id"] = _requestId;

            var androidUrl = $"intent://updates/#Intent;scheme=appcenter;package={_package};S.distribution_group_id={_distributionGroupId};S.request_id={_requestId};end";
            var iosUrl = $"appcenter-{_appSecret}://?request_id=${_requestId}&distribution_group_id={Guid.NewGuid().ToString()}";

            // Setup network adapter.
            // distributionStartSession
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "distributionStartSession");
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

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                Xamarin.Forms.Device.OpenUri(new Uri(Xamarin.Forms.Device.RuntimePlatform == "iOS" ? iosUrl : androidUrl));
            });

            // Open deep link uri.
            // Task.Run(() => { Xamarin.Forms.Device.OpenUri(new Uri(url)); }).Wait();
            // Task.Delay(5000).Wait();

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'distributionStartSession')]").ToList();
            Assert.Equal(1, eventLogs.Count());
        }
    }
}
