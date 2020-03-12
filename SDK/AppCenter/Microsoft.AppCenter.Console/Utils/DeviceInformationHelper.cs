// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;

namespace Microsoft.AppCenter.Utils
{

    /// <summary>
    /// Implements the abstract device information helper class
    /// </summary>
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        protected override string GetSdkName()
        {
            return "appcenter.uwp"; // TODO: Get info
        }

        protected override string GetDeviceModel()
        {
            return "Compucter"; // TODO: Get info
        }

        protected override string GetAppNamespace()
        {
            return Assembly.GetEntryAssembly()?.EntryPoint.DeclaringType?.Namespace;
        }

        protected override string GetDeviceOemName()
        {
            return "ECM"; // TODO: Get info
        }

        protected override string GetOsName()
        {
            return "WINDOWS"; // TODO: Get info
        }

        protected override string GetOsBuild()
        {
            return "10.0.18363.657"; // TODO: Get info
        }

        protected override string GetOsVersion()
        {
            return "10.0.18363"; // TODO: Get info
        }

        protected override string GetAppVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();            
        }

        protected override string GetAppBuild()
        {
            return GetAppVersion();
        }

        protected override string GetScreenSize()
        {
            return $"100x100"; // TODO: Get info
        }
    }
}
