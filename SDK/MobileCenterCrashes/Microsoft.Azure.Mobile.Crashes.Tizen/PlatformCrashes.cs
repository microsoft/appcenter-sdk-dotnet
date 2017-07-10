using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    internal class PlatformCrashes : PlatformCrashesBase
    {
        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)

        public override SendingErrorReportEventHandler SendingErrorReport { get; set; }
        public override SentErrorReportEventHandler SentErrorReport { get; set; }
        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport { get; set; }
        public override GetErrorAttachmentsCallback GetErrorAttachments
        {
            get
            {
                if (Crashes._GetErrorAttachments == null)
                {
                    return (errorReport) =>
                    {
                        return null;
                    };
                }
                return Crashes._GetErrorAttachments;
            }
            set
            {
                Crashes._GetErrorAttachments = value;
            }
        }
        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport
        {
            get
            {
                if (Crashes._ShouldProcessErrorReport == null)
                {
                    return (errorReport) => { return true; };
                }
                return Crashes._ShouldProcessErrorReport;
            }
            set
            {
                Crashes._ShouldProcessErrorReport = value;
            }
        }

        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation
        {
            get
            {
                if (Crashes._ShouldAwaitUserConfirmation == null)
                {
                    return () => { return false; };
                }
                return Crashes._ShouldAwaitUserConfirmation;
            }
            set
            {
                Crashes._ShouldAwaitUserConfirmation = value;
            }
        }

        public override Type BindingType => typeof(Crashes);

        public override bool Enabled
        {
            get
            {
                return Crashes.Instance.InstanceEnabled;
            }
            set
            {
                Crashes.Instance.InstanceEnabled = value;
            }
        }

        public override bool HasCrashedInLastSession
        {
            get
            {
                return (Crashes.LastSessionCrashReport != null || (Crashes._countDownLatch != null && Crashes._countDownLatch.CurrentCount > 0));
            }
        }

        public override Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            if (Crashes._countDownLatch != null)
                Crashes._countDownLatch.Wait();
            return Task.FromResult(Crashes.LastSessionCrashReport);
        }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
        {
            Crashes.HandleUserConfirmation(confirmation);
        }

        static PlatformCrashes()
        {
            MobileCenterLog.Info(Crashes.LogTag, "Set up crash handler.");
            // TODO TIZEN find way to retrieve native crashes
        }

        public PlatformCrashes()
        {
        }

    }
}
