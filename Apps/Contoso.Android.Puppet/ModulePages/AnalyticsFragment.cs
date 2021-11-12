// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace Contoso.Android.Puppet
{
	using AlertDialog = global::Android.App.AlertDialog;

    public class AnalyticsFragment : PageFragment
    {
        private readonly IDictionary<string, string> mEventProperties = new Dictionary<string, string>();

        private Switch AnalyticsEnabledSwitch;
        private Switch EnableManualSessionTrackerSwitch;
        private EditText EventNameText;
        private TextView PropertiesCountLabel;
        private Button AddPropertyButton;
        private Button TrackEventButton;
        private Button StartSessionButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Analytics, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Find views.
            AnalyticsEnabledSwitch = view.FindViewById(Resource.Id.enabled_analytics) as Switch;
            EnableManualSessionTrackerSwitch = view.FindViewById(Resource.Id.enable_manual_session_tracker_switch) as Switch;
            EventNameText = view.FindViewById(Resource.Id.event_name) as EditText;
            PropertiesCountLabel = view.FindViewById(Resource.Id.properties_count) as TextView;
            AddPropertyButton = view.FindViewById(Resource.Id.add_property) as Button;
            TrackEventButton = view.FindViewById(Resource.Id.track_event) as Button;
            StartSessionButton = view.FindViewById(Resource.Id.start_session_button) as Button;

            // Subscribe to events.
            AnalyticsEnabledSwitch.CheckedChange += UpdateEnabled;
            EnableManualSessionTrackerSwitch.CheckedChange += EnableManualSessionUpdate;
            ((View)PropertiesCountLabel.Parent).Click += Properties;
            AddPropertyButton.Click += AddProperty;
            TrackEventButton.Click += TrackEvent;
            StartSessionButton.Click += StartSessionButton;

            UpdateState();
        }

        protected override async void UpdateState()
        {
            AnalyticsEnabledSwitch.CheckedChange -= UpdateEnabled;
            AnalyticsEnabledSwitch.Enabled = true;
            AnalyticsEnabledSwitch.Checked = await Analytics.IsEnabledAsync();
            AnalyticsEnabledSwitch.Enabled = await AppCenter.IsEnabledAsync();
            AnalyticsEnabledSwitch.CheckedChange += UpdateEnabled;
            EnableManualSessionTrackerSwitch.CheckedChange -= EnableManualSessionUpdate;

            // Set manual session tracker value.
            var prefs = Context.GetSharedPreferences("AppCenter", FileCreationMode.Private);
            EnableManualSessionTrackerSwitch.Checked = prefs.GetBoolean(Constants.EnableManualSessionTrackerKey, false);
            EnableManualSessionTrackerSwitch.CheckedChange += EnableManualSessionUpdate;
            PropertiesCountLabel.Text = mEventProperties.Count.ToString();
        }

        private async void UpdateEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            await Analytics.SetEnabledAsync(e.IsChecked);
            AnalyticsEnabledSwitch.Checked = await Analytics.IsEnabledAsync();
        }

        private void Properties(object sender, EventArgs e)
        {
            var intent = new Intent(Activity, typeof(PropertiesActivity));
            intent.PutExtra("properties", mEventProperties.Select(i => i.Key + ": " + i.Value).ToArray());
            StartActivity(intent);
        }

        private void AddProperty(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetTitle(Resource.String.add_property_dialog_title);
            builder.SetMessage(Resource.String.add_property_dialog_message);
            var layoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                              ViewGroup.LayoutParams.WrapContent);
            var keyText = new EditText(Activity) { LayoutParameters = layoutParameters, Hint = "Property Name" };
            var valueText = new EditText(Activity) { LayoutParameters = layoutParameters, Hint = "Property Value" };
            var view = new LinearLayout(Activity) { Orientation = Orientation.Vertical };
            view.AddView(keyText);
            view.AddView(valueText);
            builder.SetView(view);
            builder.SetPositiveButton(Resource.String.add_property_dialog_add_button, delegate
            {
                mEventProperties.Add(keyText.Text, valueText.Text);
                PropertiesCountLabel.Text = mEventProperties.Count.ToString();
            });
            builder.SetNegativeButton(Resource.String.add_property_dialog_cancel_button, delegate
            {
            });
            builder.Create().Show();
        }

        private void TrackEvent(object sender, EventArgs e)
        {
            Analytics.TrackEvent(EventNameText.Text, mEventProperties.Count > 0 ? mEventProperties : null);
            mEventProperties.Clear();
            PropertiesCountLabel.Text = mEventProperties.Count.ToString();
        }

        private void EnableManualSessionUpdate(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var prefs = Context.GetSharedPreferences("AppCenter", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean(Constants.EnableManualSessionTrackerKey, e.IsChecked);
            prefEditor.Commit();
        }

        private void StartSessionButton(object sender, EventArgs e)
        {
            Analytics.StartSession();
        }
    }
}
