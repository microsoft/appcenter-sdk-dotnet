// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Microsoft.AppCenter;
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

        private readonly string _appSecret = "e94aaff4-e80d-4fee-9a5f-a84eb6e688fc";

        private static string _requestId = "b627efb5-dbf7-4350-92e4-b6ac4dbd09b0";

        protected override void OnCreate(Bundle bundle)
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
                Log.Warn("AppCenterTest", $"Could not connect to host for reporting results.\n{e}");
            }

            // start running the test suites as soon as the application is loaded
            AutoStart = true;

#if !DEBUG
            // crash the application (to ensure it's ended) and return to springboard
            TerminateAfterExecution = true;
#endif

            var prefs = GetSharedPreferences("AppCenter", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("Distribute.request_id", _requestId);
            prefEditor.Commit();
            AppCenter.Start(_appSecret, typeof(Distribute));

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}
