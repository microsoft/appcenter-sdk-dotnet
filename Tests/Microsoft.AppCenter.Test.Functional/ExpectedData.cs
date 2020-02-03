// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.Functional
{
     internal class ExpectedData : IDisposable
    {
        private bool _disposed = false;

        internal HttpResponse Response { get; set; }
        internal Func<RequestData, bool> Where { get; set; }
        internal TaskCompletionSource<RequestData> TaskCompletionSource { get; set; }
        internal CancellationTokenSource CancellationSource { get; set; }

        internal ExpectedData(TimeSpan tokenTimeout)
        {
            // Since we can't directly assign token to a Task created with FromResult,
            // and we can't work with token in SendAsync because it may or may not be called after this method,
            // we are registering anonymous cancellation token here.
            // The only weak point can occur if there's a significant delay between actual Task creation time and calling this method,
            // but since we only use that in tests, that can be ignored.
            CancellationSource = new CancellationTokenSource(tokenTimeout);
            CancellationSource.Token.Register(() => TaskCompletionSource.TrySetCanceled());
        }

        public void UnregisterToken()
        {
            CancellationSource.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                UnregisterToken();
                Response = null;
                Where = null;
                Response = null;
                TaskCompletionSource = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
