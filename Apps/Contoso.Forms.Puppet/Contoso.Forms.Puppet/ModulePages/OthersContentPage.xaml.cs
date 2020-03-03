// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    using XamarinDevice = Xamarin.Forms.Device;

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class OthersContentPage
    {
        static bool _eventFilterStarted;

        static OthersContentPage()
        {

        }

        public OthersContentPage()
        {
            InitializeComponent();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.iOS)
            {
                Icon = "handbag.png";
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
            RefreshPushEnabled(acEnabled);
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
                    await TrackUpdateUtils.SetPersistedUpdateTrackAsync(newTrackUpdateValue);
                    await DisplayAlert("Update Track Changed", "Please kill and restart the app for the new update track to take effect.", "OK");
                }
            }
        }

        async void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
            await Push.SetEnabledAsync(e.Value);
            var acEnabled = await AppCenter.IsEnabledAsync();
            RefreshPushEnabled(acEnabled);
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

        async void RefreshPushEnabled(bool _appCenterEnabled)
        {
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
            PushEnabledSwitchCell.IsEnabled = _appCenterEnabled;
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
            var previousValue = Application.Current.Properties.ContainsKey(Constants.AutomaticUpdateCheckKey) ? Application.Current.Properties[Constants.AutomaticUpdateCheckKey] : null;
            Application.Current.Properties[Constants.AutomaticUpdateCheckKey] = e.Value;
            await Application.Current.SavePropertiesAsync();
            if (previousValue != null && (bool)previousValue != e.Value)
            {
                await DisplayAlert("Automatic Update Check Changed", "Please kill and restart the app for the new automatic update check setting to take effect.", "OK");
            }
        }
        
        void RefreshAutomaticUpdateCheck(bool _enabled)
        {
            AutomaticUpdateCheckSwitchCell.IsEnabled = _enabled;
            if (Application.Current.Properties.TryGetValue(Constants.AutomaticUpdateCheckKey, out object persistedObject))
            {
                AutomaticUpdateCheckSwitchCell.On = (bool)persistedObject;
                return;
            }
            AutomaticUpdateCheckSwitchCell.On = true;
        }

        void CheckForUpdateClicked(object sender, EventArgs e)
        {
            Distribute.CheckForUpdate();
        }
    }
}