// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Distribute
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;
    using AppCenter = Microsoft.AppCenter.AppCenter;

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
        public async Task GetLastReleaseDetailsAsync()
        {
            // Save data to preference.
            DistributeEvent?.Invoke(this, DistributeTestType.CheckUpdateAsync);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            Func<RequestData, bool> logTypeRule = (RequestData arg) =>
            {
                return arg.Method == "GET";
            };
            var eventTask = httpNetworkAdapter.MockRequest(logTypeRule);


            // Start AppCenter.
            AppCenter.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Distribute));
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Wait for processing event.
            var result = await eventTask;

            // Verify response.
            Assert.Equal("GET", result.Method);
            Assert.True(result.Uri.Contains("releases/latest?release_hash"));
            Assert.True(result.Uri.Contains(Config.resolveAppsecret()));

            // Clear.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }
    }
}
