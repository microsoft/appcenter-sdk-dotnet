// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Runtime;
using Java.Lang;
using Java.Util;

namespace Microsoft.AppCenter.Crashes
{
    using AndroidErrorReport = Android.Model.ErrorReport;
    using ModelException = Android.Ingestion.Models.Exception;
    using ModelStackFrame = Android.Ingestion.Models.StackFrame;
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
        public static Type BindingType => typeof(Android.Crashes);

        static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
            int androidUserConfirmation;
            switch (confirmation)
            {
                case UserConfirmation.Send:
                    androidUserConfirmation = Android.Crashes.Send;
                    break;
                case UserConfirmation.DontSend:
                    androidUserConfirmation = Android.Crashes.DontSend;
                    break;
                case UserConfirmation.AlwaysSend:
                    androidUserConfirmation = Android.Crashes.AlwaysSend;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmation), confirmation, null);
            }
            Android.Crashes.NotifyUserConfirmation(androidUserConfirmation);
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            var future = Android.Crashes.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = Android.Crashes.SetEnabled(enabled);
            return Task.Run(() => future.Get());
        }

        static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            var future = Android.Crashes.HasCrashedInLastSession();
            return Task.Run(() => (bool)future.Get());
        }

        static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            var future = Android.Crashes.LastSessionCrashReport;
            return Task.Run(() =>
            {
                var androidErrorReport = future.Get() as AndroidErrorReport;
                if (androidErrorReport == null)
                    return null;
                return ErrorReportCache.GetErrorReport(androidErrorReport);
            });
        }

        static Task<bool> PlatformHasReceivedMemoryWarningInLastSessionAsync()
        {
            var future = Android.Crashes.HasReceivedMemoryWarningInLastSession();
            return Task.Run(() => (bool)future.Get());
        }

        static void PlatformTrackError(Exception exception, IDictionary<string, string> properties, ErrorAttachmentLog[] attachments)
        {
            ArrayList attachmentArray = null;
            if (attachments != null)
            {
                attachmentArray = new ArrayList();
                foreach (var attachment in attachments)
                {
                    if (attachment?.InternalAttachment != null)
                    {
                        attachmentArray.Add(attachment.InternalAttachment);
                    }
                    else
                    {
                        AppCenterLog.Warn(LogTag, "Skipping null ErrorAttachmentLog in Crashes.TrackError.");
                    }
                }
            }
            Android.WrapperSdkExceptionManager.TrackException(GenerateModelException(exception, false), properties, attachmentArray);
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
            Android.Crashes.SetListener(new AndroidCrashListener());
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
                var modelException = GenerateModelException(exception, true);
                string rawExceptionString = javaThrowable == null ? exception.ToString() : null;
                Android.WrapperSdkExceptionManager.SaveWrapperException(Thread.CurrentThread(), javaThrowable, modelException, rawExceptionString);
                _exception = exception;
            }
        }

        // Generate structured data for a dotnet exception.
        static ModelException GenerateModelException(Exception exception, bool structuredFrames)
        {
            var modelException = new ModelException
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Frames = structuredFrames ? GenerateModelStackFrames(new StackTrace(exception, true)) : null,
                WrapperSdkName = WrapperSdk.Name
            };
            var aggregateException = exception as AggregateException;
            if (aggregateException?.InnerExceptions != null)
            {
                modelException.InnerExceptions = new List<ModelException>();
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    modelException.InnerExceptions.Add(GenerateModelException(innerException, structuredFrames));
                }
            }
            else if (exception.InnerException != null)
            {
                modelException.InnerExceptions = new List<ModelException>
                {
                    GenerateModelException(exception.InnerException, structuredFrames)
                };
            }
            return modelException;
        }

        static IList<ModelStackFrame> GenerateModelStackFrames(StackTrace stackTrace)
        {
            var modelFrames = new List<ModelStackFrame>();
            var frames = stackTrace.GetFrames();
            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    var modelFrame = new ModelStackFrame
                    {
                        ClassName = frame.GetMethod()?.DeclaringType?.FullName,
                        MethodName = frame.GetMethod()?.Name,
                        FileName = frame.GetFileName(),
                        LineNumber = frame.GetFileLineNumber() != 0 ? new Integer(frame.GetFileLineNumber()) : null
                    };
                    if (!modelFrame.Equals(EmptyModelFrame))
                    {
                        modelFrames.Add(modelFrame);
                    }
                }
            }
            return modelFrames;
        }


        /* Bridge between C# events/callbacks and Java listeners. */
        class AndroidCrashListener : Java.Lang.Object, Android.ICrashesListener
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
                        attachmentList.Add(attachment?.InternalAttachment);
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

        private static void PlatformUnsetInstance()
        {
            Android.Crashes.UnsetInstance();
        }
    }
}
