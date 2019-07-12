// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Android.Runtime;
using Com.Microsoft.Appcenter.Crashes;
using Com.Microsoft.Appcenter.Crashes.Model;
using Java.Lang;
using Java.Util;

namespace Microsoft.AppCenter.Crashes
{
#pragma warning disable XA0001 // Find issues with Android API usage, the API level check does not work in libs.
    using ModelException = Com.Microsoft.Appcenter.Crashes.Ingestion.Models.Exception;
    using ModelStackFrame = Com.Microsoft.Appcenter.Crashes.Ingestion.Models.StackFrame;
    using Exception = System.Exception;

    public partial class Crashes
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The Android SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidCrashes);

        static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
            int androidUserConfirmation;
            switch (confirmation)
            {
                case UserConfirmation.Send:
                    androidUserConfirmation = AndroidCrashes.Send;
                    break;
                case UserConfirmation.DontSend:
                    androidUserConfirmation = AndroidCrashes.DontSend;
                    break;
                case UserConfirmation.AlwaysSend:
                    androidUserConfirmation = AndroidCrashes.AlwaysSend;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmation), confirmation, null);
            }
            AndroidCrashes.NotifyUserConfirmation(androidUserConfirmation);
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            var future = AndroidCrashes.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidCrashes.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            var future = AndroidCrashes.HasCrashedInLastSession();
            return Task.Run(() => (bool)future.Get());
        }

        static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            var future = AndroidCrashes.LastSessionCrashReport;
            return Task.Run(() =>
            {
                var androidErrorReport = future.Get() as AndroidErrorReport;
                if (androidErrorReport == null)
                    return null;
                return ErrorReportCache.GetErrorReport(androidErrorReport);
            });
        }

        static void PlatformTrackError(Exception exception, IDictionary<string, string> properties)
        {
            var stackTrace = GenerateFullStackTrace(exception);
            WrapperSdkExceptionManager.TrackException(GenerateModelException(exception, stackTrace), properties);
        }

        // Empty model stack frame used for comparison to optimize JSON payload.
        static readonly ModelStackFrame EmptyModelFrame = new ModelStackFrame();

        // Exception that was caught, to avoid logging it 2 times if both handlers called.
        static Exception _exception;

        static Crashes()
        {
            // Set up 2 different handlers, some exceptions are caught by one or the other or both (all scenarios are possible).
            // When caught on both, only the first invocation will actually be saved by the native SDK.
            // Android environment is called before app domain in case of both called, and that's what we want as
            // Android environment has a better stack trace object (no JavaProxyThrowable wrapper that cannot be serialized).
            // Client side exception object after restart not possible most of the time with AppDomain when it's JavaProxyThrowable.
            // AndroidEnvironment is also called before Java SDK exception handler (which itself not always called...).
            // App domain fallback is thus used only when both android environment and Java SDK handler cannot catch.
            // From our tests if only app domain is called, then it's not a JavaProxyThrowable, so stack trace looks fine.
            AppCenterLog.Info(LogTag, "Set up Xamarin crash handlers.");
            AndroidEnvironment.UnhandledExceptionRaiser += OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // Set up bridge between Java listener and .NET events/callbacks.
            AndroidCrashes.SetListener(new AndroidCrashListener());
        }

        static void OnUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            OnUnhandledException(e.Exception, "AndroidEnvironment");
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OnUnhandledException(e.ExceptionObject as Exception, "AppDomain");
        }

        static void OnUnhandledException(Exception exception, string source)
        {
            if (_exception == null)
            {
                AppCenterLog.Error(LogTag, $"Unhandled Exception from source={source}", exception);
                var javaThrowable = exception as Throwable;
                var modelException = GenerateModelException(exception);
                byte[] rawException = javaThrowable == null ? CrashesUtils.SerializeException(exception) : null;
                WrapperSdkExceptionManager.SaveWrapperException(Thread.CurrentThread(), javaThrowable, modelException, rawException);
                _exception = exception;
            }
        }

