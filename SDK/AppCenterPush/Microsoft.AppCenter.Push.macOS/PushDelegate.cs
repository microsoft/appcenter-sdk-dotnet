using System;
using Microsoft.AppCenter.Push.macOS.Bindings;

namespace Microsoft.AppCenter.Push.macOS
{
    public class PushDelegate : MSPushDelegate
    {
        public override void ReceivedPushNotification(MSPush push, MSPushNotification pushNotification)
        {
            OnPushNotificationReceivedAction?.Invoke(pushNotification);
        }

        public Action<MSPushNotification> OnPushNotificationReceivedAction { get; set; }
    }
}
