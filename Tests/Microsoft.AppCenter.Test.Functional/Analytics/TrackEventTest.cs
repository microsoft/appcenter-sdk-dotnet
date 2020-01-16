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

    public enum TrackEventTestType
    {
        OnResumeActivity,
        OnPauseActivity
    }

    public class TrackEventTest
    {
        private readonly string _appSecret = Guid.NewGuid().ToString();

        public delegate void TrackEventHandler(object sender, TrackEventTestType e);

        public static event TrackEventHandler TrackEvent;

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

        [Fact]
        public void TrackEventAfterBackground()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            StartAppCenter();

            // Stop mosule.
            Analytics.SetEnabledAsync(false).Wait();

            // Go to background.
            TrackEvent?.Invoke(this, TrackEventTestType.OnPauseActivity);

            //Wait 20 sec.
            Task.Delay(25000).Wait();

            // Go to foreground.
            TrackEvent?.Invoke(this, TrackEventTestType.OnResumeActivity);

            // Track events.
            Analytics.TrackEvent("TrackEvent 1");
            Analytics.TrackEvent("TrackEvent 2");
            Analytics.TrackEvent("TrackEvent 3");

            // Wait for processing any event.
            httpNetworkAdapter.HttpResponseTask.Wait(5000);

            // Verify.
            Assert.Equal(0, httpNetworkAdapter.CallCount);
        }

        [Fact]
        public async Task TrackEventNormalFlowAsync()
        {
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Pause Analytics module.
            Analytics.Pause();

            // Test TrackEvent.
            Analytics.TrackEvent("TrackEvent 1");

            // Wait for processing event.
            httpNetworkAdapter.HttpResponseTask.Wait(5000);

            Assert.Equal(0, httpNetworkAdapter.CallCount);

            // Resume Analytics module.
            Analytics.Resume();

            // Test TrackEvent.
            Analytics.TrackEvent("TrackEvent 2");

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();
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
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Pause Analytics module.
            Analytics.Pause();

            // Test TrackEvents.
            Analytics.TrackEvent("TrackEvent 1");
            Analytics.TrackEvent("TrackEvent 2");
            Analytics.TrackEvent("TrackEvent 3");

            // Pause Analytics module again.
            Analytics.Pause();

            // Wait for processing event.
            httpNetworkAdapter.HttpResponseTask.Wait(5000);

            // Check that TrackEvent was never called.
            Assert.Equal(0, httpNetworkAdapter.CallCount);

            // Resume Analytics module.
            Analytics.Resume();

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify. The start session can be in same batch as the event HTTP request so look for it inside.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'event')]").ToList();

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

        private void StartAppCenter()
        {
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Resume Activity.
            TrackEvent?.Invoke(this, TrackEventTestType.OnResumeActivity);

            // Wait start Analytics module.
            Analytics.IsEnabledAsync().Wait();
        }
    }
}
