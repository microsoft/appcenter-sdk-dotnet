using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Distribute
{
    using Distribute = Microsoft.AppCenter.Distribute.Distribute;

    public enum DistributeTestType
    {
        FreshInstallAsync,
        CheckUpdateAsync,
        OnResumeActivity,
        Clear
    }

    public class DistributeUpdateTest
    {
        public delegate void DistributeEventHandler(object sender, DistributeTestType e);

        public static event DistributeEventHandler DistributeEvent;

        [Fact]
        public async Task GetLastReleaseDetailsAsync()
        {
            // Save data to preference.
            DistributeEvent?.Invoke(this, DistributeTestType.CheckUpdateAsync);

            // Setup network adapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start AppCenter.
            AppCenter.UnsetInstance();
            Distribute.UnsetInstance();
            AppCenter.LogLevel = LogLevel.Verbose;
            Distribute.SetEnabledForDebuggableBuild(true);
            AppCenter.Start(Config._appSecret, typeof(Distribute));
            DistributeEvent?.Invoke(this, DistributeTestType.OnResumeActivity);

            // Wait when Distribute will start.
            await Distribute.IsEnabledAsync();

            // Disable and enable distribute.
            Distribute.SetEnabledAsync(false).Wait();
            Distribute.SetEnabledAsync(true).Wait();

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify response.
            Assert.Equal("GET", httpNetworkAdapter.Method);
            Assert.True(httpNetworkAdapter.Uri.Contains("releases/latest?release_hash"));
            Assert.True(httpNetworkAdapter.Uri.Contains(Config._appSecret));

            // Clear.
            DistributeEvent?.Invoke(this, DistributeTestType.Clear);
        }
    }
}
