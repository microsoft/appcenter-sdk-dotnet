using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;


namespace Contoso.Forms.Test
{
#if TIZEN
    public delegate void RequestUpdateCallback();

    public class ErrorReportPageUpdateCallback
    {
        public static LastSessionErrorReportPage page { get; set; }

        public static RequestUpdateCallback RequestUpdate { get; set; }

        public static void Update(ErrorReport errorReport)
        {
            if (page != null)
                Device.BeginInvokeOnMainThread(() => page.UpdateLabels(errorReport));
        }
    };
#endif

    public partial class LastSessionErrorReportPage : ContentPage
    {
        readonly string _nullText;

        public LastSessionErrorReportPage()
        {
            InitializeComponent();
            _nullText = ExceptionTypeLabel.Text;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
#if TIZEN
            // Implemented in Tizen-specific part
            ErrorReportPageUpdateCallback.page = this;
            ErrorReportPageUpdateCallback.RequestUpdate();
#else
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
            {
                Device.BeginInvokeOnMainThread(() => UpdateLabels(task.Result));
            });
#endif
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        public void UpdateLabels(ErrorReport errorReport)
        {
            ExceptionTypeLabel.Text = errorReport?.Exception?.GetType().Name ?? _nullText;
            ExceptionMessageLabel.Text = errorReport?.Exception?.Message ?? _nullText;
            AppStartTimeLabel.Text = errorReport?.AppStartTime.ToString() ?? _nullText;
            AppErrorTimeLabel.Text = errorReport?.AppErrorTime.ToString() ?? _nullText;
            IdLabel.Text = errorReport?.Id ?? _nullText;
            DeviceLabel.Text = errorReport?.Device != null ? TestStrings.DeviceReportedText : _nullText;
            iOSDetailsLabel.Text = errorReport?.iOSDetails != null ? TestStrings.HasiOSDetailsText : _nullText;
            AndroidDetailsLabel.Text = errorReport?.AndroidDetails != null ? TestStrings.HasAndroidDetailsText : _nullText;
        }
    }
}
