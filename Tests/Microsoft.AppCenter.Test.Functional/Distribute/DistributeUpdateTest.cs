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
            Assert.True(result.Uri.Contains("releases/latest?release_hash"));
            Assert.True(result.Uri.Contains(Config.ResolveAppSecret()));

            // Clear.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }
    }
}
