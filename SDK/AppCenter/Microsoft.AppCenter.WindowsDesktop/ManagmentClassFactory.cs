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
        private static ManagmentClassFactory Instance;

        internal ManagmentClassFactory()
        {
        }

        public static ManagmentClassFactory GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ManagmentClassFactory();
            }
            return Instance;
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
