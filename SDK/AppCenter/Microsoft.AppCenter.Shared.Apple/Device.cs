// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if __IOS__
using Microsoft.AppCenter.iOS.Bindings;
#elif __MACOS__
using Microsoft.AppCenter.MacOS.Bindings;
#endif

namespace Microsoft.AppCenter
{
    public partial class Device
    {
        public Device(MSACDevice device)
        {
            SdkName = device.SdkName;
            SdkVersion = device.SdkVersion;
            Model = device.Model;
            OemName = device.OemName;
            OsName = device.OsName;
            OsVersion = device.OsVersion;
            OsBuild = device.OsBuild;
            OsApiLevel = device.OsApiLevel == null ? 0 : device.OsApiLevel.Int32Value;
            Locale = device.Locale;
            TimeZoneOffset = device.TimeZoneOffset == null ? 0 : device.TimeZoneOffset.Int32Value;
            ScreenSize = device.ScreenSize;
            AppVersion= device.AppVersion;
            CarrierName = device.CarrierName;
            CarrierCountry = device.CarrierCountry;
            AppBuild = device.AppBuild;
            AppNamespace = device.AppNamespace;
        }
    }
}
