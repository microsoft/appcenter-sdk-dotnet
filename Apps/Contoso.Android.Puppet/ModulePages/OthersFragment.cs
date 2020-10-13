// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;

namespace Contoso.Android.Puppet
{
    public class OthersFragment : PageFragment
    {
        private Switch DistributeEnabledSwitch;
        private Switch FirebaseAnalyticsEnabledSwitch;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Others, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Find views.
            DistributeEnabledSwitch = view.FindViewById(Resource.Id.enabled_distribute) as Switch;
            FirebaseAnalyticsEnabledSwitch = view.FindViewById(Resource.Id.enabled_firebase_analytics) as Switch;

            // Subscribe to events.
            DistributeEnabledSwitch.CheckedChange += UpdateDistributeEnabled;
            FirebaseAnalyticsEnabledSwitch.CheckedChange += UpdateFirebaseAnalyticsEnabled;

            UpdateState();
        }

        protected override async void UpdateState()
        {
            DistributeEnabledSwitch.CheckedChange -= UpdateDistributeEnabled;
            DistributeEnabledSwitch.Enabled = true;
            DistributeEnabledSwitch.Checked = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitch.Enabled = await AppCenter.IsEnabledAsync();
            DistributeEnabledSwitch.CheckedChange += UpdateDistributeEnabled;

            FirebaseAnalyticsEnabledSwitch.CheckedChange -= UpdateFirebaseAnalyticsEnabled;
            var enableAnalytics = Preferences.SharedPreferences.GetBoolean(Constants.FirebaseAnalyticsEnabledKey, false);
            FirebaseAnalyticsEnabledSwitch.Checked = enableAnalytics;
            FirebaseAnalyticsEnabledSwitch.CheckedChange += UpdateFirebaseAnalyticsEnabled;
        }

        private async void UpdateDistributeEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.IsChecked);
            DistributeEnabledSwitch.Checked = await Distribute.IsEnabledAsync();
        }

        private void UpdateFirebaseAnalyticsEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            var editor = Preferences.SharedPreferences.Edit();
            editor.PutBoolean(Constants.FirebaseAnalyticsEnabledKey, e.IsChecked);
            FirebaseAnalyticsEnabledSwitch.Checked = e.IsChecked;
            editor.Apply();
        }
    }
}
