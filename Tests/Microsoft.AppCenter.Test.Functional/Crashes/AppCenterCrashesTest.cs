// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Crashes
{
    using Crashes = Microsoft.AppCenter.Crashes.Crashes;
    using AppCenter = Microsoft.AppCenter.AppCenter;

    public class AppCenterCrashesTest
    {
        // Before
        public AppCenterCrashesTest()
        {
            Utils.deleteDatabase();
        }

        [Fact]
        public async Task EnableDisableTest()
        {
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Crashes));

            // Disable Appcenter.
            await AppCenter.SetEnabledAsync(false);

            // Verify disabled.
            var isEnabled = await AppCenter.IsEnabledAsync();
            var isEnabledCrashes = await Crashes.IsEnabledAsync();
            Assert.False(isEnabled);
            Assert.False(isEnabledCrashes);

            // Restart SDK.
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Crashes));

            // Verify disabled.
            var isEnabled2 = await AppCenter.IsEnabledAsync();
            var isEnabledCrashes2 = await Crashes.IsEnabledAsync();
            Assert.False(isEnabled2);
            Assert.False(isEnabledCrashes2);

            // Enable SDK.
            await AppCenter.SetEnabledAsync(true);

            // Verify enabled.
            var isEnabled3 = await AppCenter.IsEnabledAsync();
            var isEnabledCrashes3 = await Crashes.IsEnabledAsync();
            Assert.True(isEnabled3);
            Assert.True(isEnabledCrashes3);

            // Restart SDK.
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Crashes));

            // Verify enabled.
            var isEnabled4 = await AppCenter.IsEnabledAsync();
            var isEnabledCrashes4 = await Crashes.IsEnabledAsync();
            Assert.True(isEnabled4);
            Assert.True(isEnabledCrashes4);
        }

        [Fact]
        public async Task TrackErrorTest()
        {
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();

            // Set up HttpNetworkAdapter.
            var typeEvent = "handledError";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");
            var eventTask = httpNetworkAdapter.MockRequestByLogType(typeEvent);

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Crashes));

            // Wait for "startService" log to be sent.
            await startServiceTask;

            Crashes.TrackError(new Exception("The answert is 42"));
            RequestData requestData = await eventTask;
            var events = requestData.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeEvent}')]").ToList();
            Assert.Equal(1, events.Count());
        }

        [Fact]
        public async Task SetUserIdTest()
        {
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();

            // Set up HttpNetworkAdapter.
            var typeEvent = "handledError";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");
            var eventTask = httpNetworkAdapter.MockRequestByLogType(typeEvent);

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Crashes));
            var userId = "I-am-test-user-id";
            AppCenter.SetUserId(userId);
            Crashes.TrackError(new Exception("The answert is 42"));

            // Wait for "startService" log to be sent.
            await startServiceTask;

            // Wait for processing event.
            RequestData requestData = await eventTask;
            var events = requestData.JsonContent?.SelectTokens($"$.logs[?(@.type == '{typeEvent}')]").ToList();
            Assert.Equal(1, events?.Count());
            var userIdFromLog = events?[0]["userId"];
            Assert.Equal(userIdFromLog, userId);
        }
    }
}
