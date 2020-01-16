// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Crashes
{
    using Crashes = Microsoft.AppCenter.Crashes.Crashes;

    public class AppCenterCrashesTest
    {
        private readonly string _appSecret = Guid.NewGuid().ToString();

        [Fact]
        public async Task EnableDisableTest()
        {
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Crashes));

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
            AppCenter.Start(_appSecret, typeof(Crashes));

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
            AppCenter.Start(_appSecret, typeof(Crashes));

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
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "handledError");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Crashes));

            Crashes.TrackError(new Exception("The answert is 42"));
            await httpNetworkAdapter.HttpResponseTask;
            var events = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'handledError')]").ToList();
            Assert.Equal(1, events.Count());
        }

        [Fact]
        public async Task SetUserIdTest()
        {
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();

            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "handledError");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Crashes));
            var userId = "I-am-test-user-id";
            AppCenter.SetUserId(userId);

            Crashes.TrackError(new Exception("The answert is 42"));
            await httpNetworkAdapter.HttpResponseTask;
            var events = httpNetworkAdapter.JsonContent?.SelectTokens($"$.logs[?(@.type == 'handledError')]").ToList();
            Assert.Equal(1, events?.Count());
            Assert.Contains(userId, events?[0]);
        }
    }
}
