using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
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

			// you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
			//Writer = new TcpTextWriter ("10.0.1.2", 16384);
            Task.Run(async () =>
            {
                ResultChannel = await TrxResultChannel.CreateTcpTrxResultChannel("10.0.2.2", 16384);
            });

            // start running the test suites as soon as the application is loaded
            AutoStart = true;
			// crash the application (to ensure it's ended) and return to springboard
			TerminateAfterExecution = true;

            // you cannot add more assemblies once calling base
            base.OnCreate(bundle);
        }
    }
}
