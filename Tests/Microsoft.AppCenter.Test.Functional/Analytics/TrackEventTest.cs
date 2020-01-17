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
    using AppCenter = Microsoft.AppCenter.AppCenter;

    public class TrackEventTest
    {
        // Before
        public TrackEventTest()
        {
            Utils.deleteDatabase();
        }

        [Fact]
        public async Task TrackEventWithoutPropertiesAsync()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequest("startService");
            var eventTask = httpNetworkAdapter.MockRequest("event");

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Wait for "startService" log to be sent.
            await startServiceTask;

            // Test TrackEvent.
            Analytics.TrackEvent("Hello World");

            // Wait for processing event.
            RequestData requestData = await eventTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", requestData.Method);
            var eventLogs = requestData.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();
            Assert.Equal(1, eventLogs.Count());
            var eventLog = eventLogs[0];
            var actualEventName = (string)eventLog["name"];
            Assert.Equal("Hello World", actualEventName);
            var typedProperties = eventLog["typedProperties"];
            Assert.Null(typedProperties);
            Assert.Equal(2, httpNetworkAdapter.CallCount);
        }

        [Fact]
        public async Task TrackEventWithPropertiesAsync()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequest("startService");
            var eventTask = httpNetworkAdapter.MockRequest("event");

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Wait for "startService" log to be sent.
            await startServiceTask;

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
            RequestData requestData = await eventTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", requestData.Method);
            var eventLogs = requestData.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();
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
            Assert.Equal(2, httpNetworkAdapter.CallCount);
        }
    }
}
