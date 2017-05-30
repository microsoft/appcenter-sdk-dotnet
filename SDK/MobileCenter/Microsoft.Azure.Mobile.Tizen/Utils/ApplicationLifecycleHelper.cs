using System;
/*using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;*/

using Tizen.Applications;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        private static bool _started;
        private static bool _suspended;

        //Static instance of current Application
        private static CoreUIApplication CoreUIApp = (CoreUIApplication)(Application.Current);

        public ApplicationLifecycleHelper()
        {
            Enabled = true;
            _started = true;

            // TODO add unhandled exception handler
            /*Application.Current.UnhandledException += (sender, eventArgs) =>
            {
                UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs(eventArgs.Exception));
            };*/
        }

        private void InvokeResuming(object sender, object e)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Calling InvokeResuming()");
            _suspended = false;
            ApplicationResuming?.Invoke(sender, EventArgs.Empty);
        }

        private void InvokeSuspending(object sender, object e)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Calling InvokeSuspending()");
            _suspended = true;
            ApplicationSuspended?.Invoke(sender, EventArgs.Empty);
        }

        private bool _enabled;
        public bool Enabled {
            get
            {
                return _enabled;
            }
            set
            {
                if (value == _enabled)
                {
                    return;
                }
                if (value)
                {
                    CoreUIApp.Resumed += InvokeResuming;
                    CoreUIApp.Paused += InvokeSuspending;
                    //CoreApplication.LeavingBackground += InvokeResuming;
                    //CoreApplication.EnteredBackground += InvokeSuspended;
                }
                else
                {
                    CoreUIApp.Resumed -= InvokeResuming;
                    CoreUIApp.Paused -= InvokeSuspending;
                    //CoreApplication.LeavingBackground -= InvokeResuming;
                    //CoreApplication.EnteredBackground -= InvokeSuspended;
                }
                _enabled = value;
            }
        }

        public bool HasShownWindow => _started;
        public bool IsSuspended => _suspended;

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler ApplicationStarted;

        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
