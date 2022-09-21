// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AppCenter;

namespace Contoso.MAUI.Demo;

public partial class AppCenterContentPage : ContentPage
{
    public AppCenterContentPage()
    {
        InitializeComponent();

        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            IconImageSource = "bolt.png";
        }

        UserIdEntry.Text = Preferences.Get(Constants.UserId, string.Empty);

        if (Preferences.ContainsKey(Constants.StorageMaxSize))
        {
            StorageMaxSize.Text = Preferences.Get(Constants.StorageMaxSize, 0).ToString();
        }

        // Setup start type dropdown choices.
        foreach (var startType in StartTypeUtils.GetStartTypeChoiceStrings())
        {
            this.StartTypePicker.Items.Add(startType);
        }
        this.StartTypePicker.SelectedIndex = (int)(StartTypeUtils.GetPersistedStartType());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AppCenterEnabledSwitchCell.IsToggled = await AppCenter.IsEnabledAsync();
        AllowedNetworkRequestSwitchCell.IsToggled = AppCenter.IsNetworkRequestsAllowed;
        UserIdEntry.Unfocused += (sender, args) =>
        {
            var inputText = UserIdEntry.Text;
            var text = string.IsNullOrEmpty(inputText) ? null : inputText;
            AppCenter.SetUserId(text);
            Preferences.Set(Constants.UserId, text);
        };
    }

    async void ChangeStartType(object sender, PropertyChangedEventArgs e)
    {
        // IOS sends an event every time user rests their selection on an item without hitting "done", and the only event they send when hitting "done" is that the control is no longer focused.
        // So we'll process the change at that time. This works for android as well.
        if (e.PropertyName == "IsFocused" && !this.StartTypePicker.IsFocused)
        {
            var newSelectionCandidate = this.StartTypePicker.SelectedIndex;
            var persistedStartType = StartTypeUtils.GetPersistedStartType();
            if (newSelectionCandidate != (int)persistedStartType)
            {
                StartTypeUtils.SetPersistedStartType((StartType)newSelectionCandidate);
                await DisplayAlert("Start Type Changed", "Start type has changed, which alters the app secret. Please close and re-run the app for the new app secret to take effect.", "OK");
            }
        }
    }

    async void UpdateEnabled(object sender, ToggledEventArgs e)
    {
        await AppCenter.SetEnabledAsync(e.Value);
    }

    private void SaveStorageSize_Clicked(object sender, System.EventArgs e)
    {
        var inputText = StorageMaxSize.Text;
        if (long.TryParse(inputText, out var result))
        {
            _ = AppCenter.SetMaxStorageSizeAsync(result);
            Preferences.Set(Constants.StorageMaxSize, result);
        }
        else
        {
            AppCenterLog.Error(App.LogTag, "Wrong number value for the max storage size.");
        }
    }

    void AllowedNetworkRequestEnabled(System.Object sender, ToggledEventArgs e)
    {
        AppCenter.IsNetworkRequestsAllowed = e.Value;
    }
}
