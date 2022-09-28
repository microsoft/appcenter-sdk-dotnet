// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Windows.Forms;

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationLifecycleHelperDesktop : ApplicationLifecycleHelper
    {
        // Need to ensure delegate is not collected while we're using it,
        // storing it in a class field is simplest way to do this.
        private static WinEventDelegate _OnMinimizedDelegate = new WinEventDelegate(OnMinimized);
        private static void OnMinimized(IntPtr winEventHookHandle, uint eventType, IntPtr windowHandle, int objectId, int childId, uint eventThreadId, uint eventTimeInMilliseconds)
        {
            // Filter out non-HWND
            if (objectId != 0 || childId != 0)
            {
                return;
            }

            if (WindowsHelper.IsAnyWindowNotMinimized())
            {
                InvokeResuming();
            }
            else
            {
                InvokeSuspended();
            }
        }

        static ApplicationLifecycleHelperDesktop()
        {
            // The change of the state of the flag in this place occurs at the start of the app
            // The `OnMinimized` method does not handle the first entry into the app,
            // so it must happen after initialization
            _suspended = false;

            WindowsHelper.OnMinimized += _OnMinimizedDelegate;
            Application.ApplicationExit += delegate { WindowsHelper.OnMinimized -= _OnMinimizedDelegate; };
        }

        public ApplicationLifecycleHelperDesktop()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
               InvokeUnhandledExceptionOccurred(sender, new UnhandledExceptionOccurredEventArgs((Exception)eventArgs.ExceptionObject));
            };
        }
    }
}
