// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using System.Globalization;
using Microsoft.AspNetCore.StaticFiles;

namespace Contoso.WinUI.Desktop.Demo
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private ApplicationDataContainer localSettings;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {

            // Init settings.
            localSettings = ApplicationData.Current.LocalSettings;
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs args) =>
            {
                // If you see this message while testing the app and if the stack trace is SDK related, we might have a bug in the SDK as we don't want to leak any exception from the SDK.
                AppCenterLog.Error("AppCenterPuppet", "Unobserved exception observed=" + args.Observed, args.Exception);
            };
            CoreApplication.EnablePrelaunch(true);
            InitializeComponent();
            AppCenter.LogLevel = LogLevel.Verbose;

            // Set data from local storage.
            var countryCode = localSettings.Values[Constants.KeyCountryCode] as string;
            if (!string.IsNullOrEmpty(countryCode))
            {
                AppCenter.SetCountryCode(countryCode);
            }
            var storageSize = localSettings.Values[Constants.KeyStorageMaxSize] as long?;
            if (storageSize != null && storageSize > 0)
            {
                AppCenter.SetMaxStorageSizeAsync((long)storageSize);
            }
            var isManualSessionTrackerEnabled = localSettings.Values[Constants.KeyEnableManualSessionTracker] as bool?;
            if (isManualSessionTrackerEnabled != null && isManualSessionTrackerEnabled.Value)
            {
                Analytics.EnableManualSessionTracker();
            }

            // User callbacks.
            Crashes.ShouldProcessErrorReport = (report) =>
            {
                Log($"Determining whether to process error report with an ID: {report.Id}");
                return true;
            };
            Crashes.GetErrorAttachments = GetErrorAttachmentsHandler;

            // Event handlers.
            Crashes.SendingErrorReport += (_, args) => Log($"Sending error report for an error ID: {args.Report.Id}");
            Crashes.SentErrorReport += (_, args) => Log($"Sent error report for an error ID: {args.Report.Id}");
            Crashes.FailedToSendErrorReport += (_, args) => Log($"Failed to send error report for an error ID: {args.Report.Id}");

            // Start App Center.
            var appSecret = Environment.GetEnvironmentVariable("WINUI_IN_DESKTOP_PROD");
            AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes));

            // Set userId.
            var userId = localSettings.Values[Constants.KeyUserId] as string;
            if (!string.IsNullOrEmpty(userId))
            {
                AppCenter.SetUserId(userId);
            }
            Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
            {
                Log("Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
            });
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
            {
                Log("Crashes.LastSessionCrashReport.StackTrace=" + task.Result?.StackTrace);
            });
        }

        private static void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            System.Diagnostics.Debug.WriteLine($"{timestamp} [AppCenterPuppet] Info: {message}");
        }

        private static IEnumerable<ErrorAttachmentLog> GetErrorAttachmentsHandler(ErrorReport report)
        {
            return GetErrorAttachments();
        }

        public static IEnumerable<ErrorAttachmentLog> GetErrorAttachments()
        {
            var attachments = new List<ErrorAttachmentLog>();

            // Text attachment.
            var localSettings = ApplicationData.Current.LocalSettings;
            var textAttachments = localSettings.Values[Constants.KeyTextErrorAttachments] as string;
            if (!string.IsNullOrEmpty(textAttachments))
            {
                attachments.Add(ErrorAttachmentLog.AttachmentWithText(textAttachments, "text.txt"));
            }

            // Binary attachment.
            var fileAttachments = localSettings.Values[Constants.KeyFileErrorAttachments] as string;
            if (!string.IsNullOrEmpty(fileAttachments))
            {
                if (File.Exists(fileAttachments))
                {
                    var fileName = new FileInfo(fileAttachments).Name;
                    var provider = new FileExtensionContentTypeProvider();
                    if (!provider.TryGetContentType(fileName, out var contentType))
                    {
                        contentType = "application/octet-stream";
                    }
                    var fileContent = File.ReadAllBytes(fileAttachments);
                    attachments.Add(ErrorAttachmentLog.AttachmentWithBinary(fileContent, fileName, contentType));
                }
                else
                {
                    localSettings.Values[Constants.KeyFileErrorAttachments] = null;
                }
            }
            return attachments;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
