// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Contoso.Forms.Demo.MacOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppConfiguration))]
namespace Contoso.Forms.Demo.MacOS
{
    public class AppConfiguration: IAppConfiguration
    {
        public AppConfiguration()
        {
        }

        public string GetAppSecret()
        {
            return Environment.GetEnvironmentVariable("XAMARIN_FORMS_MACOS_PROD");
        }

        public string GetTargetToken()
        {
            return "";
        }
    }
}
