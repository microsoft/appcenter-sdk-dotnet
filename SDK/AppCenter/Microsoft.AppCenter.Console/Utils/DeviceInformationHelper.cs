// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Utils
{

    /// <summary>
    /// Implements the abstract device information helper class
    /// </summary>
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        protected override string GetSdkName()
        {
            return "appcenter.uwp";
        }

        protected override string GetDeviceModel()
        {
            // TODO: Hardcoded - quite a plenty of work required to get this field right on .net core
            return "DeviceModel"; 
        }

        protected override string GetAppNamespace()
        {
            return Assembly.GetEntryAssembly()?.EntryPoint.DeclaringType?.Namespace;
        }

        protected override string GetDeviceOemName()
        {
            // TODO: Hardcoded - quite a plenty of work required to get this field right on .net core
            return "DeviceManufacturer"; 
        }

        protected override string GetOsName()
        {
            
            // TODO: Hardcoded - for backend compatibility
            return "WINDOWS"; 
        }

        protected override string GetOsBuild()
        {
            return Environment.OSVersion.Version.Build.ToString();
        }

        protected override string GetOsVersion()
        {
            return Environment.OSVersion.Version.ToString();
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
            return $"{Console.WindowWidth}x{Console.WindowHeight}";
        }
    }
}
