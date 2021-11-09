// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class MainPuppetPage : TabbedPage
    {
        public MainPuppetPage ()
        {
            InitializeComponent();
            if ((Device.RuntimePlatform == Device.macOS || Device.RuntimePlatform == Device.UWP)
                && Children.Contains(DistributeContentPage))
            {
                Children.Remove(DistributeContentPage);
            }
        }
    }
}
