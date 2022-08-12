// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if WINDOWS10_0_17763_0_OR_GREATER
using System;
using System.Runtime.ExceptionServices;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationLifecycleHelperWinUI: ApplicationLifecycleHelper
    {
        public ApplicationLifecycleHelperWinUI()
        {

            // Subscribe to Resuming and Suspending events.
            CoreApplication.Suspending += delegate { InvokeSuspended(); };

            // If the "LeavingBackground" event is present, use that for Resuming. Else, use CoreApplication.Resuming.
            if (ApiInformation.IsEventPresent(typeof(CoreApplication).FullName, "LeavingBackground"))
            {
                CoreApplication.LeavingBackground += delegate { InvokeResuming(); };

                // If the application has anything visible, then it has already started,
                // so invoke the resuming event immediately.
                InvokeResuming();
            }
            else
            {

                // In versions of Windows 10 where the LeavingBackground event is unavailable, we consider this point to be
                // the start so invoke resuming (and subscribe to future resume events). If InvokeResuming was not called here,
                // the resuming event wouldn't be invoked until the *next* time the application is resumed, which is a problem
                // if the application is not currently suspended. The side effect is that regardless of whether UI is available
                // ever in the process, InvokeResuming will be called at least once (in the case where LeavingBackground isn't
                // available).
                CoreApplication.Resuming += delegate { InvokeResuming(); };
                InvokeResuming();
            }

            // Subscribe to unhandled errors events.
            CoreApplication.UnhandledErrorDetected += (sender, eventArgs) =>
            {
                try
                {

                    // Intentionally propagate exception to get the exception object that crashed the app.
                    eventArgs.UnhandledError.Propagate();
                }
                catch (Exception exception)
                {
                    InvokeUnhandledExceptionOccurred(sender, exception);

                    // Since UnhandledError.Propagate marks the error as Handled, rethrow in order to only Log and not Handle.
                    // Use ExceptionDispatchInfo to avoid changing the stack-trace.
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            };
        }

        internal void InvokeUnhandledExceptionOccurred(object sender, Exception exception)
        {
            base.InvokeUnhandledExceptionOccurred(sender, new UnhandledExceptionOccurredEventArgs(exception));
        }
    }
}
#endif
