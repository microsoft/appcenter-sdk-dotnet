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

    public class AppCenterCoreTest : IDisposable
    {

        // Before
        public AppCenterCoreTest()
        {
            Utils.deleteDatabase();
        }

        // After
        public void Dispose()
        {
            // Let pending SDK calls be completed.
            Task.Delay(3000).Wait();
        }

        [Fact]
        public async Task EnableDisableTest()
        {
            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Disable Appcenter.
            await AppCenter.SetEnabledAsync(false);

            // Verify disabled.
            var isEnabled = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics = await Analytics.IsEnabledAsync();
            Assert.False(isEnabled);
            Assert.False(isEnabledAnalytics);

            // Restart SDK.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Verify disabled.
            var isEnabled2 = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics2 = await Analytics.IsEnabledAsync();
            Assert.False(isEnabled2);
            Assert.False(isEnabledAnalytics2);

            // Enable SDK.
            await AppCenter.SetEnabledAsync(true);

            // Verify enabled.
            var isEnabled3 = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics3 = await Analytics.IsEnabledAsync();
            Assert.True(isEnabled3);
            Assert.True(isEnabledAnalytics3);

            // Restart SDK.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Verify enabled.
            var isEnabled4 = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics4 = await Analytics.IsEnabledAsync();
            Assert.True(isEnabled4);
            Assert.True(isEnabledAnalytics4);

            // Let pending SDK calls be completed, we have a lot of "startService" calls.
            Task.Delay(5000).Wait();
        }

        [Fact]
        public async Task CustomPropertiesTest()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "customProperties");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Enable Appcenter.
            await AppCenter.SetEnabledAsync(true);

            // Verify enabled.
            var isEnabled = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics = await Analytics.IsEnabledAsync();
            Assert.True(isEnabled);
            Assert.True(isEnabledAnalytics);

            // Set custom properties.
            var propertiesDictionary = new Dictionary<string, object>
            {
                { "keyBoolean", true },
                { "keyString", "value" },
                { "keyInt", 42 },
                { "keyDateTime", new DateTime() },
            };
            var customProperties = new CustomProperties();
            customProperties.Set("keyBoolean", true);
            customProperties.Set("keyString", "value");
            customProperties.Set("keyInt", 42);
            customProperties.Set("keyDateTime", new DateTime());
            AppCenter.SetCustomProperties(customProperties);

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogs = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'customProperties')]").ToList();

            // Verify the log sctructure.
            Assert.Equal(1, eventLogs.Count());
            var eventLog = eventLogs[0];
            var properties = eventLog["properties"];
            Assert.NotNull(properties);
            Assert.Equal(4, properties.Count());

            // Verify initial dictionary has the values.
            foreach (var item in properties)
            {
                Assert.NotNull(propertiesDictionary[(string)item.SelectToken("name")]);
            }

            Assert.Equal(1, httpNetworkAdapter.CallCount);
        }

        [Fact]
        public async Task CustomPropertiesClearTest()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "customProperties");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.resolveAppsecret(), typeof(Analytics));

            // Enable Appcenter.
            await AppCenter.SetEnabledAsync(true);

            // Verify enabled.
            var isEnabled = await AppCenter.IsEnabledAsync();
            var isEnabledAnalytics = await Analytics.IsEnabledAsync();
            Assert.True(isEnabled);
            Assert.True(isEnabledAnalytics);

            // Set custom properties.
            var propertiesDictionary = new Dictionary<string, object>
            {
                { "keyBoolean", true },
                { "keyString", "value" },
                { "keyInt", 42 },
                { "keyDateTime", new DateTime() },
            };

            // Clear custom properties.
            var customPropertiesClear = new CustomProperties();
            foreach (var item in propertiesDictionary)
            {
                customPropertiesClear.Clear(item.Key);
            }
            AppCenter.SetCustomProperties(customPropertiesClear);

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var eventLogsClear = httpNetworkAdapter.JsonContent.SelectTokens($"$.logs[?(@.type == 'customProperties')]").ToList();

            // Verify the log sctructure.
            Assert.Equal(1, eventLogsClear.Count());
            var eventLogClear = eventLogsClear[0];
            var propertiesClear = eventLogClear["properties"];
            Assert.NotNull(propertiesClear);
            Assert.Equal(4, propertiesClear.Count());

            // Verify initial dictionary has the values.
            foreach (var item in propertiesClear)
            {
                Assert.Equal((string)item.SelectToken("type"), "clear");
            }
            Assert.Equal(1, httpNetworkAdapter.CallCount);
        }
    }
}
