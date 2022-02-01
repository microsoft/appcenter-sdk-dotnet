// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using System;

namespace Microsoft.AppCenter
{
    public abstract class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        private static IApplicationLifecycleHelper _instance = null;

        // Considered to be suspended until can verify that the application has started.
        protected static bool _suspended = true;

        /// <summary>
        /// Indicates whether the application is currently in a suspended state. 
        /// </summary>
        public bool IsSuspended => _suspended;

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler ApplicationStarted;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;

        public static IApplicationLifecycleHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (WpfHelper.IsRunningAsUwp)
                    {
                        _instance = new ApplicationLifecycleHelperWinUI();
                        AppCenterLog.Debug(AppCenterLog.LogTag, "Use lifecycle for WinUI applications.");
                    }
                    else
                    {
                        _instance = new ApplicationLifecycleHelperDesktop();
                        AppCenterLog.Debug(AppCenterLog.LogTag, "Use lifecycle for desktop applications.");
                    }
                }
                return _instance;
            }

            // Setter for using in tests.
            internal set { _instance = value; }
        }

        protected void InvokeResuming(object sender, EventArgs args)
        {
            ApplicationResuming?.Invoke(sender, args);
        }

        protected void InvokeStarted(object sender, EventArgs args)
        {
            ApplicationStarted?.Invoke(sender, args);
        }

        protected void InvokeSuspended(object sender, EventArgs args)
        {
            ApplicationSuspended?.Invoke(sender, args);
        }

        protected void InvokeUnhandledExceptionOccurred(object sender, UnhandledExceptionOccurredEventArgs args)
        {
            UnhandledExceptionOccurred?.Invoke(sender, args);
        }
    }
}

