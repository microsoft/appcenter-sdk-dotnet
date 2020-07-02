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
            Utils.DeleteDatabase();
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
            var eventTask1 = httpNetworkAdapter.MockRequestByLogType(typeEvent);
            var eventTask2 = httpNetworkAdapter.MockRequestByLogType(typeEvent);

            // Start App Center.
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Crashes));

            // Wait for "startService" log to be sent.
            await startServiceTask;

            // Check track error without attachments.
            Crashes.TrackError(new Exception("The answert is 1"));
            RequestData requestData1 = await eventTask1;
            var events1 = requestData1.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeEvent}')]").ToList();
            Assert.Single(events1);

            // Check track error when attachments are null.
            Crashes.TrackError(new Exception("The answert is 2"), null, null);
            RequestData requestData2 = await eventTask2;
            var events2 = requestData2.JsonContent.SelectTokens($"$.logs[?(@.type == '{typeEvent}')]").ToList();
            Assert.Single(events2);
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
            AppCenter.Start(Config.ResolveAppSecret(), typeof(Crashes));
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
