using System;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AppCenterContentPage : ContentPage
    {

        Fabulous.CustomControls.CustomEntryCell customEntryCell;
        public AppCenterContentPage()
        {
            InitializeComponent();
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "bolt.png";
            }
            customEntryCell = new Fabulous.CustomControls.CustomEntryCell();
            customEntryCell.HorizontalTextAlignment = TextAlignment.End;
            customEntryCell.TextChanged += UserIdCompleted;
            customEntryCell.Label = "User Id";
            UserIdTableSection.Add(customEntryCell);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            AppCenterEnabledSwitchCell.On = await AppCenter.IsEnabledAsync();
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.Value);
        }

        private void UserIdCompleted(object sender, EventArgs e)
        {
            var text = string.IsNullOrEmpty(customEntryCell.Text) ? null : customEntryCell.Text;
            AppCenter.SetUserId(text);
        }
    }
}
