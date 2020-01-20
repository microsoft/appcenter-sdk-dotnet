// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Test.Functional
{
    public static class Config
    {
        public const int ResultChannelPort = 16384;

        public const string AppSecret = "e94aaff4-e80d-4fee-9a5f-a84eb6e688fc";

        public const string DistributionGroupId = "00000000-0000-0000-0000-000000000000";

        public const string RequestId = "b627efb5-dbf7-4350-92e4-b6ac4dbd09b0";

        public const string Package = "com.contoso.test.functional";
        public static string resolveAppsecret()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.iOS:
                    return "fe2bf05d-f4f9-48a6-83d9-ea8033fbb644";
                case Xamarin.Forms.Device.Android:
                    return "987b5941-4fac-4968-933e-98a7ff29237c";
                case Xamarin.Forms.Device.UWP:
                    return "5bce20c8-f00b-49ca-8580-7a49d5705d4c";
                default:
                    return "fe2bf05d-f4f9-48a6-83d9-ea8033fbb644";
            }
        }
    }
}
