// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Reflection;
using Android.App;
using Android.OS;
using Android.Util;
using Xunit.Runners.ResultChannels;
using Xunit.Runners.UI;

namespace Contoso.Android.FuncTest
{
    [Activity(Label = "xUnit Android Runner", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            // tests can be inside the main assembly
            AddTestAssembly(Assembly.GetExecutingAssembly());

            //AddTestAssembly(typeof(PortableTests).Assembly);
            // or in any assembly that you load (since JIT is available)

            // Try to send results to the host via a socket for CI.
            try
            {
                ResultChannel = TrxResultChannel.CreateTcpTrxResultChannel("10.0.2.2", 16384).Result;
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

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}
