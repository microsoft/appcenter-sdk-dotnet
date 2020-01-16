// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Test.Functional;
using Microsoft.AppCenter.Test.Functional.Distribute;
using UIKit;
using Xunit.Runner;
using Xunit.Runners.ResultChannels;
using Xunit.Sdk;

namespace Contoso.Test.Functional.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : RunnerAppDelegate
    {
        private const string ResultChannelHost = "127.0.0.1";

        private static UIApplication UiApplication;
        private static NSDictionary LaunchOptions;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            // Register tests from shared library.
            AddTestAssembly(typeof(Config).Assembly);

            // Try to send results to the host via a socket for CI.
            try
            {
                ResultChannel = TrxResultChannel.CreateTcpTrxResultChannel(ResultChannelHost, Config.ResultChannelPort).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[AppCenterTest] WARN Could not connect to host for reporting results.\n{e}");
            }

            // start running the test suites as soon as the application is loaded
            AutoStart = true;

#if !DEBUG
            // crash the application (to ensure it's ended) and return to springboard
            TerminateAfterExecution = true;
#endif
            DistributeUpdateTest.DistributeEvent += ConfigureDataForDistribute;
            UiApplication = uiApplication;
            LaunchOptions = launchOptions;
            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        private void ConfigureDataForDistribute(object sender, DistributeTestType distributeTestType)
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            switch (distributeTestType)
            {
                case DistributeTestType.FreshInstallAsync:
                    plist.SetString("MSDownloadedReleaseId", Config._requestId);
                    break;
                case DistributeTestType.CheckUpdateAsync:
                    plist.SetString("MSDownloadedReleaseId", Config._requestId);
                    plist.SetString("MSUpdateTokenRequestId", "token");
                    plist.SetString("MSDistributionGroupId", Config._distributionGroupId);
                    plist.SetString("MSDownloadedReleaseHash", "hash");
                    break;
                case DistributeTestType.Clear:
                    plist.RemoveObject("MSUpdateTokenRequestId");
                    plist.RemoveObject("MSUpdateTokenRequestId");
                    plist.RemoveObject("MSDistributionGroupId");
                    plist.RemoveObject("MSDownloadedReleaseHash");
                    break;
                case DistributeTestType.OnResumeActivity:
                    FinishedLaunching(UiApplication, LaunchOptions);
                    break;
            }
        }
    }
}

