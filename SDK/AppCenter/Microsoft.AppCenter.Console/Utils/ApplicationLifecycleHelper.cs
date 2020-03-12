// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        // Singleton instance of ApplicationLifecycleHelper
        private static IApplicationLifecycleHelper _instance;
        public static IApplicationLifecycleHelper Instance
        {
            get { return _instance ?? (_instance = new ApplicationLifecycleHelper()); }

            // Setter for testing
            internal set { _instance = value; }
        }

        static ApplicationLifecycleHelper()
        {
          
          //  AppDomain.CurrentDomain.ProcessExit += delegate { UnhookWinEvent(hook); };
        }

        public ApplicationLifecycleHelper()
        {
            Enabled = true;
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs((Exception)eventArgs.ExceptionObject));
            };
        }


        private bool enabled;

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;

        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        public bool IsSuspended => false;
    }
}
