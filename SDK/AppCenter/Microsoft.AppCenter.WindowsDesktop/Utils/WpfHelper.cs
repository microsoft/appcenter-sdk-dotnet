// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.AppCenter.Utils
{
    public static class WpfHelper
    {
        static WpfHelper()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                PresentationFramework =
                    assemblies.FirstOrDefault(assembly => assembly.GetName().Name == "PresentationFramework");
                IsRunningOnWpf = PresentationFramework != null;
#if WINDOWS10_0_17763_0
                IsRunningAsUwp = (new DesktopBridge.Helpers()).IsRunningAsUwp();
#endif
            }
            catch (AppDomainUnloadedException)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Unabled to determine whether this application is WPF or Windows Forms; proceeding as though it is Windows Forms.");
            }
        }

        public static bool IsRunningOnWpf { get; }

        public static bool IsRunningAsUwp { get; } = false;

        public static Assembly PresentationFramework { get; }
    }
}
