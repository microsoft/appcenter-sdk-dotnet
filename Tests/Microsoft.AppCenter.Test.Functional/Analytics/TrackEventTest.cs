// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var typeEvent = "event";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");
            var eventTask = httpNetworkAdapter.MockRequestByLogType(typeEvent);

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
            var eventLogs = requestData.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeEvent}')]").ToList();
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
            var typeEvent = "event";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");
            var eventTask = httpNetworkAdapter.MockRequestByLogType(typeEvent);

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
            var eventLogs = requestData.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeEvent}')]").ToList();
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

        [Fact]
        public async Task TrackEventNormalFlowAsync()
        {
            var typeEvent = "event";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var eventTask = httpNetworkAdapter.MockRequestByLogType(typeEvent);

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Pause Analytics module.
            Analytics.Pause();

            // Test TrackEvent.
            Analytics.TrackEvent("TrackEvent 1");

            // Wait for 5 seconds to allow batching happen (3 seconds), and verify nothing has been sent.
            Task.WaitAny(eventTask, Task.Delay(5000));
            Assert.Equal(0, httpNetworkAdapter.CallCount);

            // Resume Analytics module.
            Analytics.Resume();

            // Test TrackEvent.
            Analytics.TrackEvent("TrackEvent 2");

            // Wait for processing event.
            var result = await eventTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", result.Method);
            var eventLogs = result.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();
            Assert.Equal(2, eventLogs.Count());

            // Check that the first event was sent.
            var actualEventName = (string)eventLogs[0]["name"];
            Assert.Equal("TrackEvent 1", actualEventName);
            var typedProperties = eventLogs[0]["typedProperties"];
            Assert.Null(typedProperties);

            // Check that the second event was sent.
            actualEventName = (string)eventLogs[1]["name"];
            Assert.Equal("TrackEvent 2", actualEventName);
            typedProperties = eventLogs[1]["typedProperties"];
            Assert.Null(typedProperties);

            // Check count calls.
            Assert.Equal(1, httpNetworkAdapter.CallCount);
        }

        [Fact]
        public async Task TrackEventMulticlickFlowAsync()
        {
            var typeEvent = "event";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var eventTask = httpNetworkAdapter.MockRequestByLogType(typeEvent);

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Pause Analytics module.
            Analytics.Pause();

            // Test TrackEvents.
            Analytics.TrackEvent("TrackEvent 1");
            Analytics.TrackEvent("TrackEvent 2");
            Analytics.TrackEvent("TrackEvent 3");

            // Pause Analytics module again.
            Analytics.Pause();

            // Wait for 5 seconds to allow batching happen (3 seconds), and verify nothing has been sent.
            Task.WaitAny(eventTask, Task.Delay(5000));
            Assert.Equal(0, httpNetworkAdapter.CallCount);

            // Resume Analytics module.
            Analytics.Resume();

            // Wait for processing event.
            var result = await eventTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", result.Method);
            var eventLogs = result.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeEvent}')]").ToList();

            // Check that the first event was sent.
            var actualEventName = (string)eventLogs[0]["name"];
            Assert.Equal("TrackEvent 1", actualEventName);
            var typedProperties = eventLogs[0]["typedProperties"];
            Assert.Null(typedProperties);

            // Check that the second event was sent.
            actualEventName = (string)eventLogs[1]["name"];
            Assert.Equal("TrackEvent 2", actualEventName);
            typedProperties = eventLogs[1]["typedProperties"];
            Assert.Null(typedProperties);

            // Check that the third event was sent.
            actualEventName = (string)eventLogs[2]["name"];
            Assert.Equal("TrackEvent 3", actualEventName);
            typedProperties = eventLogs[2]["typedProperties"];
            Assert.Null(typedProperties);

            // Check count calls.
            Assert.Equal(1, httpNetworkAdapter.CallCount);
        }
    }
}
