// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

namespace Contoso.MAUI.Demo;

public interface IClearCrashClick
{
    void ClearCrashButton();
}

public partial class App : Application
{
    public const string LogTag = "AppCenterMAUIDemo";
    private Task<string> dialog = null;

    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();

        StartAppCenter();
    }

    private void StartAppCenter()
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
            if (Preferences.ContainsKey(Constants.CountryCode))
            {
                AppCenter.SetCountryCode(Preferences.Get(Constants.CountryCode, string.Empty));
            }

            // Manual session tracker.
            if (Preferences.ContainsKey(Constants.EnableManualSessionTracker)
                && Preferences.Get(Constants.EnableManualSessionTracker, false))
            {
                Analytics.EnableManualSessionTracker();
            }

            AppCenterLog.Assert(LogTag, "AppCenter.Configured=" + AppCenter.Configured);

            var updateTrack = TrackUpdateUtils.GetPersistedUpdateTrack();
            if (updateTrack != null)
            {
                Distribute.UpdateTrack = updateTrack.Value;
            }
            if (!Preferences.Get(Constants.AutomaticUpdateCheckKey, true))
            {
                Distribute.DisableAutomaticCheckForUpdate();
            }
            if (Preferences.ContainsKey(Constants.StorageMaxSize))
            {
                AppCenter.SetMaxStorageSizeAsync(Preferences.Get(Constants.StorageMaxSize, 0));
            }

            var appSecret = GetTokensString();
            AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes), typeof(Distribute));

            if (Preferences.ContainsKey(Constants.UserId))
            {
                AppCenter.SetUserId(Preferences.Get(Constants.UserId, string.Empty));
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
        return "iosTarget={MAUI_IOS_TARGET_TOKEN_PROD};" +
            "androidTarget={MAUI_ANDROID_TARGET_TOKEN_PROD};" +
            $"windowsTarget={Environment.GetEnvironmentVariable("MAUI_WINDOWS_TARGET_TOKEN_PROD")}";
    }

    private string GetAppCenterTokenString()
    {
        return "ios={MAUI_IOS_PROD};" +
            "android={MAUI_ANDROID_PROD};" +
            $"windows={Environment.GetEnvironmentVariable("MAUI_WINDOWS_PROD")}";
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
        
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string answer;
            if (DeviceInfo.Platform == DevicePlatform.macOS)
            {
                answer = await Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", "Send", "Always Send");
            }
            else
            {
                answer = await Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", null, null, "Send", "Always Send", "Don't Send");
            }

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
        return true;
    }

    static IEnumerable<ErrorAttachmentLog> GetErrorAttachmentsCallback(ErrorReport report)
    {
        return GetErrorAttachments();
    }

    public static IEnumerable<ErrorAttachmentLog> GetErrorAttachments()
    {
        var attachments = new List<ErrorAttachmentLog>();
        if (Preferences.ContainsKey(CrashesContentPage.TextAttachmentKey))
        {
            var attachment = ErrorAttachmentLog.AttachmentWithText(Preferences.Get(CrashesContentPage.TextAttachmentKey, string.Empty), "hello.txt");
            attachments.Add(attachment);
        }
        if (Preferences.ContainsKey(CrashesContentPage.FileAttachmentKey))
        {
            var filePicker = DependencyService.Get<IFilePicker>();
            if (filePicker != null)
            {
                try
                {
                    var filePath = Preferences.Get(CrashesContentPage.FileAttachmentKey, string.Empty);
                    var fileBytes = File.ReadAllBytes(filePath);

                    //var result = filePicker.ReadFile(Preferences.Get(CrashesContentPage.FileAttachmentKey, string.Empty);
                    if (fileBytes != null)
                    {
                        var attachment = ErrorAttachmentLog.AttachmentWithBinary(fileBytes, Path.GetFileName(filePath), null);
                        attachments.Add(attachment);
                    }
                }
                catch (Exception e)
                {
                    AppCenterLog.Warn(LogTag, "Couldn't read file attachment", e);
                    Preferences.Remove(CrashesContentPage.FileAttachmentKey);
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
