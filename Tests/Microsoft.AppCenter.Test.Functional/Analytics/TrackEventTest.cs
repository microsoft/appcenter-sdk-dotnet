// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Analytics
{
    using Analytics = Microsoft.AppCenter.Analytics.Analytics;

    // TODO: Move to AppCenterCrashesTest.cs after resolving problem with tests
    using Crashes = Microsoft.AppCenter.Crashes.Crashes;

    public class TrackEventTest
    {
        private readonly string _appSecret = Guid.NewGuid().ToString();

        [Fact]
        public async Task TrackEventWithoutPropertiesAsync()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Test TrackEvent.
            Analytics.TrackEvent("Hello World");

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();
            Assert.Equal(1, eventLogs.Count());
            var eventLog = eventLogs[0];
            var actualEventName = (string)eventLog["name"];
            Assert.Equal("Hello World", actualEventName);
            var typedProperties = eventLog["typedProperties"];
            Assert.Null(typedProperties);
            Assert.Equal(1, httpNetworkAdapter.CallCount);
        }

        [Fact]
        public async Task TrackEventWithPropertiesAsync()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Build event properties.
            var properties = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
                { "Key3", "Value3" }
            };

            // Test TrackEvent.
            Analytics.TrackEvent("Hello World", properties);

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();
            Assert.Equal(1, eventLogs.Count());
            var eventLog = eventLogs[0];
            var actualEventName = (string)eventLog["name"];
            Assert.Equal("Hello World", actualEventName);
            var typedProperties = eventLog["typedProperties"];
            Assert.NotNull(typedProperties);
            Assert.Equal(3, typedProperties.Count());
            for (var i = 1; i <= 3; i++)
            {
                Assert.NotNull(typedProperties.SelectToken($"[?(@.name == 'Key{i}' && @.value == 'Value{i}')]"));
            }
            Assert.Equal(1, httpNetworkAdapter.CallCount);
        }

        
        // TODO: Move following methods to AppCenterCrashesTest.cs after resolving issues with tests
        [Fact]
        public async Task EnableDisableTest()
        {

            // Start App Center.
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();
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
        public async Task SetUserIdTest()
        {

            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.UnsetInstance();
            Crashes.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Crashes));

            Crashes.TrackError(new Exception("The answert is 42"));
            await httpNetworkAdapter.HttpResponseTask;
            var events = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();
            Assert.Equal(1, events.Count());
        }
    }
}
