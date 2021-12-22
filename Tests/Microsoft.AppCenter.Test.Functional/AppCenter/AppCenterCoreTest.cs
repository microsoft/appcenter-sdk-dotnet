// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.AppCenter
{
    using Analytics = Microsoft.AppCenter.Analytics.Analytics;
    using AppCenter = Microsoft.AppCenter.AppCenter;

    public class AppCenterCoreTest
    {
        // Before
        public AppCenterCoreTest()
        {
            Utils.DeleteDatabase();
        }

        [Fact]
        public async Task EnableDisableTest()
        {
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");
            var startServiceTask2 = httpNetworkAdapter.MockRequestByLogType("startService");
            var startServiceTask3 = httpNetworkAdapter.MockRequestByLogType("startService");

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Analytics));

            // Wait for "startService" log to be sent.
            Task.WaitAny(startServiceTask, Task.Delay(5000));

            // Disable Appcenter.
            await AppCenter.SetEnabledAsync(false);

            // On iOS when set disabled all appcenter logs are removing from DB. We should wait here otherwise there might be a deadlock.
            Task.Delay(3000).Wait();

            // Verify disabled.
            var isEnabled = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics = await Analytics.IsEnabledAsync();
            Assert.False(isEnabled);
            Assert.False(isEnabledAnalytics);

            // Restart SDK.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Analytics));

            // On iOS when started in disabled mode SDK will try to remove all pending logs. We should wait here otherwise there might be a deadlock.
            Task.Delay(3000).Wait();

            // Verify disabled.
            var isEnabled2 = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics2 = await Analytics.IsEnabledAsync();
            Assert.False(isEnabled2);
            Assert.False(isEnabledAnalytics2);

            // Enable SDK.
            await AppCenter.SetEnabledAsync(true);

            // Wait for "startService" log to be sent.
            Task.WaitAny(startServiceTask2, Task.Delay(5000));

            // Verify enabled.
            var isEnabled3 = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics3 = await Analytics.IsEnabledAsync();
            Assert.True(isEnabled3);
            Assert.True(isEnabledAnalytics3);

            // Restart SDK.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Analytics));

            // Wait for "startService" log to be sent.
            Task.WaitAny(startServiceTask3, Task.Delay(5000));

            // Verify enabled.
            var isEnabled4 = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics4 = await Analytics.IsEnabledAsync();
            Assert.True(isEnabled4);
            Assert.True(isEnabledAnalytics4);
        }
    }
}
