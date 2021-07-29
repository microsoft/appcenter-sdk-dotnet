using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.ObjectModel;
using Contoso.UtilClassLibrary;
using System.Runtime.InteropServices;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Contoso.WinUI.Desktop.Demo
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private string fileAttachments;
        private string textAttachments;

        private ObservableCollection<Property> EventPropertiesSource = new ObservableCollection<Property>();
        private ObservableCollection<Property> ErrorPropertiesSource = new ObservableCollection<Property>();

        private ApplicationDataContainer localSettings;

        public MainWindow()
        {
            this.InitializeComponent();
            localSettings = ApplicationData.Current.LocalSettings;
            UpdateState();
            EventProperties.ItemsSource = EventPropertiesSource;
            ErrorProperties.ItemsSource = ErrorPropertiesSource;
            fileAttachments = localSettings.Values[Constants.KeyFileErrorAttachments] as string;
            textAttachments = localSettings.Values[Constants.KeyTextErrorAttachments] as string;
            TextAttachmentTextBox.Text = textAttachments;
            FileAttachmentLabel.Text = fileAttachments ?? "The file isn't selected";
            var countryCode = localSettings.Values[Constants.KeyCountryCode] as string;
            if (!string.IsNullOrEmpty(countryCode))
            {
                CountryCodeEnableCheckbox.IsChecked = true;
                CountryCodeText.Text = countryCode;
            }
            var userId = localSettings.Values[Constants.KeyUserId] as string;
            if (!string.IsNullOrEmpty(userId))
            {
                UserId.Text = userId;
            }
            var storageSize = localSettings.Values[Constants.KeyStorageMaxSize] as long?;
            if (storageSize != null && storageSize > 0)
            {
                StorageMaxSize.Text = storageSize.ToString();
            }
        }

        private void UpdateState()
        {
            AppCenterEnabled.IsChecked = AppCenter.IsEnabledAsync().Result;
            AppCenterAllowNetworkRequests.IsChecked = AppCenter.IsNetworkRequestsAllowed;
            CrashesEnabled.IsChecked = Crashes.IsEnabledAsync().Result;
            AnalyticsEnabled.IsChecked = Analytics.IsEnabledAsync().Result;
            AnalyticsEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
            CrashesEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateState();
        }

        #region AppCenter

        private void AppCenterAllowNetworkRequests_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleAllowNetworkRequest();
        }

        private void AppCenterAllowNetworkRequests_Checked(object sender, RoutedEventArgs e)
        {
            HandleAllowNetworkRequest();
        }

        private void HandleAllowNetworkRequest()
        {
            if (AppCenterAllowNetworkRequests.IsChecked.HasValue)
            {
                AppCenter.IsNetworkRequestsAllowed = AppCenterAllowNetworkRequests.IsChecked.Value;
            }
        }

        private void AppCenterEnabled_Checked(object sender, RoutedEventArgs e)
        {
            HandleAppCenterEnabled();
        }

        private void AppCenterEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleAppCenterEnabled();
        }

        private void HandleAppCenterEnabled()
        {
            if (AppCenterEnabled.IsChecked.HasValue)
            {
                AppCenter.SetEnabledAsync(AppCenterEnabled.IsChecked.Value).Wait();
            }
        }

        private void CountryCodeEnableCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleCountryCodeEnabled();
        }

        private void CountryCodeEnableCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            HandleCountryCodeEnabled();
        }

        private void HandleCountryCodeEnabled()
        {
            if (!CountryCodeEnableCheckbox.IsChecked.HasValue)
            {
                return;
            }
            if (!CountryCodeEnableCheckbox.IsChecked.Value)
            {
                CountryCodeText.Text = "";
                SaveCountryCode();
            }
            else
            {
                if (string.IsNullOrEmpty(localSettings.Values[Constants.KeyCountryCode] as string))
                {
                    CountryCodeText.Text = RegionInfo.CurrentRegion.TwoLetterISORegionName;
                }
                else
                {
                    CountryCodeText.Text = localSettings.Values[Constants.KeyCountryCode] as string;
                }
            }
        }

        private void SaveCountryCodeButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCountryCode();
        }

        private void SaveCountryCode()
        {
            CountryCodeNotice.Visibility = Visibility.Visible;
            localSettings.Values[Constants.KeyCountryCode] = CountryCodeText.Text;
            AppCenter.SetCountryCode(CountryCodeText.Text.Length > 0 ? CountryCodeText.Text : null);
        }

        private void SaveStorageSize_Click(object sender, RoutedEventArgs e)
        {
            var storageSize = StorageMaxSize.Text;
            var size = 10L * 1024 * 1024;
            long.TryParse(storageSize, out size);
            AppCenter.SetMaxStorageSizeAsync(size);
            localSettings.Values[Constants.KeyStorageMaxSize] = size;
        }

        private void UserId_LostFocus(object sender, RoutedEventArgs e)
        {
            HandleUserIdChange();
        }

        private void UserId_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            HandleUserIdChange();
        }

        private void HandleUserIdChange()
        {
            var userId = UserId.Text;
            var text = string.IsNullOrEmpty(userId) ? null : userId;
            AppCenter.SetUserId(text);
            localSettings.Values[Constants.KeyUserId] = text;
        }
        #endregion

        #region Analytics

        private void AnalyticsEnabled_Checked(object sender, RoutedEventArgs e)
        {
            HandleAnalyticsEnabled();
        }

        private void AnalyticsEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleAnalyticsEnabled();
        }

        private void HandleAnalyticsEnabled()
        {
            AnalyticsEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
            Analytics.SetEnabledAsync(AnalyticsEnabled.IsChecked.Value).Wait();
        }

        private void TrackEvent_Click(object sender, RoutedEventArgs e)
        {
            var name = EventName.Text;
            var propertiesDictionary = EventPropertiesSource.Where(property => property.Key != null && property.Value != null)
                .ToDictionary(property => property.Key, property => property.Value);
            Analytics.TrackEvent(name, propertiesDictionary);
            EventPropertiesSource.Clear();
        }

        private void EventAddProperties_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EventKeyItem.Text) || string.IsNullOrEmpty(EventValueItem.Text))
            {
                return;
            }
            var property = new Property();
            property.Key = EventKeyItem.Text;
            property.Value = EventValueItem.Text;
            EventKeyItem.Text = "";
            EventValueItem.Text = "";
            EventPropertiesSource.Add(property);
        }

        #endregion

        #region Crash

        private void CrashesEnabled_Checked(object sender, RoutedEventArgs e)
        {
            HandleCrashesEnabled();
        }

        private void CrashesEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleCrashesEnabled();
        }

        private void HandleCrashesEnabled()
        {
            CrashesEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
            Crashes.SetEnabledAsync(CrashesEnabled.IsChecked.Value).Wait();
        }

        public class NonSerializableException : Exception
        {
        }

        private void CrashWithTestException_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() => Crashes.GenerateTestCrash());
            ErrorPropertiesSource.Clear();
        }

        private void CrashWithNonSerializableException_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() => throw new NonSerializableException());
            ErrorPropertiesSource.Clear();
        }

        private void CrashWithDivisionByZero_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() => { _ = 42 / int.Parse("0"); });
            ErrorPropertiesSource.Clear();
        }

        private void CrashWithAggregateException_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() => throw GenerateAggregateException());
            ErrorPropertiesSource.Clear();
        }

        private static Exception GenerateAggregateException()
        {
            try
            {
                throw new AggregateException(SendHttp(), new ArgumentException("Invalid parameter", ValidateLength()));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception SendHttp()
        {
            try
            {
                throw new IOException("Network down");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception ValidateLength()
        {
            try
            {
                throw new ArgumentOutOfRangeException(null, "It's over 9000!");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private void CrashWithNullReference_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() =>
            {
                string[] values = { "a", null, "c" };
                var b = values[1].Trim();
                System.Diagnostics.Debug.WriteLine(b);
            });
            ErrorPropertiesSource.Clear();
        }

        private async void CrashInsideAsyncTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FakeService.DoStuffInBackground();
            }
            catch (Exception ex) when (HandleExceptions.IsChecked.Value)
            {
                TrackException(ex);
            }
            ErrorPropertiesSource.Clear();
        }

        private void ClassLibraryException_Click(object sender, RoutedEventArgs e)
        {
            CrashUtils.BackgroundExceptionTask().RunSynchronously();
        }

        private static class FakeService
        {
            public static async Task DoStuffInBackground()
            {
                await Task.Run(() => throw new IOException("Server did not respond"));
            }
        }

        private void TrackException(Exception e)
        {
            var properties = ErrorPropertiesSource.Where(property => property.Key != null && property.Value != null).ToDictionary(property => property.Key, property => property.Value);
            if (properties.Count == 0)
            {
                properties = null;
            }
            Crashes.TrackError(e, properties, App.GetErrorAttachments().ToArray());
            ErrorPropertiesSource.Clear();
        }

        void HandleOrThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e) when (HandleExceptions.IsChecked.Value)
            {
                TrackException(e);
            }
            ErrorPropertiesSource.Clear();
        }

        private void TextAttachmentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textAttachments = TextAttachmentTextBox.Text;
            localSettings.Values[Constants.KeyTextErrorAttachments] = textAttachments;
        }

        private async void FileErrorAttachment_Click(object sender, RoutedEventArgs e)
        {
            // Open a text file.
            FileOpenPicker open = new FileOpenPicker();
            open.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add("*");

            // When running on win32, FileOpenPicker needs to know the top-level hwnd via IInitializeWithWindow::Initialize.
            if (Window.Current == null)
            {
                IInitializeWithWindow initializeWithWindowWrapper = open.As<IInitializeWithWindow>();
                IntPtr hwnd = GetActiveWindow();
                initializeWithWindowWrapper.Initialize(hwnd);
            }
            StorageFile file = await open.PickSingleFileAsync();
            var filePath = string.Empty;
            if (file != null)
            {
                filePath = file.Path;
                FileAttachmentLabel.Text = filePath;
            }
            else
            {
                FileAttachmentLabel.Text = "The file isn't selected";
            }
            localSettings.Values[Constants.KeyFileErrorAttachments] = filePath;
        }

        private void CrashesAddNewProperty_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ErrorKeyItem.Text) || string.IsNullOrEmpty(ErrorValueItem.Text))
            {
                return;
            }
            var property = new Property();
            property.Key = ErrorKeyItem.Text;
            property.Value = ErrorValueItem.Text;
            ErrorKeyItem.Text = "";
            ErrorValueItem.Text = "";
            ErrorPropertiesSource.Add(property);
        }

        #endregion

        [ComImport, Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize([In] IntPtr hwnd);
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, PreserveSig = true, SetLastError = false)]
        public static extern IntPtr GetActiveWindow();

    }
}
