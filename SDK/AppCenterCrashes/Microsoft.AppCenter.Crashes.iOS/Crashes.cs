// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Crashes.iOS.Bindings;

namespace Microsoft.AppCenter.Crashes
{
    public partial class Crashes
    {
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Crashes bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(MSCrashes);

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSCrashes.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSCrashes.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            return Task.FromResult(MSCrashes.HasCrashedInLastSession);
        }

        static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            return Task.Run(() =>
            {
                var msReport = MSCrashes.LastSessionCrashReport;
                return (msReport == null) ? null : new ErrorReport(msReport);
            });
        }

        static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
            MSUserConfirmation iosUserConfirmation;
            switch (confirmation)
            {
                case UserConfirmation.Send:
                    iosUserConfirmation = MSUserConfirmation.Send;
                    break;
                case UserConfirmation.DontSend:
                    iosUserConfirmation = MSUserConfirmation.DontSend;
                    break;
                case UserConfirmation.AlwaysSend:
                    iosUserConfirmation = MSUserConfirmation.Always;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmation), confirmation, null);
            }
            MSCrashes.NotifyWithUserConfirmation(iosUserConfirmation);
        }

        static void PlatformTrackError(Exception exception, IDictionary<string, string> properties)
        {
            var stackTrace = GenerateFullStackTrace(exception);
            var modelException = GenerateiOSException(exception, stackTrace);
            if (properties != null)
            {
                MSCrashes.TrackModelException(modelException, StringDictToNSDict(properties));
                return;
            }
            MSCrashes.TrackModelException(modelException);
        }

        /// <summary>
        /// We keep the reference to avoid it being freed, inlining this object will cause listeners not to be called.
        /// </summary>
        static readonly CrashesInitializationDelegate _crashesInitializationDelegate = new CrashesInitializationDelegate();

        /// <summary>
        /// We keep the reference to avoid it being freed, inlining this object will cause listeners not to be called.
        /// </summary>
        static readonly CrashesDelegate _crashesDelegate = new CrashesDelegate();

        static Crashes()
        {
            /* Perform custom setup around the native SDK's for setting signal handlers */
            MSCrashes.DisableMachExceptionHandler();
            MSWrapperCrashesHelper.SetCrashHandlerSetupDelegate(_crashesInitializationDelegate);
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            MSCrashes.SetUserConfirmationHandler((reports) =>
                    {
                        if (ShouldAwaitUserConfirmation != null)
                        {
                            return ShouldAwaitUserConfirmation();
                        }
                        return false;
                    });
            MSCrashes.SetDelegate(_crashesDelegate);
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception systemException = e.ExceptionObject as Exception;
            AppCenterLog.Error(LogTag, "Unhandled Exception:", systemException);
            MSException exception = GenerateiOSException(systemException);
            byte[] exceptionBytes = CrashesUtils.SerializeException(systemException) ?? new byte[0];
            NSData wrapperExceptionData = NSData.FromArray(exceptionBytes);
            MSWrapperException wrapperException = new MSWrapperException
            {
                Exception = exception,
                ExceptionData = wrapperExceptionData,
                ProcessId = new NSNumber(Process.GetCurrentProcess().Id)
            };
            AppCenterLog.Info(LogTag, "Saving wrapper exception...");
            MSWrapperExceptionManager.SaveWrapperException(wrapperException);
            AppCenterLog.Info(LogTag, "Saved wrapper exception.");
        }

        static MSException GenerateiOSException(Exception exception, string stackTrace = null)
        {
            var msException = new MSException();
            msException.Type = exception.GetType().FullName;
            msException.Message = exception.Message ?? "";
            msException.StackTrace = stackTrace ?? exception.StackTrace;
            msException.WrapperSdkName = WrapperSdk.Name;

            var aggregateException = exception as AggregateException;
            var innerExceptions = new List<MSException>();

            if (aggregateException?.InnerExceptions != null)
            {
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    innerExceptions.Add(GenerateiOSException(innerException));
                }
            }
            else if (exception.InnerException != null)
            {
                innerExceptions.Add(GenerateiOSException(exception.InnerException));
            }

            msException.InnerExceptions = innerExceptions.Count > 0 ? innerExceptions.ToArray() : null;

            return msException;
        }

        static string AnonymizePath(string path)
        {
            if ((path == null) || (path.Count() == 0) || !path.Contains("/Users/"))
            {
                return path;
            }

            string pattern = "(/Users/[^/]+/)";
            return Regex.Replace(path, pattern, "/Users/USER/");
        }

        // Bridge between .NET events/callbacks and Apple native SDK
        class CrashesDelegate : MSCrashesDelegate
        {
            public override bool CrashesShouldProcessErrorReport(MSCrashes crashes, MSErrorReport msReport)
            {
                if (ShouldProcessErrorReport == null)
                {
                    return true;
                }
                var report = new ErrorReport(msReport);
                return ShouldProcessErrorReport(report);
            }

            public override NSArray AttachmentsWithCrashes(MSCrashes crashes, MSErrorReport msReport)
            {
                if (GetErrorAttachments == null)
                {
                    return null;
                }
                var report = new ErrorReport(msReport);
                var attachments = GetErrorAttachments(report);
                if (attachments != null)
                {
                    var nsArray = new NSMutableArray();
                    foreach (var attachment in attachments)
                    {
                        if (attachment != null)
                        {
                            nsArray.Add(attachment.internalAttachment);
                        }
                        else
                        {
                            AppCenterLog.Warn(LogTag, "Skipping null ErrorAttachmentLog in Crashes.GetErrorAttachments.");
                        }
                    }
                    return nsArray;
                }
                return null;
            }

            public override void CrashesWillSendErrorReport(MSCrashes crashes, MSErrorReport msReport)
            {
                if (SendingErrorReport != null)
                {
                    var report = new ErrorReport(msReport);
                    var e = new SendingErrorReportEventArgs
                    {
                        Report = report
                    };
                    SendingErrorReport(null, e);
                }
            }

            public override void CrashesDidSucceedSendingErrorReport(MSCrashes crashes, MSErrorReport msReport)
            {
                if (SentErrorReport != null)
                {
                    var report = new ErrorReport(msReport);
                    var e = new SentErrorReportEventArgs
                    {
                        Report = report
                    };
                    SentErrorReport(null, e);
                }

            }

            public override void CrashesDidFailSendingErrorReport(MSCrashes crashes, MSErrorReport msReport, NSError error)
            {
                if (FailedToSendErrorReport != null)
                {
                    var report = new ErrorReport(msReport);
                    var e = new FailedToSendErrorReportEventArgs
                    {
                        Report = report,
                        Exception = error
                    };
                    FailedToSendErrorReport(null, e);
                }
            }
        }

        private static NSDictionary StringDictToNSDict(IDictionary<string, string> dict)
        {
            return NSDictionary.FromObjectsAndKeys(dict.Values.ToArray(), dict.Keys.ToArray());
        }

        // Exceptions don't always have complete stack traces, so they must be augmented.
        // Crashes don't need trace augmenting.
        private static string GenerateFullStackTrace(Exception e)
        {
            if (string.IsNullOrEmpty(e.StackTrace))
            {
                return e.StackTrace;
            }
            var exceptionStackTrace = new StackTrace(e, true);

            // Generate current stack trace. Skip three frames to avoid showing SDK code -
            // `GenerateFullStackTrace`, `PlatformTrackError`, and `TrackError`.
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
    }
}
