// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
        [InlineData(new object[] { UpdateTrack.Private, "releases/private/latest" })]
        [InlineData(new object[] { UpdateTrack.Public, "releases/latest" })]
        public async Task CheckForUpdateTest(UpdateTrack updateTrack, string urlDiff)
        {
            string json = "{" +
                "id: 42," +
                "version: '14'," +
                "short_version: '2.1.5'," +
                "release_notes: 'Fix a critical bug, this text was entered in App Center portal.'," +
                "release_notes_url: 'https://mock/'," +
                "android_min_api_level: 19," +
                "download_url: 'http://download.thinkbroadband.com/1GB.zip'," +
                "size: 4242," +
                "mandatory_update: false," +
                "package_hashes: ['9f52199c986d9210842824df695900e1656180946212bd5e8978501a5b732e60']," +
                "distribution_group_id: 'fd37a4b1-4937-45ef-97fb-b864154371f0'" +
                "}";

            // Enable Distribute for debuggable builds.
            DistributeEvent?.Invoke(this, DistributeTestType.EnableDebuggableBuilds);

            if (updateTrack == UpdateTrack.Private)
			{
                // Save data to preference.
                DistributeEvent?.Invoke(this, DistributeTestType.CheckUpdateAsync);
            }

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            HttpResponse response = new HttpResponse()
            {
                Content = json,
                StatusCode = 200
            };
            var implicitCheckForUpdateTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET");
            var explicitCheckForUpdateTask = httpNetworkAdapter.MockRequest(request => request.Method == "GET", response);
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");

            // Start AppCenter.
            AppCenter.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.UpdateTrack = updateTrack;

            if (updateTrack == UpdateTrack.Private)
            {
                // MockUpdateToken.
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
    }
}
