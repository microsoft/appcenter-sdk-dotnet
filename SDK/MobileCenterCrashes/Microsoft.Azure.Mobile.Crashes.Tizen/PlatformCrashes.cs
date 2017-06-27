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
        public override GetErrorAttachmentsCallback GetErrorAttachments { get; set; }
        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }

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
            Crashes._countDownLatch.Wait();
            return Task.FromResult(Crashes.LastSessionCrashReport);
        }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
        {
            // TODO TIZEN Check user confirmation
            // Trigger action based on that value

        }

        static PlatformCrashes()
        {
            MobileCenterLog.Info(Crashes.LogTag, "Set up crash handler.");

            // TODO TIZEN find way to retrieve native crashes
            //AndroidCrashes.Instance.SetWrapperSdkListener(new CrashListener());
        }

        public PlatformCrashes()
        {
        }

    }
}
