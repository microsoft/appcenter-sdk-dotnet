// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;

namespace MAUI.Demo;

public partial class OthersContentPage
{
    static bool _eventFilterStarted;

    static OthersContentPage()
    {

    }

    public OthersContentPage()
    {
        InitializeComponent();
        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            //Icon = "handbag.png";
        }

        // Setup track update dropdown choices.
        foreach (var trackUpdateType in TrackUpdateUtils.GetUpdateTrackChoiceStrings())
        {
            this.UpdateTrackPicker.Items.Add(trackUpdateType);
        }
        UpdateTrackPicker.SelectedIndex = TrackUpdateUtils.ToPickerUpdateTrackIndex(TrackUpdateUtils.GetPersistedUpdateTrack() ?? UpdateTrack.Public);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var acEnabled = await AppCenter.IsEnabledAsync();
        RefreshDistributeEnabled(acEnabled);
        EventFilterEnabledSwitchCell.On = _eventFilterStarted && await EventFilterHolder.Implementation?.IsEnabledAsync();
        EventFilterEnabledSwitchCell.IsEnabled = acEnabled && EventFilterHolder.Implementation != null;
    }

    async void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
    {
        await Distribute.SetEnabledAsync(e.Value);
        var acEnabled = await AppCenter.IsEnabledAsync();
        RefreshDistributeEnabled(acEnabled);
    }

    async void ChangeUpdateTrack(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsFocused" && !UpdateTrackPicker.IsFocused)
        {
            var newSelectionCandidate = UpdateTrackPicker.SelectedIndex;
            var persistedStartType = TrackUpdateUtils.ToPickerUpdateTrackIndex(TrackUpdateUtils.GetPersistedUpdateTrack() ?? UpdateTrack.Public);
            if (newSelectionCandidate != persistedStartType)
            {
                var newTrackUpdateValue = TrackUpdateUtils.FromPickerUpdateTrackIndex(newSelectionCandidate);
                TrackUpdateUtils.SetPersistedUpdateTrack(newTrackUpdateValue);
                await DisplayAlert("Update Track Changed", "Please kill and restart the app for the new update track to take effect.", "OK");
            }
        }
    }

    async void RefreshDistributeEnabled(bool _appCenterEnabled)
    {
        DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
        DistributeEnabledSwitchCell.IsEnabled = _appCenterEnabled;
        RefreshDistributeTrackUpdate();
        RefreshAutomaticUpdateCheck(_appCenterEnabled);
    }

    void RefreshDistributeTrackUpdate()
    {
        UpdateTrackPicker.SelectedIndex = TrackUpdateUtils.ToPickerUpdateTrackIndex(TrackUpdateUtils.GetPersistedUpdateTrack() ?? UpdateTrack.Public);
    }

    async void UpdateEventFilterEnabled(object sender, ToggledEventArgs e)
    {
        if (EventFilterHolder.Implementation != null)
        {
            if (!_eventFilterStarted)
            {
                AppCenter.Start(EventFilterHolder.Implementation.BindingType);
                _eventFilterStarted = true;
            }
            await EventFilterHolder.Implementation.SetEnabledAsync(e.Value);
        }
    }
    
    async void OnAutomaticUpdateCheckChanged(object sender, ToggledEventArgs e)
    {
        bool? previousValue = Preferences.ContainsKey(Constants.AutomaticUpdateCheckKey) ? Preferences.Get(Constants.AutomaticUpdateCheckKey, true) : null;
        Preferences.Set(Constants.AutomaticUpdateCheckKey, e.Value);

        if (previousValue.HasValue && previousValue.Value != e.Value)
        {
            await DisplayAlert("Automatic Update Check Changed", "Please kill and restart the app for the new automatic update check setting to take effect.", "OK");
        }
    }
    
    void RefreshAutomaticUpdateCheck(bool _enabled)
    {
        AutomaticUpdateCheckSwitchCell.IsEnabled = _enabled;
        if (Preferences.ContainsKey(Constants.AutomaticUpdateCheckKey))
        {
            AutomaticUpdateCheckSwitchCell.On = Preferences.Get((Constants.AutomaticUpdateCheckKey), true);
            return;
        }
        AutomaticUpdateCheckSwitchCell.On = true;
    }

    void CheckForUpdateClicked(object sender, EventArgs e)
    {
        Distribute.CheckForUpdate();
    }
}