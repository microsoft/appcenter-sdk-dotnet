// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.Functional
{
     internal class ExpectedData : IDisposable
    {
        internal HttpResponse Response { get; set; }
        internal Func<RequestData, bool> Where { get; set; }
        private TaskCompletionSource<RequestData> _taskCompletionSource { get; set; }
        private CancellationTokenSource _cancellationSource { get; set; }

        internal ExpectedData(TimeSpan tokenTimeout)
        {
            _taskCompletionSource = new TaskCompletionSource<RequestData>();

            // Since we can't directly assign token to a Task created with FromResult,
            // and we can't work with token in SendAsync because it may or may not be called after this method,
            // we are registering anonymous cancellation token here.
            // The only weak point can occur if there's a significant delay between actual Task creation time and calling this method,
            // but since we only use that in tests, that can be ignored.
            _cancellationSource = new CancellationTokenSource(tokenTimeout);
            _cancellationSource.Token.Register(() => _taskCompletionSource.TrySetCanceled());
        }

        public bool TrySetResult(RequestData requestData)
        {
            return _taskCompletionSource.TrySetResult(requestData);
        }

        public Task<RequestData> GetTask()
        {
            return _taskCompletionSource.Task;
        }

        public void UnregisterToken()
        {
            _cancellationSource.Dispose();
        }

        public void Dispose()
        {
            UnregisterToken();
        }
    }
}
