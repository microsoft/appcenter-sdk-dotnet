// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter
{
    // Maps Objective-C delegate to callback in C#.
    class AndroidAuthTokenListener : Java.Lang.Object, IAuthTokenListener
    {
        private readonly Func<Task<string>> _acquireAuthToken;

        public AndroidAuthTokenListener(Func<Task<string>> acquireAuthToken)
        {
            _acquireAuthToken = acquireAuthToken;
        }

        void AcquireAuthToken(AuthTokenCallback authTokenCallback)
        {
            _acquireAuthToken.Invoke().ContinueWith(t => authTokenCallback(t.Result));
        }
    }
}