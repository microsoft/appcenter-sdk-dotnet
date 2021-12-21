// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AnalyticsContentPage : ContentPage
    {
        List<Property> EventProperties;

        public AnalyticsContentPage()
        {
            InitializeComponent();
            EventProperties = new List<Property>();
            NumPropertiesLabel.Text = EventProperties.Count.ToString();

            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "lightning.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            EnabledSwitchCell.IsToggled = await Analytics.IsEnabledAsync();
            EnabledSwitchCell.IsEnabled = await AppCenter.IsEnabledAsync();
            if (Application.Current.Properties.ContainsKey(Constants.CountryCode)
                && Application.Current.Properties[Constants.CountryCode] is string countryCode)
            {
                CountryCodeText.Text = countryCode;
            }
            if (Application.Current.Properties.ContainsKey(Constants.EnableManualSessionTracker)
                && Application.Current.Properties[Constants.EnableManualSessionTracker] is bool isEnabled)
            {
                EnableManualSessionTrackerSwitch.IsToggled = isEnabled;
            }
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
            Application.Current.Properties[Constants.EnableManualSessionTracker] = e.Value;
            _ = Application.Current.SavePropertiesAsync();
        }

        void StartSessionButton_Clicked(object sender, EventArgs e)
        {
            Analytics.StartSession();
        }

        void SaveCountryCode_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties[Constants.CountryCode] = CountryCodeText.Text;
            _ = Application.Current.SavePropertiesAsync();
        }
    }
}
