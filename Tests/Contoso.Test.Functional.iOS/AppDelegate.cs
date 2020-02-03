// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Foundation;
using Microsoft.AppCenter.Test.Functional;
using Microsoft.AppCenter.Test.Functional.Distribute;
using UIKit;
using Xunit.Runner;
using Xunit.Runners.ResultChannels;
using Xunit.Sdk;

namespace Contoso.Test.Functional.iOS
{

    using iOSKeyChainUtil = Microsoft.AppCenter.iOS.Bindings.MSKeychainUtil;
    
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
            // Clear NSUserDefaults before the test.
            var plist = NSUserDefaults.StandardUserDefaults;
            var appDomain = NSBundle.MainBundle.BundleIdentifier;
            plist.RemovePersistentDomain(appDomain);
            
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
                case DistributeTestType.SaveMockUpdateToken:
                    iOSKeyChainUtil.StoreString(new NSString("xamarinUpdateToken"), new NSString("MSUpdateToken"));
                    break;
                case DistributeTestType.FreshInstallAsync:
                    plist.SetString("MSDownloadedReleaseId", Config.RequestId);
                    break;
                case DistributeTestType.CheckUpdateAsync:
                    plist.SetString(Config.RequestId, "MSDownloadedReleaseId");
                    plist.SetString("token", "MSUpdateTokenRequestId");
                    plist.SetString(Config.DistributionGroupId, "MSDistributionGroupId");
                    plist.SetString("hash", "MSDownloadedReleaseHash");
                    break;
                case DistributeTestType.Clear:
                    foreach (var i in plist.ToDictionary())
                    {
                        plist.RemoveObject(i.Key.ToString());
                    }
                    iOSKeyChainUtil.Clear();
                    break;
                case DistributeTestType.OnResumeActivity:
                    WillFinishLaunching(UiApplication, LaunchOptions);
                    break;
            }
        }
    }
}
