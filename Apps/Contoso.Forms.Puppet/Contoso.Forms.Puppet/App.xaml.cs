// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinDevice = Xamarin.Forms.Device;

namespace Contoso.Forms.Puppet
{
    public interface IClearCrashClick
    {
        void ClearCrashButton();
    }

    public partial class App
    {
        public const string LogTag = "AppCenterXamarinPuppet";
        private Task<string> dialog = null;

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPuppetPage());
        }

        protected override void OnStart()
        {
            if (!AppCenter.Configured)
            {
                AppCenterLog.Assert(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
                AppCenter.LogLevel = LogLevel.Verbose;
                AppCenterLog.Info(LogTag, "AppCenter.LogLevel=" + AppCenter.LogLevel);
                AppCenterLog.Info(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

                // Set callbacks
                Crashes.ShouldProcessErrorReport = ShouldProcess;
                Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
                Crashes.GetErrorAttachments = GetErrorAttachmentsCallback;
                Distribute.ReleaseAvailable = OnReleaseAvailable;
                Distribute.WillExitApp = OnWillExitApp;
                Distribute.NoReleaseAvailable = OnNoReleaseAvailable;

                // Event handlers
                Crashes.SendingErrorReport += SendingErrorReportHandler;
                Crashes.SentErrorReport += SentErrorReportHandler;
                Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;

                // Country code.
                if (Current.Properties.ContainsKey(Constants.CountryCode)
                    && Current.Properties[Constants.CountryCode] is string countryCode)
                {
                    AppCenter.SetCountryCode(countryCode);
                }

                // Manual session tracker.
                if (Current.Properties.ContainsKey(Constants.EnableManualSessionTracker)
                    && Current.Properties[Constants.EnableManualSessionTracker] is bool isEnabled
                    && isEnabled)
                {
                    Analytics.EnableManualSessionTracker();
                }

                AppCenterLog.Assert(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

                if (!StartType.OneCollector.Equals(StartTypeUtils.GetPersistedStartType()))
                {
                    AppCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
                }

                Distribute.SetInstallUrl("https://install.portal-server-core-integration.dev.avalanch.es");
                Distribute.SetApiUrl("https://api-gateway-core-integration.dev.avalanch.es/v0.1");
                var updateTrack = TrackUpdateUtils.GetPersistedUpdateTrack();
                if (updateTrack != null)
                {
                    Distribute.UpdateTrack = updateTrack.Value;
                }
                if (Current.Properties.TryGetValue(Constants.AutomaticUpdateCheckKey, out object persistedObject) && !(bool)persistedObject)
                {
                    Distribute.DisableAutomaticCheckForUpdate();
                }
                if (Current.Properties.ContainsKey(Constants.StorageMaxSize) && Current.Properties[Constants.StorageMaxSize] is long size)
                {
                    AppCenter.SetMaxStorageSizeAsync(size);
                }
                AppCenter.Start(GetTokensString(), typeof(Analytics), typeof(Crashes), typeof(Distribute));
                if (Current.Properties.ContainsKey(Constants.UserId) && Current.Properties[Constants.UserId] is string id)
                {
                    AppCenter.SetUserId(id);
                }
                AppCenter.IsEnabledAsync().ContinueWith(enabled =>
                {
                    AppCenterLog.Info(LogTag, "AppCenter.Enabled=" + enabled.Result);
                });
                AppCenter.GetInstallIdAsync().ContinueWith(installId =>
                {
                    AppCenterLog.Info(LogTag, "AppCenter.InstallId=" + installId.Result);
                });
                AppCenterLog.Info(LogTag, "AppCenter.SdkVersion=" + AppCenter.SdkVersion);
                Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
                {
                    AppCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
                });
                Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
                {
                    AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.StackTrace=" + task.Result?.StackTrace);
                });
            }
        }

        private string GetOneCollectorTokenString()
        {
            return DependencyService.Get<IAppConfiguration>().GetTargetToken();
        }

        private string GetAppCenterTokenString()
        {
            return DependencyService.Get<IAppConfiguration>().GetAppSecret();
        }

        private string GetTokensString()
        {
            var persistedStartType = StartTypeUtils.GetPersistedStartType();
            switch (persistedStartType)
            {
                case StartType.OneCollector:
                    return GetOneCollectorTokenString();
                case StartType.Both:
                    return $"{GetAppCenterTokenString()};{GetOneCollectorTokenString()}";
                default:
                    return GetAppCenterTokenString();
            }
        }

        static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");
        }

        static void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");
        }

        static void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");
        }

        bool ShouldProcess(ErrorReport report)
        {
            AppCenterLog.Info(LogTag, "Determining whether to process error report");
            return true;
        }

        bool ConfirmationHandler()
        {
            XamarinDevice.BeginInvokeOnMainThread(() =>
            {
                if (XamarinDevice.RuntimePlatform == XamarinDevice.macOS)
                {
                    dialog = Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", "Send", "Always Send");
                }
                else
                {
                    dialog = Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", null, null, "Send", "Always Send", "Don't Send");
                }
                dialog.ContinueWith((arg) =>
                {
                    var answer = arg.Result;
                    UserConfirmation userConfirmationSelection;
                    if (answer == "Send")
                    {
                        userConfirmationSelection = UserConfirmation.Send;
                    }
                    else if (answer == "Always Send")
                    {
                        userConfirmationSelection = UserConfirmation.AlwaysSend;
                    }
                    else
                    {
                        userConfirmationSelection = UserConfirmation.DontSend;
                    }
                    AppCenterLog.Debug(LogTag, "User selected confirmation option: \"" + answer + "\"");
                    Crashes.NotifyUserConfirmation(userConfirmationSelection);
                });
            });
            return true;
        }

        static IEnumerable<ErrorAttachmentLog> GetErrorAttachmentsCallback(ErrorReport report)
        {
            return GetErrorAttachments();
        }

        public static IEnumerable<ErrorAttachmentLog> GetErrorAttachments()
        {
            var attachments = new List<ErrorAttachmentLog>();
            if (Current.Properties.TryGetValue(CrashesContentPage.TextAttachmentKey, out var textAttachment) &&
                textAttachment is string text)
            {
                var attachment = ErrorAttachmentLog.AttachmentWithText(text, "hello.txt");
                attachments.Add(attachment);
            }
            if (Current.Properties.TryGetValue(CrashesContentPage.FileAttachmentKey, out var fileAttachment) &&
                fileAttachment is string file)
            {
                var filePicker = DependencyService.Get<IFilePicker>();
                if (filePicker != null)
                {
                    try
                    {
                        var result = filePicker.ReadFile(file);
                        if (result != null)
                        {
                            var attachment = ErrorAttachmentLog.AttachmentWithBinary(result.Item1, result.Item2, result.Item3);
                            attachments.Add(attachment);
                        }
                    }
                    catch (Exception e)
                    {
                        AppCenterLog.Warn(LogTag, "Couldn't read file attachment", e);
                        Current.Properties.Remove(CrashesContentPage.FileAttachmentKey);
                    }
                }
            }
            return attachments;
        }

        void OnNoReleaseAvailable()
        {
            AppCenterLog.Info(LogTag, "No release available callback invoked.");
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            AppCenterLog.Info(LogTag, "OnReleaseAvailable id=" + releaseDetails.Id
                                            + " version=" + releaseDetails.Version
                                            + " releaseNotesUrl=" + releaseDetails.ReleaseNotesUrl);
            var custom = releaseDetails.ReleaseNotes?.ToLowerInvariant().Contains("custom") ?? false;
            if (custom)
            {
                var title = "Version " + releaseDetails.ShortVersion + " available!";
                Task answer;
                if (releaseDetails.MandatoryUpdate)
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!");
                }
                else
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!", "Maybe tomorrow...");
                }
                answer.ContinueWith((task) =>
                {
                    if (releaseDetails.MandatoryUpdate || ((Task<bool>)task).Result)
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Update);
                    }
                    else
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                    }
                });
            }
            return custom;
        }

        void OnWillExitApp()
        {
            AppCenterLog.Info(LogTag, "App will close callback invoked.");
        }
    }
}
