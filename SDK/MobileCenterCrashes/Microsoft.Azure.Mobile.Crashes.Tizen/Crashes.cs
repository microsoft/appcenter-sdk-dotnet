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
        #region static
        private static readonly object CrashesLock = new object();

        public static readonly string LogTag = "Crashes";

        private static Crashes _instanceField;

        public static Crashes Instance
        {
            get
            {
                lock(CrashesLock)
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

        #endregion


        #region instance

        //protected override string ChannelName => "crashes";

        public /*override*/ string ServiceName => "Crashes";

        public /*override*/ bool InstanceEnabled
        {
            get
            {
                //return base.InstanceEnabled;
                return true;
            }

            set
            {
            }
        }

        private bool _hasStarted;

        //private void ApplyEnabledState(bool enabled)
        //{
        //    lock (_serviceLock)
        //    {
        //        // TODO TIZENImplement Crashes.ApplyEnabled State
        //        // Based on if (enabled), do the following
        //        // 1) Set Enabled = true
        //        //    Set the exception handlers
        //        //    Instantiate and assign necessary variables
        //        //    Start Thread to check for stored error logs
        //        // 2) Set Enabled = false
        //        //    Destroy crash logs, etc, etc

        //        // TODO refer to Android Implementation
        //    }
        //}

        public /*override*/ void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                MobileCenterLog.Debug(Crashes.LogTag, $"Crashes has Received Exception!!!\n{(Exception)args.ExceptionObject}");
            };

            //MobileCenterLog.Warn(MobileCenterLog.LogTag, "Crashes service is not yet supported on Tizen.");
            //lock (_serviceLock)
            //{
            //    // TODO TIZEN Implement Crashes.OnChannelGroupReady
            //    // base.OnChannelGroupReady(channelGroup, appSecret);

            //    // Refer to implementation from Android
            //}
        }

        #endregion
    }
}
