﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.iOS.Bindings;

namespace Microsoft.AppCenter
{
    // Maps Objective-C delegate to callback in C#.
    internal class AuthTokenDelegate : MSAuthTokenDelegate
    {
        private readonly Func<Task<string>> _acquireAuthToken;

        public AuthTokenDelegate(Func<Task<string>> acquireAuthToken)
        {
            _acquireAuthToken = acquireAuthToken;
        }

        void AcquireToken(MSAppCenter appCenter, MSAuthTokenCompletionHandler completionHandler)
        {
            _acquireAuthToken.Invoke().ContinueWith(t => completionHandler(t.Result));
        }
    }
}
