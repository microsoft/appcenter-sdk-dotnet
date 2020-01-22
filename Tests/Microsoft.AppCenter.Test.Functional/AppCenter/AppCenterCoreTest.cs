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

        [Fact]
        public async Task CustomPropertiesTest()
        {
            // Set up HttpNetworkAdapter.
            var typeProperty = "customProperties";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");
            var customPropertiesTask = httpNetworkAdapter.MockRequestByLogType(typeProperty);

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Analytics));

            // Wait for "startService" log to be sent.
            await startServiceTask;

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
            RequestData requestData = await customPropertiesTask;
            Assert.Equal("POST", requestData.Method);
            var eventLogs = requestData.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeProperty}')]").ToList();

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

            Assert.Equal(2, httpNetworkAdapter.CallCount);
        }

        [Fact]
        public async Task CustomPropertiesClearTest()
        {
            // Set up HttpNetworkAdapter.
            var typeProperty = "customProperties";
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;
            var startServiceTask = httpNetworkAdapter.MockRequestByLogType("startService");
            var customPropertiesTask = httpNetworkAdapter.MockRequestByLogType(typeProperty);

            // Start App Center.
            AppCenter.UnsetInstance();
            Analytics.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Analytics));

            // Wait for "startService" log to be sent.
            await startServiceTask;

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
            RequestData requestData = await customPropertiesTask;
            Assert.Equal("POST", requestData.Method);
            var eventLogsClear = requestData.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeProperty}')]").ToList();

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
            Assert.Equal(2, httpNetworkAdapter.CallCount);
        }
    }
}
