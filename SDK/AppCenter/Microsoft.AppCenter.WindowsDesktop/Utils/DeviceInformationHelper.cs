// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#if NET461
using System.Deployment.Application;
#endif

namespace Microsoft.AppCenter.Utils
{

    /// <summary>
    /// Implements the abstract device information helper class
    /// </summary>
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        private IManagmentClassFactory _managmentClassFactory;

        public DeviceInformationHelper()
        {
            _managmentClassFactory = ManagmentClassFactory.GetInstance();
        }

        /// <summary>
        /// Set the specific class factory for the management class.
        /// </summary>
        /// <param name="factory">Specific management class factory.</param>
        internal void SetManagmentClassFactory(IManagmentClassFactory factory)
        {
            _managmentClassFactory = factory;
        }

        protected override string GetSdkName()
        {
            var sdkName = WpfHelper.IsRunningOnWpf ? "appcenter.wpf" : "appcenter.winforms";
#if NETCOREAPP3_0
            sdkName = $"{sdkName}.netcore";
#endif
            return sdkName;
        }

        protected override string GetDeviceModel()
        {
            try
            {
                ManagementClass managementClass = _managmentClassFactory.GetComputerSystemClass();
                foreach (var managementObject in managementClass.GetInstances())
                {
                    var model = (string)managementObject["Model"];
                    return (string.IsNullOrEmpty(model) || DefaultSystemProductName == model ? null : model);
                }
            }
            catch (UnauthorizedAccessException exc)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model with error: ", exc);
                return string.Empty;
            }
            return string.Empty;
        }

        protected override string GetAppNamespace()
        {
            return Assembly.GetEntryAssembly()?.EntryPoint.DeclaringType?.Namespace;
        }

        protected override string GetDeviceOemName()
        {
            try
            {
                ManagementClass managementClass = _managmentClassFactory.GetComputerSystemClass();
                foreach (var managementObject in managementClass.GetInstances())
                {
                    var manufacturer = (string)managementObject["Manufacturer"];
                    return (string.IsNullOrEmpty(manufacturer) || DefaultSystemManufacturer == manufacturer ? null : manufacturer);
                }
            } 
            catch (UnauthorizedAccessException exc)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device oem name with error: ", exc);
                return string.Empty;
            }
            return string.Empty;
        }

        protected override string GetOsName()
        {
            return "WINDOWS";
        }

        protected override string GetOsBuild()
        {
            using (var hklmKey = Win32.Registry.LocalMachine)
            using (var subKey = hklmKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                // CurrentMajorVersionNumber present in registry starting with Windows 10
                var majorVersion = subKey.GetValue("CurrentMajorVersionNumber");
                if (majorVersion != null)
                {
                    var minorVersion = subKey.GetValue("CurrentMinorVersionNumber", "0");
                    var buildNumber = subKey.GetValue("CurrentBuildNumber", "0");
                    var revisionNumber = subKey.GetValue("UBR", "0");
                    return $"{majorVersion}.{minorVersion}.{buildNumber}.{revisionNumber}";
                }
                else
                {
                    // If CurrentMajorVersionNumber not present in registry then use CurrentVersion
                    var version = subKey.GetValue("CurrentVersion", "0.0");
                    var buildNumber = subKey.GetValue("CurrentBuild", "0");
                    var buildLabEx = subKey.GetValue("BuildLabEx")?.ToString().Split('.');
                    var revisionNumber = buildLabEx?.Length >= 2 ? buildLabEx[1] : "0";
                    return $"{version}.{buildNumber}.{revisionNumber}";
                }
            }
        }

        protected override string GetOsVersion()
        {
            try
            {
                ManagementClass managementClass = _managmentClassFactory.GetOperatingSystemClass();
                foreach (var managementObject in managementClass.GetInstances())
                {
                    return (string)managementObject["Version"];
                }
            }
            catch (UnauthorizedAccessException exc)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device os version with error: ", exc);
                return string.Empty;
            }
            return string.Empty;
        }

        protected override string GetAppVersion()
        {
            /*
             * Application.ProductVersion returns the value from AssemblyInformationalVersion.
             * If the AssemblyInformationalVersion is not applied to an assembly,
             * the version number specified by the AssemblyFileVersion attribute is used instead.
             */
            return DeploymentVersion ?? Application.ProductVersion;
        }

        protected override string GetAppBuild()
        {
            return DeploymentVersion ?? FileVersion;
        }

        protected override string GetScreenSize()
        {
            const int DESKTOPVERTRES = 117;
            const int DESKTOPHORZRES = 118;
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                var desktop = graphics.GetHdc();
                var height = GetDeviceCaps(desktop, DESKTOPVERTRES);
                var width = GetDeviceCaps(desktop, DESKTOPHORZRES);
                return $"{width}x{height}";
            }
        }

        private static string DeploymentVersion
        {
            get
            {
#if NET461
                // Get ClickOnce version (does not exist on .NET Core). 
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
#endif
                return null;
            }
        }

        private static string FileVersion
        {
            get
            {
                // The AssemblyFileVersion uniquely identifies a build.
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
                    return fileVersionInfo.FileVersion;
                }

                // Fallback if entry assembly is not found (in unit tests for example).
                return Application.ProductVersion;
            }
        }

        /// <summary>
        /// Import GetDeviceCaps function to retreive scale-independent screen size.
        /// </summary>
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
    }
}
