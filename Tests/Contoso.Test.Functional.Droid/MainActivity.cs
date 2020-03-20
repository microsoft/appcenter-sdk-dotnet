// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Test.Functional.Distribute;
using Xunit.Runners.ResultChannels;
using Xunit.Runners.UI;
using Config = Microsoft.AppCenter.Test.Functional.Config;

namespace Contoso.Test.Functional.Droid
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;

    [Activity(Label = "xUnit Android Runner", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {
        private const string ResultChannelHost = "10.0.2.2";

        protected override void OnCreate(Bundle bundle)
        {
            // Clear shared preferences before the test.
            var prefs = GetSharedPreferences("AppCenter", FileCreationMode.Private);
            prefs.Edit().Clear();

            // Register tests from shared library.
            AddTestAssembly(typeof(Config).Assembly);

            // Try to send results to the host via a socket for CI.
            try
            {
                ResultChannel = TrxResultChannel.CreateTcpTrxResultChannel(ResultChannelHost, Config.ResultChannelPort).Result;
            }
            catch (Exception e)
            {
                Log.Warn("AppCenterTest", $"Could not connect to host for reporting results.\n{e}");
            }

            // start running the test suites as soon as the application is loaded
            AutoStart = true;

#if !DEBUG
            // crash the application (to ensure it's ended) and return to springboard
            TerminateAfterExecution = true;
#endif
            Distribute.SetEnabledForDebuggableBuild(true);
            DistributeUpdateTest.DistributeEvent += ConfigureDataForDistribute;

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }

        private void ConfigureDataForDistribute(object sender, DistributeTestType distributeTestType)
        {
            var prefs = GetSharedPreferences("AppCenter", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            switch (distributeTestType)
            {
                case DistributeTestType.EnableDebuggableBuilds:
                    Distribute.SetEnabledForDebuggableBuild(true);
                    break;
                case DistributeTestType.FreshInstallAsync:
                    prefEditor.PutString("Distribute.request_id", Config.RequestId);
                    break;
                case DistributeTestType.CheckUpdateAsync:
                    prefEditor.PutString("Distribute.request_id", Config.RequestId);
                    prefEditor.PutString("Distribute.update_token", "token");
                    prefEditor.PutString("Distribute.distribution_group_id", Config.DistributionGroupId);
                    prefEditor.PutString("Distribute.downloaded_release_hash", "hash");
                    break;
                case DistributeTestType.Clear:
                    prefEditor.Remove("Distribute.request_id");
                    prefEditor.Remove("Distribute.update_token");
                    prefEditor.Remove("Distribute.distribution_group_id");
                    prefEditor.Remove("Distribute.downloaded_release_hash");
                    break;
                case DistributeTestType.SaveMockUpdateToken:
                    prefEditor.PutString("Distribute.update_token", "token");
                    break;
                case DistributeTestType.OnResumeActivity:
                    OnResume();
                    break;
            }
            prefEditor.Commit();
        }
    }
}
