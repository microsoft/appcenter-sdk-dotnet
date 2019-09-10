using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.iOS.Bindings;

namespace Microsoft.AppCenter
{
    // Maps Objective-C delegate to callback in C#.
    internal class AuthTokenDelegate : MSAuthTokenDelegate
    {
        private Func<Task<string>> _acquireAuthToken;

        public AuthTokenDelegate(Func<Task<string>> acquireAuthToken)
        {
            _acquireAuthToken = acquireAuthToken;
        }

        void AcquireToken(MSAppCenter appCenter, MSAuthTokenCompletionHandler completionHandler)
        {
            Task.Factory.StartNew(async () =>
            {
                if (_acquireAuthToken != null)
                {
                    var authToken = await _acquireAuthToken();
                    completionHandler?.Invoke(authToken);
                }
            });
        }
    }
}