#pragma warning disable XS0001 // Find usages of mono todo items

        // Generate structured data for a dotnet exception.
        static ModelException GenerateModelException(Exception exception, string stackTrace = null)
        {
            stackTrace = stackTrace ?? exception.StackTrace;
            var modelException = new ModelException
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                StackTrace = stackTrace,
                WrapperSdkName = WrapperSdk.Name
            };
            var aggregateException = exception as AggregateException;
            if (aggregateException?.InnerExceptions != null)
            {
                modelException.InnerExceptions = new List<ModelException>();
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    modelException.InnerExceptions.Add(GenerateModelException(innerException));
                }
            }
            else if (exception.InnerException != null)
            {
                modelException.InnerExceptions = new List<ModelException>
                {
                    GenerateModelException(exception.InnerException)
                };
            }
            return modelException;
        }

        // Exceptions don't always have complete stack traces, so they must be augmented.
        private static string GenerateFullStackTrace(Exception e)
        {
            if (string.IsNullOrEmpty(e.StackTrace))
            {
                return e.StackTrace;
            }
            var exceptionStackTrace = new StackTrace(e, true);

            // Generate current stack trace. Skip three frames to avoid showing SDK code.
            var currentStackTrace = new StackTrace(3, true);

            /*
             * The exception's stack trace begins at the first method that threw, and includes only methods that
             * rethrew. The current stack trace includes all methods up to and including the first method that threw,
             * but no methods that rethrew up to the first method that threw. For example:
             * 
             * If method A calls B, B calls C, C calls D, and D throws an exception, and the exception is caught in B,
             * then the stack trace will only include D, C, and B. So A is missing from it. But in the "current" stack
             * trace generated above, we would only see methods B and A. In some cases there could be frames that were
             * created after the exception was thrown but are present now. These frames can be ignored, as they were not
             * part of the flow that involved the exception. For example, we may see exception stack trace "D->C->B" and
             * current stack trace "F->D->A->B->A". The solution is to find the last frame of the exception's stack
             * trace in the current stack trace, append everything after, and ignore everything before. So the result
             * would be "D->C->B->A. Thank you for your time.
             */
            var commonFrame = exceptionStackTrace.GetFrame(exceptionStackTrace.FrameCount - 1);
            var concatenationIndex = -1;
            for (var i = 0; i < currentStackTrace.FrameCount; ++i)
            {
                var otherFrame = currentStackTrace.GetFrame(i);

                // Can't just compare the strings because they may have different line numbers.
                if (otherFrame.GetMethod() == commonFrame.GetMethod())
                {
                    // If the concatenationIndex has already been set, we've found another match. Thus the concatenation
                    // index is ambiguous and cannot be solved.
                    if (concatenationIndex != -1)
                    {
                        concatenationIndex = -1;
                        break;
                    }

                    // Add one to the index to avoid duplicating the common frame.
                    concatenationIndex = i + 1;
                }
            }

            // If the concatenation index could not be determined or is out of range, fall back to the exception's
            // stack trace.
            if (concatenationIndex == -1 || currentStackTrace.FrameCount <= concatenationIndex)
            {
                return e.StackTrace;
            }

            // Compute the missing frames as everything that comes after the common frame. There is no way to convert an
            // array of StackFrame objects to a StackTrace, and the ToString() of StackFrame objects appears to be
            // different from those of StackTrace. Thus, we must work with strings.
            var exceptionStackTraceStrings = exceptionStackTrace.ToString().Split(Environment.NewLine);
            var currentStackTraceString = currentStackTrace.ToString().Split(Environment.NewLine);
            var missingFrames = currentStackTraceString.TakeLast(currentStackTraceString.Length - concatenationIndex);
            var allFrames = exceptionStackTraceStrings.Concat(missingFrames);
            var completeStackTrace = allFrames.Aggregate((result, item) => result + Environment.NewLine + item);
            return completeStackTrace;
        }

#pragma warning restore XS0001 // Find usages of mono todo items


        /* Bridge between C# events/callbacks and Java listeners. */
        class AndroidCrashListener : Java.Lang.Object, ICrashesListener
        {
#pragma warning disable RECS0146 // Member hides static member from outer class
            public IIterable GetErrorAttachments(AndroidErrorReport androidReport)
#pragma warning restore RECS0146 // Member hides static member from outer class
            {
                if (Crashes.GetErrorAttachments == null)
                {
                    return null;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var attachments = Crashes.GetErrorAttachments(report);
                if (attachments != null)
                {
                    var attachmentList = new ArrayList();
                    foreach (var attachment in attachments)
                    {
                        /* Let Java SDK warn against null. */
                        attachmentList.Add(attachment?.internalAttachment);
                    }
                    return attachmentList;
                }
                return null;
            }

            public void OnBeforeSending(AndroidErrorReport androidReport)
            {
                if (SendingErrorReport == null)
                {
                    return;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var e = new SendingErrorReportEventArgs
                {
                    Report = report
                };
                SendingErrorReport(null, e);
            }

            public void OnSendingFailed(AndroidErrorReport androidReport, Java.Lang.Exception exception)
            {
                if (FailedToSendErrorReport == null)
                {
                    return;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var e = new FailedToSendErrorReportEventArgs
                {
                    Report = report,
                    Exception = exception
                };
                FailedToSendErrorReport(null, e);
            }

            public void OnSendingSucceeded(AndroidErrorReport androidReport)
            {
                if (SentErrorReport == null)
                {
                    return;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                var e = new SentErrorReportEventArgs
                {
                    Report = report
                };
                SentErrorReport(null, e);
            }

#pragma warning disable RECS0146 // Member hides static member from outer class
            public bool ShouldAwaitUserConfirmation()
#pragma warning restore RECS0146 // Member hides static member from outer class
            {
                if (Crashes.ShouldAwaitUserConfirmation != null)
                {
                    return Crashes.ShouldAwaitUserConfirmation();
                }
                return false;
            }

            public bool ShouldProcess(AndroidErrorReport androidReport)
            {
                if (ShouldProcessErrorReport == null)
                {
                    return true;
                }
                var report = ErrorReportCache.GetErrorReport(androidReport);
                return ShouldProcessErrorReport(report);
            }
        }
    }
}
