using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Crashes.Ingestion.Models;
using Microsoft.Azure.Mobile.Crashes.Utils;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class Crashes : MobileCenterService
    {
        #region static
        // TODO TIZEN Look into Crashes LogTag
        internal new static string LogTag = "MobileCenterCrashes";

        internal static string PREF_KEY_ALWAYS_SEND = Constants.KeyPrefix + "Crashes" + "_ALWAYS_SEND";

        private static readonly object CrashesLock = new object();

        private static Crashes _instanceField;

        internal static CountdownEvent _countDownLatch = null;

        private static readonly IApplicationSettings _applicationSettings = new ApplicationSettings();

        internal static ShouldAwaitUserConfirmationCallback _ShouldAwaitUserConfirmation = null;

        internal static ShouldProcessErrorReportCallback _ShouldProcessErrorReport = null;

        internal static GetErrorAttachmentsCallback _GetErrorAttachments = null;

        public static Crashes Instance
        {
            get
            {
                lock (CrashesLock)
                {
                    return _instanceField ?? (_instanceField = new Crashes());
                }
            }
            set
            {
                lock (CrashesLock)
                {
                    _instanceField = value; //for testing
                }
            }
        }

        private static IDictionary<Guid, ErrorReport> _errorReportCache;

        private static IDictionary<Guid, Tuple<ManagedErrorLog, ErrorReport>> _UnProcessedErrorReports;

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TIZEN Handle crash
            // 1) Generate Error log from Exception
            // 2) Store to file
            // 3) SuhtDown process

            var exception = (Exception)e.ExceptionObject;
            var errorLog = new ManagedErrorLog(0, null, e.IsTerminating, exception, Instance.initializeTimeStamp, Guid.NewGuid());

            MobileCenterLog.Debug(LogTag, $"Exception generated !!! {exception.Message}");
            MobileCenterLog.Debug(LogTag, $"{LogSerializer.Serialize(errorLog)}");

            try
            {
                ErrorLogHelper.WriteErrorLogToFile(errorLog);
                ErrorLogHelper.WriteExceptionToFile(exception, errorLog.Id);
            }
            catch (Exception exc)
            {
                MobileCenterLog.Debug(LogTag, $"Unable to write error logs to file: {exc.GetType()}, {exc.Message}");
                MobileCenterLog.Debug(LogTag, $"Aborting!!!");
            }

            // TODO TIZEN check if crash is propogated to native layer for generation of crash dump
            ShutDownHelper.ShutDown();
        }

        private static bool _isHandlerSet = false;

        private static ErrorReport _lastSessionErrorReport = null;

        internal static ErrorReport LastSessionCrashReport
        {
            get
            {
                return _lastSessionErrorReport;
            }
        }

        private static bool ShouldStopProcessingErrors
        {
            get
            {
                if (!Instance.InstanceEnabled)
                {
                    MobileCenterLog.Info(LogTag, "Crashes service is disabled while processing errors. Cancel processing all pending errors.");
                    return true;
                }
                return false;
            }
        }

        private static Task ProcessPendingErrors()
        {
            var fileList = ErrorLogHelper.GetErrorLogFileNames();
            foreach (string fileName in fileList)
            {
                if (ShouldStopProcessingErrors)
                    return Task.CompletedTask;

                ManagedErrorLog errorLog = ErrorLogHelper.ReadErrorLogFromFile(fileName);
                ErrorReport errorReport = BuildErrorReport(errorLog);

                // TIZEN decide whether to process Error Report based on User callbacks
                // TIZEN check if enabled state is changed in the middle - refer android shouldStopProcessingPendingErrors

                if (errorReport == null)
                {
                    ErrorLogHelper.RemoveErrorLogFile(errorLog.Id);
                    ErrorLogHelper.RemoveExceptionFile(errorLog.Id);
                }
                else if (PlatformCrashes.ShouldProcessErrorReport != null && !PlatformCrashes.ShouldProcessErrorReport(errorReport))
                {
                    MobileCenterLog.Debug(LogTag, "PlatFormCrashes.ShouldProcessErrorReport returned false, clean up and ignore log: " + errorLog.Id.ToString());
                    ErrorLogHelper.RemoveErrorLogFile(errorLog.Id);
                    ErrorLogHelper.RemoveExceptionFile(errorLog.Id);
                }
                else
                {
                    MobileCenterLog.Debug(LogTag, "PlatFormCrashes.ShouldProcessErrorReport returned true, continue processing log: " + errorLog.Id.ToString());
                    // TIZEN Store in Unprocessed error cache
                    _UnProcessedErrorReports[errorLog.Id] = Tuple.Create(errorLog, errorReport);
                }

                // TODO TIZEN Handle serialization and file i/o errors
            }

            if (ShouldStopProcessingErrors)
            {
                return Task.CompletedTask;
            }

            // TIZEN After checking shouldProcess()
            // handle user confirmation
            // remove files in the end

            ProcessUserConfirmation();


            return Task.CompletedTask;
        }

        private static Task ProcessUserConfirmation()
        {
            bool shouldAwaitUserConfirmation = PlatformCrashes.ShouldAwaitUserConfirmation();
            MobileCenterLog.Debug(LogTag, $"{_UnProcessedErrorReports.Count}, {_applicationSettings.GetValue(PREF_KEY_ALWAYS_SEND, false)}");
            if (_UnProcessedErrorReports.Count > 0 && (_applicationSettings.GetValue(PREF_KEY_ALWAYS_SEND, false) || !shouldAwaitUserConfirmation))
            {
                if (!shouldAwaitUserConfirmation)
                {
                    MobileCenterLog.Debug(LogTag, "PlatformCrashes.ShouldAwaitUserConfirmation returned false, continue sending logs");
                }
                else
                {
                    MobileCenterLog.Debug(LogTag, "The flag for user confirmation is set to ALWAYS_SEND, continue sending logs");
                }
                HandleUserConfirmation(UserConfirmation.Send);
            }
            return Task.CompletedTask;
        }

        internal static Task HandleUserConfirmation(UserConfirmation userConfirmation)
        {
            // TODO TIZEN look into Android mhandler.post(runnable) and Runnable.Run()
            if (!Instance.InstanceEnabled)
            {
                MobileCenterLog.Error(LogTag, "Crashes service not initialized, discarding calls.");
                return Task.CompletedTask;
            }

            if (userConfirmation == UserConfirmation.DontSend)
            {
                foreach (Guid logId in _UnProcessedErrorReports.Keys)
                {
                    ErrorLogHelper.RemoveErrorLogFile(logId);
                    ErrorLogHelper.RemoveExceptionFile(logId);
                }
                _UnProcessedErrorReports.Clear();
            }
            else
            {
                if (userConfirmation == UserConfirmation.AlwaysSend)
                    _applicationSettings[PREF_KEY_ALWAYS_SEND] = true;


                List<Guid> ToBeRemoved = new List<Guid>();
                foreach (Guid Id in _UnProcessedErrorReports.Keys)
                {
                    if (ShouldStopProcessingErrors)
                        break;

                    var tuple =_UnProcessedErrorReports[Id];
                    var errorLog = tuple.Item1;
                    var errorReport = tuple.Item2;

                    Instance.Channel.Enqueue(errorLog);

                    // TIZEN Process ErrorAttachmentLog
                    IEnumerable<ErrorAttachmentLog> errorAttachments = PlatformCrashes.GetErrorAttachments(errorReport);
                    HandleErrorAttachments(errorAttachments, Id);

                    ToBeRemoved.Add(Id);
                    ErrorLogHelper.RemoveErrorLogFile(Id);
                    ErrorLogHelper.RemoveExceptionFile(Id);
                }
                foreach (Guid Id in ToBeRemoved)
                {
                    _UnProcessedErrorReports.Remove(Id);
                }
            }

            return Task.CompletedTask;
        }

        private static Task HandleErrorAttachments(IEnumerable<ErrorAttachmentLog> errorAttachments, Guid errorId)
        {
            if (errorAttachments == null)
            {
                MobileCenterLog.Info(LogTag, $"PlatformCrashes.GetErrorAttachments returned null. No additional information attached to log: {errorId.ToString()}");
            }
            else
            {
                foreach (ErrorAttachmentLog attachment in errorAttachments)
                {
                    if (attachment == null)
                    {
                        MobileCenterLog.Info(LogTag, $"Skipping null ErrorAttachmentLog");
                    }
                    else
                    {
                        attachment.Id = Guid.NewGuid();
                        attachment.ErrorId = errorId;
                        // TODO TIZEN check if attachment is valid (required fields not null)
                        Instance.Channel.Enqueue(attachment);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private static ErrorReport BuildErrorReport(ManagedErrorLog log)
        {
            if (!_errorReportCache.ContainsKey(log.Id))
            {
                // If not in cache retrieve from file
                Exception exception = ErrorLogHelper.ReadExceptionFromFile(log.Id);
                _errorReportCache[log.Id] = new ErrorReport(log, exception);
            }
            return _errorReportCache[log.Id];
            // TODO TIZEN handle errors and return null if failed
        }

        #endregion


        #region instance
        protected override string ChannelName => "crashes";

        public override string ServiceName => "Crashes";

        internal Crashes()
        {
            LogSerializer.AddLogType(ManagedErrorLog.JsonIdentifier, typeof(ManagedErrorLog));
            LogSerializer.AddLogType(ErrorAttachmentLog.JsonIdentifier, typeof(ErrorAttachmentLog));
            ErrorLogHelper.InitializeErrorDirectoryPath();
            _errorReportCache = new Dictionary<Guid, ErrorReport>();
            _UnProcessedErrorReports = new Dictionary<Guid, Tuple<ManagedErrorLog, ErrorReport>>();
        }

        public override bool InstanceEnabled
        {
            get
            {
                return base.InstanceEnabled;
            }
            set
            {
                lock (_serviceLock)
                {
                    var prevValue = InstanceEnabled;
                    base.InstanceEnabled = value;
                    if (value != prevValue)
                    {
                        ApplyEnabledState(value);
                    }
                }
            }
        }

        private long initializeTimeStamp;

        private void ApplyEnabledState(bool enabled)
        {
            lock (_serviceLock)
            {
                // TIZEN Implement Crashes.ApplyEnabled State
                // Based on if (enabled), do the following
                // 1) Set Enabled = true
                //    Set the exception handlers
                //    Instantiate and assign necessary variables
                //    Start Thread to check for stored error logs
                // 2) Set Enabled = false
                //    Destroy crash logs, etc, etc
                //    Look into it

                initialize(enabled);
            }
        }

        private void initialize(bool enabled)
        {
            initializeTimeStamp = enabled ? TimeHelper.CurrentTimeInMilliseconds() : -1;
            if (!enabled)
            {
                if (_isHandlerSet)
                {
                    AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
                    _isHandlerSet = false;
                }

                // TIZEN
                // Stop tasks related to crashes -> checking for crash data
                // Delete previously stored crash logs
                ErrorLogHelper.RemoveAllFiles();
            }
            else
            {
                if (!_isHandlerSet)
                {
                    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                    _isHandlerSet = true;

                    // TIZEN
                    // Acquire countdown latch
                    // Retrieve error log file and send to Mobile Center
                    string filePath = ErrorLogHelper.GetLastAddedLogFile();
                    if (filePath != null)
                    {
                        _countDownLatch = new CountdownEvent(1);
                        Task.Run(() =>
                        {
                            ManagedErrorLog errorLog = ErrorLogHelper.ReadErrorLogFromFile(filePath);

                            // TODO TIZEN checking for read failures

                            // TIZEN build error report and store it
                            _lastSessionErrorReport = BuildErrorReport(errorLog);
                            _countDownLatch.Signal();
                        });
                    }
                }
            }
        }

        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            // TIZEN Implement Crashes.OnChannelGroupReady

            lock (_serviceLock)
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(InstanceEnabled);

                Channel.SendingLog += (sender, args) =>
                {
                    if (args.Log.GetType() != typeof(ManagedErrorLog))
                        return;

                    var errorReportEventArgs = new SendingErrorReportEventArgs();
                    errorReportEventArgs.Report = _errorReportCache[((ManagedErrorLog)args.Log).Id];
                    PlatformCrashes.SendingErrorReport?.Invoke(sender, errorReportEventArgs);
                };

                Channel.SentLog += (sender, args) =>
                {
                    if (args.Log.GetType() != typeof(ManagedErrorLog))
                        return;

                    var errorReportEventArgs = new SentErrorReportEventArgs();
                    errorReportEventArgs.Report = _errorReportCache[((ManagedErrorLog)args.Log).Id];
                    PlatformCrashes.SentErrorReport?.Invoke(sender, errorReportEventArgs);
                };

                Channel.FailedToSendLog += (sender, args) =>
                {
                    if (args.Log.GetType() != typeof(ManagedErrorLog))
                        return;

                    var errorReportEventArgs = new FailedToSendErrorReportEventArgs();
                    errorReportEventArgs.Report = _errorReportCache[((ManagedErrorLog)args.Log).Id];
                    errorReportEventArgs.Exception = args.Exception;
                    PlatformCrashes.FailedToSendErrorReport?.Invoke(sender, errorReportEventArgs);
                };

                if (InstanceEnabled)
                {
                    // TIZEN process pending errors
                    ProcessPendingErrors();
                }
            }
        }
        #endregion
    }
}
