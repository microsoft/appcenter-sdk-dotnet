// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace MAUI.Demo;

public partial class AnalyticsContentPage : ContentPage
{
    List<Property> EventProperties;

    public AnalyticsContentPage()
    {
        InitializeComponent();
        EventProperties = new List<Property>();
        NumPropertiesLabel.Text = EventProperties.Count.ToString();

        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            //Icon = "lightning.png";
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        EnabledSwitchCell.IsToggled = await Analytics.IsEnabledAsync();
        EnabledSwitchCell.IsEnabled = await AppCenter.IsEnabledAsync();
        
        CountryCodeText.Text = Preferences.Get(Constants.CountryCode, String.Empty);
        EnableManualSessionTrackerSwitch.IsToggled = Preferences.Get(Constants.EnableManualSessionTracker, false);
    }

    async void AddProperty(object sender, EventArgs e)
    {
        var addPage = new AddPropertyContentPage();
        addPage.PropertyAdded += (Property property) =>
        {
            if (property.Name == null || EventProperties.Any(i => i.Name == property.Name))
            {
                return;
            }
            EventProperties.Add(property);
            RefreshPropCount();
        };
        await Navigation.PushModalAsync(addPage);
    }

    async void PropertiesCellTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PropertiesContentPage(EventProperties));
    }

    void TrackEvent(object sender, EventArgs e)
    {
        var properties = new Dictionary<string, string>();
        foreach (Property property in EventProperties)
        {
            properties.Add(property.Name, property.Value);
        }

        if (EventProperties.Count == 0)
        {
            Analytics.TrackEvent(EventNameCell.Text);
            return;
        }

        EventProperties.Clear();
        RefreshPropCount();
        Analytics.TrackEvent(EventNameCell.Text, properties);

    }

    async void UpdateEnabled(object sender, ToggledEventArgs e)
    {
        await Analytics.SetEnabledAsync(e.Value);
    }

    void RefreshPropCount()
    {
        NumPropertiesLabel.Text = EventProperties.Count.ToString();
    }

    void EnableManualSessionTrackerCellEnabled(object sender, ToggledEventArgs e)
    {
        Preferences.Set(Constants.EnableManualSessionTracker, e.Value);

        // alternative?
        //_ = Application.Current.SavePropertiesAsync();
    }

    void StartSessionButton_Clicked(object sender, EventArgs e)
    {
        Analytics.StartSession();
    }

    void SaveCountryCode_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(Constants.CountryCode, CountryCodeText.Text);

        // alternative?
        // _ = Application.Current.SavePropertiesAsync();
    }
}
