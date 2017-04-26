using System;
using Tizen.System;

using Tizen.Applications;

namespace Microsoft.Azure.Mobile.Utils
{
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        private string _cachedScreenSize;

        //Why is this event handler required?
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

        //TODO
        protected override string GetAppBuild()
        {
            return "Undefined";
        }

        protected override string GetScreenSize()
        {
            return _cachedScreenSize;
        }
    }
}
