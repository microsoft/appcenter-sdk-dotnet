// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Contoso.Forms.Puppet.UWP;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppConfiguration))]
namespace Contoso.Forms.Puppet.UWP
{
    class AppConfiguration : IAppConfiguration
    {
        public string GetAppSecret()
        {
            return Environment.GetEnvironmentVariable("XAMARIN_FORMS_UWP_INT");
        }

        public string GetTargetToken()
        {
            return "";
        }
    }
}
