namespace Microsoft.Azure.Mobile
{
    public partial class Device
    {
        public Device(Ingestion.Models.Device deviceModel)
        {
            AppBuild = deviceModel.AppBuild;
            AppNamespace = deviceModel.AppNamespace;
            AppVersion = deviceModel.AppVersion;
            CarrierCountry = deviceModel.CarrierCountry;
            CarrierName = deviceModel.CarrierName;
            Locale = deviceModel.Locale;
            Model = deviceModel.Model;
            OemName = deviceModel.OemName;
            if (deviceModel.OsApiLevel != null)
            {
                OsApiLevel = (int)deviceModel.OsApiLevel;
            }
            OsBuild = deviceModel.OsBuild;
            OsName = deviceModel.OsName;
            OsVersion = deviceModel.OsVersion;
            ScreenSize = deviceModel.ScreenSize;
            SdkName = deviceModel.SdkName;
            SdkVersion = deviceModel.SdkVersion;
            TimeZoneOffset = deviceModel.TimeZoneOffset;
        }
    }
}
