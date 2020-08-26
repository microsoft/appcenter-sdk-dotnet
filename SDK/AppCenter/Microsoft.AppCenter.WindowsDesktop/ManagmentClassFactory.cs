using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Microsoft.AppCenter
{
    interface IManagmentClassFactory
    {
        /// <summary>
        /// Get object of class Win32_ComputerSystem.
        /// </summary>
        /// <returns>Win32_ComputerSystem object.</returns>
        ManagementClass GetComputerSystemClass();

        /// <summary>
        /// Get object of class Win32_OperatingSystem.
        /// </summary>
        /// <returns>Win32_OperatingSystem object.</returns>
        ManagementClass GetOperatingSystemClass();
    }

    class ManagmentClassFactory : IManagmentClassFactory
    {
        // The shared instance of ManagmentClassFactory.
        private static ManagmentClassFactory _instanceField;

        /// <summary>
        /// Gets or sets the shared instance of ManagmentClassFactory. Should never return null.
        /// </summary>
        internal static ManagmentClassFactory Instance 
        {
            get 
            {
                return _instanceField ?? (_instanceField = new ManagmentClassFactory());
            }
        }

        private ManagmentClassFactory()
        {
        }

        public ManagementClass GetComputerSystemClass()
        {
            return new ManagementClass("Win32_ComputerSystem");
        }

        public ManagementClass GetOperatingSystemClass()
        {
            return new ManagementClass("Win32_OperatingSystem");
        }
    }
}
