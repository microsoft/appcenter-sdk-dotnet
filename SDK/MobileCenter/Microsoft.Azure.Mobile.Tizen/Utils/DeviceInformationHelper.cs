using System;
using Tizen.System;

using Tizen.Applications;

namespace Microsoft.Azure.Mobile.Utils
{
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        private string _cachedScreenSize;
        // TODO TIZEN add MobileCenter API to set country code
        private string _country;

        //TODO TIZEN use when updating Device Information like country, screensize
        public static event EventHandler InformationInvalidated;

        public DeviceInformationHelper()
        {
            CacheScreenSize();
        }

        private void CacheScreenSize()
        {
            try
            {
                int height = 0;
                int width = 0;

                SystemInfo.TryGetValue("http://tizen.org/feature/screen.height", out height);
                SystemInfo.TryGetValue("http://tizen.org/feature/screen.width", out width);
                // TODO TIZEN Add event for screen orientation change

                _cachedScreenSize = $"{height}x{width}";
            }
            catch (Exception e)
            {
                _cachedScreenSize = "";
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Failed to retrieve screen size", e);
            }
        }

        protected override string GetSdkName()
        {
            return "mobilecenter.tizen";
        }

        protected override string GetDeviceModel()
        {
            string modelName = "";
            SystemInfo.TryGetValue("http://tizen.org/system/model_name", out modelName);
            return modelName;
        }

        protected override string GetAppNamespace()
        {
            return Tizen.Applications.Application.Current.ApplicationInfo.ApplicationId;
        }

        protected override string GetDeviceOemName()
        {
            string oemName = "";
            SystemInfo.TryGetValue("http://tizen.org/system/manufacturer", out oemName);
            return oemName;
        }

        protected override string GetOsName()
        {
            string osName = "";
            SystemInfo.TryGetValue("http://tizen.org/system/platform.name", out osName);
            return osName;
        }

        protected override string GetOsBuild()
        {
            string osBuild = "";
            SystemInfo.TryGetValue("http://tizen.org/system/build.string", out osBuild);
            return osBuild;
        }

        protected override string GetOsVersion()
        {
            string osVersion = "";
            SystemInfo.TryGetValue("http://tizen.org/feature/platform.version", out osVersion);
            return osVersion;
        }

        protected override string GetAppVersion()
        {
            string packageId = Tizen.Applications.Application.Current.ApplicationInfo.PackageId;
            return PackageManager.GetPackage(packageId).Version;
        }

        protected override string GetAppBuild()
        {
            string packageId = Tizen.Applications.Application.Current.ApplicationInfo.PackageId;
            return PackageManager.GetPackage(packageId).Version;
        }

        protected override string GetScreenSize()
        {
            return _cachedScreenSize;
        }

        // TODO TIZEN Confirm distinction between Carrier and Provider

        // TODO TIZEN Retrieve carrier name from Tizen.Telephony
        //      after C# API for telephony_network_get_network_name() is available
        // Also telephony_sim_get_spn() -- Service Provider Name
        // telephony_sim_get_operator() -- SIM Operator (MCC + MNC)
        // telephony_network_get_mcc() -- Get MCC
        // telephony_network_get_mnc() -- Get MNC
        protected override string GetCarrierName()
        {
            return null;
        }

        // TODO TIZEN Retrieve carrier name from Tizen.Telephony
        //      after C# API for  telephony_network_get_mcc() is available
        // Retrieve carrier country code and convert to country.
        // Make API available in MobileCenter. Invalidate cache if user sets it.
        protected override string GetCarrierCountry()
        {
            return null;
        }
    }
}
