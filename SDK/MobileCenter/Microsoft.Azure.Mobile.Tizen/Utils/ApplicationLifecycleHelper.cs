using System;
/*using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;*/

using Tizen.Applications;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
    {
        //Static instance of current Application
        private static CoreUIApplication CoreUIApp = (CoreUIApplication)(Application.Current);

        public ApplicationLifecycleHelper()
        {
            Enabled = true;

            // TODO add unhandled exception handler
            /*Application.Current.UnhandledException += (sender, eventArgs) =>
            {
                UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs(eventArgs.Exception));
            };*/
        }

        private void InvokeResuming(object sender, object e)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Calling InvokeResuming()");
            ApplicationResuming?.Invoke(sender, EventArgs.Empty);
        }

        private void InvokeSuspending(object sender, object e)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Calling InvokeSuspending()");
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
                }
                else
                {
                    CoreUIApp.Resumed -= InvokeResuming;
                    CoreUIApp.Paused -= InvokeSuspending;
                }
                _enabled = value;
            }
        }

        public event EventHandler ApplicationSuspended;
        public event EventHandler ApplicationResuming;
        public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;
    }
}
