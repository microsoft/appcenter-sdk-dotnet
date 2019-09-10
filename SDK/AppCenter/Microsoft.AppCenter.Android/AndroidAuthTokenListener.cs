// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Com.Microsoft.Appcenter;

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

        public void AcquireAuthToken(IAuthTokenCallback callback)
        {
            _acquireAuthToken.Invoke().ContinueWith(t => callback.OnAuthTokenResult(t.Result));
        }
    }
}