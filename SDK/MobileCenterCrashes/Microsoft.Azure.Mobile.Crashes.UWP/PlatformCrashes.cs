using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Crashes
{

    public class TestCrashException : Exception { }

    /* Parts of this are adapted from http://stackoverflow.com/questions/6452951/how-to-dynamically-load-and-unload-a-native-dll-file */

    class PlatformCrashes : PlatformCrashesBase
    {
        private const string WatsonKey = "VSMCAppSecret";
        private const string ErrorMessage = "Crashes will not be reported for this version of Windows; please upgrade to a newer version in order to enable Crash reporting.";
        
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadPackagedLibrary(string libname);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        private delegate int RegisterCustomMetadataDelegate([MarshalAs(UnmanagedType.LPWStr)]string key, [MarshalAs(UnmanagedType.LPWStr)]string value);

        /// <exception cref="MobileCenterException"/>
        public void Configure(string appSecret)
        {
            var handle = LoadPackagedLibrary("kernel32.dll");
            if (handle == IntPtr.Zero)
            {
                throw new MobileCenterException(ErrorMessage);
            }
            try
            {
                var address = GetProcAddress(handle, "WerRegisterCustomMetadata");
                if (address == IntPtr.Zero)
                {
                    throw new MobileCenterException(ErrorMessage);
                }
                var registrationMethod = Marshal.GetDelegateForFunctionPointer<RegisterCustomMetadataDelegate>(address);
                if (registrationMethod == null)
                {
                    throw new MobileCenterException(ErrorMessage);
                }
                registrationMethod.Invoke(WatsonKey, appSecret);
            }
            finally
            {
                FreeLibrary(handle);
            }
        }

        // Note: in PlatformCrashes we use only callbacks; not events (in Crashes, there are corresponding events)
        public override SendingErrorReportEventHandler SendingErrorReport { get; set; }
        public override SentErrorReportEventHandler SentErrorReport { get; set; }
        public override FailedToSendErrorReportEventHandler FailedToSendErrorReport { get; set; }
        public override ShouldProcessErrorReportCallback ShouldProcessErrorReport { get; set; }
        //public override GetErrorAttachmentCallback GetErrorAttachment { get; set; }
        public override ShouldAwaitUserConfirmationCallback ShouldAwaitUserConfirmation { get; set; }

        public override void NotifyUserConfirmation(UserConfirmation confirmation)
        {
        }
        public override bool Enabled { get; set; }
        public override bool HasCrashedInLastSession => false;

        public override Type BindingType => typeof(Crashes);

        public override Task<ErrorReport> GetLastSessionCrashReportAsync()
        {
            return null;
        }      
    }
}