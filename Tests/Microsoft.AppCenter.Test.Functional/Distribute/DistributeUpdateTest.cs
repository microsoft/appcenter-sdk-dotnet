// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Distribute
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;
    using AppCenter = Microsoft.AppCenter.AppCenter;
    using UpdateTrack = Microsoft.AppCenter.Distribute.UpdateTrack;

    public enum DistributeTestType
    {
        SaveMockUpdateToken,
        EnableDebuggableBuilds,
        FreshInstallAsync,
        CheckUpdateAsync,
        OnResumeActivity,
        Clear
    }

    public class DistributeUpdateTest
    {
        public delegate void DistributeEventHandler(object sender, DistributeTestType e);

        public static event DistributeEventHandler DistributeEvent;

        // Before
        public DistributeUpdateTest()
        {
            Utils.DeleteDatabase();
            Distribute.UnsetInstance();
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }

        [Fact]
        public async Task GetLastReleaseDetailsAsync()
        {
            // Enable Distribute for debuggable builds.
            DistributeEvent?.Invoke(this, DistributeTestType.EnableDebuggableBuilds);

            // Save data to preference.
            DistributeEvent?.Invoke(this, DistributeTestType.CheckUpdateAsync);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var eventTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET");
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");

            // Start AppCenter.
            AppCenter.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Distribute));

            // Wait for "startService" log to be sent.
            await startServiceTask;
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Wait for processing event.
            var result = await eventTask;

            // Verify response.
            Assert.Equal("GET", result.Method);
            Assert.True(result.Uri.Contains("releases/latest?release_hash"));
            Assert.True(result.Uri.Contains(Config.ResolveAppSecret()));

            // Clear.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }

        [Fact]
        public async Task SetUpdateTrackPublicTest()
        {
            // Enable Distribute for debuggable builds.
            DistributeEvent?.Invoke(this, DistributeTestType.EnableDebuggableBuilds);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var eventTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET");
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");

            // Start AppCenter.
            AppCenter.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.UpdateTrack = UpdateTrack.Public;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Distribute));

            // Wait for "startService" log to be sent.
            await startServiceTask;
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Wait for processing event.
            var result = await eventTask;

            // Verify response.
            Assert.Equal("GET", result.Method);
            Assert.True(result.Uri.Contains("public"));
            Assert.True(result.Uri.Contains("releases/latest"));
            Assert.True(result.Uri.Contains(Config.ResolveAppSecret()));

            // Clear.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }

        [Fact]
        public async Task SetUpdateTrackPrivateTest()
        {
            // Enable Distribute for debuggable builds.
            DistributeEvent?.Invoke(this, DistributeTestType.EnableDebuggableBuilds);

            // Save data to preference.
            DistributeEvent?.Invoke(this, DistributeTestType.CheckUpdateAsync);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var eventTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET");
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");

            // Start AppCenter.
            AppCenter.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.UpdateTrack = UpdateTrack.Private;

            // MockUpdateToken.
            DistributeEvent?.Invoke(this, DistributeTestType.SaveMockUpdateToken);
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Distribute));

            // Wait for "startService" log to be sent.
            await startServiceTask;
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Wait for processing event.
            var result = await eventTask;

            // Verify response.
            Assert.Equal("GET", result.Method);
            Assert.False(result.Uri.Contains("public"));
            Assert.True(result.Uri.Contains("releases/private/latest?release_hash"));
            Assert.True(result.Uri.Contains(Config.ResolveAppSecret()));

            // Clear.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }

        [Theory]
        [InlineData(new object[] { null, "releases/latest" })]
        [InlineData(new object[] { UpdateTrack.Private, "releases/private/latest" })]
        [InlineData(new object[] { UpdateTrack.Public, "releases/latest" })]
        public async Task CheckForUpdateTest(UpdateTrack updateTrack, string urlDiff)
        {
            // Enable Distribute for debuggable builds.
            DistributeEvent?.Invoke(this, DistributeTestType.EnableDebuggableBuilds);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            HttpResponse response = new HttpResponse()
            {
                Content = GetReleaseJson("30", "3.0.0", false, 19),
                StatusCode = 200
            };
            var implicitCheckForUpdateTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET");
            var explicitCheckForUpdateTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET", response);
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");

            // Start AppCenter.
            AppCenter.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.UpdateTrack = updateTrack;

            // Save update token.
            if (updateTrack == UpdateTrack.Private)
            {
                DistributeEvent?.Invoke(this, DistributeTestType.SaveMockUpdateToken);
            }
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Distribute));

            // Wait for "startService" log to be sent.
            await startServiceTask;
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Wait for processing event.
            var result = await implicitCheckForUpdateTask;

            // Verify response.
            Assert.Equal("GET", result.Method);
            Assert.True(result.Uri.Contains(urlDiff));
            Assert.True(result.Uri.Contains(Config.ResolveAppSecret()));

            // Check for update.
            Distribute.CheckForUpdate();

            // Wait for processing event.
            result = await explicitCheckForUpdateTask;

            // Verify response.
            Assert.Equal("GET", result.Method);
            Assert.True(result.Uri.Contains(urlDiff));
            Assert.True(result.Uri.Contains(Config.ResolveAppSecret()));
        }

        [Fact]
        public async Task DisableAuthomaticCheckUpdateTest()
        {
            // Enable Distribute for debuggable builds.
            DistributeEvent?.Invoke(this, DistributeTestType.EnableDebuggableBuilds);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            HttpResponse response = new HttpResponse()
            {
                Content = GetReleaseJson("30", "3.0.0", false, 19),
                StatusCode = 200
            };
            var explicitCheckForUpdateTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET", response, 30);
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");

            // Start AppCenter.
            AppCenter.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.DisableAutomaticCheckForUpdate();
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Distribute));

            // Wait for "startService" log to be sent.
            await startServiceTask;
            Assert.Equal(1, httpNetworkAdapter.CallCount);
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Wait a 5s and verify that we will not have new calls.
            await Task.Delay(5000);
            Assert.Equal(1, httpNetworkAdapter.CallCount);

            // Check for update.
            Distribute.CheckForUpdate();

            // Wait for processing event.
            var result = await explicitCheckForUpdateTask;

            // Verify response.
            Assert.Equal(2, httpNetworkAdapter.CallCount);
            Assert.Equal("GET", result.Method);
            Assert.True(result.Uri.Contains("releases/latest"));
            Assert.True(result.Uri.Contains(Config.ResolveAppSecret()));
        }

        private string GetReleaseJson(string version, string shortVersion, bool isMandatory, int minApiVersion)
        {
            return $@"{{
                    ""id"": 42,
                    ""version"": ""{version}"",
                    ""short_version"": ""{shortVersion}"",
                    ""release_notes"": ""Fix a critical bug, this text was entered in App Center portal."",
                    ""release_notes_url"": ""https://mock/"",
                    ""{GetMinApiLevelKey()}"": {minApiVersion},
                    ""download_url"": ""http://download.thinkbroadband.com/1GB.zip"",
                    ""size"": 4242,
                    ""mandatory_update"": {isMandatory.ToString().ToLower()},
                    ""package_hashes"": [""9f52199c986d9210842824df695900e1656180946212bd5e8978501a5b732e60""],
                    ""distribution_group_id"": ""fd37a4b1-4937-45ef-97fb-b864154371f0"" }}";
        }

        private string GetMinApiLevelKey()
        {
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                return "min_os";
            }
            return "android_min_api_level";
        }
    }
}
