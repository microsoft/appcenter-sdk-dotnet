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
        public new static string LogTag = "MobileCenterCrashes";

        private static readonly object CrashesLock = new object();

        private static Crashes _instanceField;

        internal static CountdownEvent _countDownLatch = null;

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

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // DONE
            // TODO TIZEN Handle crash
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

        private static Task processPendingErrors()
        {
            var fileList = ErrorLogHelper.GetErrorLogFileNames();
            foreach (string fileName in fileList)
            {
                ManagedErrorLog errorLog = ErrorLogHelper.ReadErrorLogFromFile(fileName);
                ErrorReport errorReport = BuildErrorReport(errorLog);

                // TODO TIZEN decide whether to process Error Report based on User callbacks
                // TODO TIZEN check if enabled state is changed in the middle - refer android shouldStopProcessingPendingErrors

                // testing
                // only enque after performing all checks
                Instance.Channel.Enqueue(errorLog);
                ErrorLogHelper.RemoveErrorLogFile(errorLog.Id);
                ErrorLogHelper.RemoveExceptionFile(errorLog.Id);
            }
            // TODO TIZEN After checking shouldProcess()
            // handle user confirmation
            // remove files in the end


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
            ErrorLogHelper.InitializeErrorDirectoryPath();
            _errorReportCache = new Dictionary<Guid, ErrorReport>();
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
                initialize(enabled);
                // TODO TIZEN Implement Crashes.ApplyEnabled State
                // Based on if (enabled), do the following
                // 1) Set Enabled = true
                //    Set the exception handlers
                //    Instantiate and assign necessary variables
                //    Start Thread to check for stored error logs
                // 2) Set Enabled = false
                //    Destroy crash logs, etc, etc
                //    Look into it

                // refer to Android Implementation
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

                // TODO TIZEN
                // Stop tasks related to crashes -> checking for crash data
                // Delete previously stored crash logs??

            }
            else
            {
                if (!_isHandlerSet)
                {
                    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                    _isHandlerSet = true;

                    // TODO TIZEN
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

                            // TODO TIZEN build error report and store it
                            _lastSessionErrorReport = BuildErrorReport(errorLog);
                            _countDownLatch.Signal();

                            // TODO TIZEN process any callbacks registered by the user
                        });
                    }
                }
            }
        }

        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            //MobileCenterLog.Warn(MobileCenterLog.LogTag, "Crashes service is not yet supported on Tizen.");
            //lock (_serviceLock)
            //{
            //    // TODO TIZEN Implement Crashes.OnChannelGroupReady
            //    // base.OnChannelGroupReady(channelGroup, appSecret);

            //    // Refer to implementation from Android
            //}

            lock (_serviceLock)
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(InstanceEnabled);
                if (InstanceEnabled)
                {
                    // TODO TIZEN process pending errors
                    processPendingErrors();
                }
            }
        }



        #endregion
    }
}
