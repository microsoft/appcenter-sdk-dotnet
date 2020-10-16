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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Others, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Find views.
            DistributeEnabledSwitch = view.FindViewById(Resource.Id.enabled_distribute) as Switch;

            // Subscribe to events.
            DistributeEnabledSwitch.CheckedChange += UpdateDistributeEnabled;
            UpdateState();
        }

        protected override async void UpdateState()
        {
            DistributeEnabledSwitch.CheckedChange -= UpdateDistributeEnabled;
            DistributeEnabledSwitch.Enabled = true;
            DistributeEnabledSwitch.Checked = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitch.Enabled = await AppCenter.IsEnabledAsync();
            DistributeEnabledSwitch.CheckedChange += UpdateDistributeEnabled;
        }

        private async void UpdateDistributeEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.IsChecked);
            DistributeEnabledSwitch.Checked = await Distribute.IsEnabledAsync();
        }
    }
}
