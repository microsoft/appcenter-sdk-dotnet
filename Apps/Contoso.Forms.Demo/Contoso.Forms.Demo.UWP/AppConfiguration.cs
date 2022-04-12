// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Contoso.Forms.Demo.UWP;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppConfiguration))]
namespace Contoso.Forms.Demo.UWP
{
    class AppConfiguration: IAppConfiguration
    {
        public string GetAppSecret()
        {
            return Environment.GetEnvironmentVariable("XAMARIN_FORMS_UWP_PROD");
        }

        public string GetTargetToken()
        {
            return "";
        }
    }
}
