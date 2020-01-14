using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Distribute;
using Xamarin.Forms;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Distribute
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;

    public enum DistributeTestType
    {
        FreshInstallAsync,
        CheckUpdateAsync,
        OnResumeActivity,
        Clear
    }

    public class DistributeUpdateTest
    {
        public delegate void DistributeEventHandler(object sender, DistributeTestType e);

        public static event DistributeEventHandler DistributeEvent;

        [Fact]
        public async Task FreshInstallAsync()
        {
            // Save data to preference.
            DistributeEvent?.Invoke(this, DistributeTestType.FreshInstallAsync);

            // Prepare data.
            var prefs = Application.Current.Properties["Distribute.request_id"] = Config._requestId;
            var androidUrl = $"appcenter://updates/?#Intent;scheme=appcenter;package={Config._package};distribution_group_id={Config._distributionGroupId};request_id={Config._requestId};end";
            var iosUrl = $"appcenter-{Config._appSecret}://?request_id=${Config._requestId}&distribution_group_id={Config._distributionGroupId}";

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "distributionStartSession");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start AppCenter.
            StartAppCenter();
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Disable and enable distribute.
            Distribute.SetEnabledAsync(false).Wait();
            Distribute.SetEnabledAsync(true).Wait();

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Xamarin.Essentials.Browser.OpenAsync(new Uri(Xamarin.Forms.Device.RuntimePlatform == "iOS" ? iosUrl : androidUrl));
            });

            // Wait for processing event.
            httpNetworkAdapter.HttpResponseTask.Wait(1000);

            // Verify response.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'distributionStartSession')]").ToList();
            Assert.Equal(1, eventLogs.Count());

            // Clear data.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }

        [Fact]
        public async Task CheckUpdateAsync()
        {
            // Save data to preference.
            DistributeEvent?.Invoke(this, DistributeTestType.CheckUpdateAsync);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start AppCenter.
            StartAppCenter();
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Disable and enable distribute.
            Distribute.SetEnabledAsync(false).Wait();
            Distribute.SetEnabledAsync(true).Wait();

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify response.
            Assert.True(httpNetworkAdapter.Uri.Contains(Config._appSecret));

            // Clear.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }

        private void StartAppCenter()
        {
            AppCenter.UnsetInstance();
            Distribute.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config._appSecret, typeof(Distribute));
        }
    }
}
