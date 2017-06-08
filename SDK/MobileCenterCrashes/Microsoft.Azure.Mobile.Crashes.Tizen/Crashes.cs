using Microsoft.Azure.Mobile.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class Crashes : IMobileCenterService
    {
        public string ServiceName => "Crashes";

        public bool InstanceEnabled { get; set; }

        private static Crashes _instanceField;

        public static Crashes Instance
        {
            get
            {
                return _instanceField ?? (_instanceField = new Crashes());
            }
            set
            {
                _instanceField = value; //for testing
            }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            MobileCenterLog.Warn(MobileCenterLog.LogTag, "Crashes service is not yet supported on Tizen.");
            //base.OnChannelGroupReady(channelGroup);
        }
    }
}
