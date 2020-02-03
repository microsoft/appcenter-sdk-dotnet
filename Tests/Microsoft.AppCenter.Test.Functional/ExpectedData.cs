// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.Functional
{
    internal class ExpectedData : IDisposable
    {
        private readonly TaskCompletionSource<RequestData> _taskCompletionSource = new TaskCompletionSource<RequestData>();
        private readonly CancellationTokenSource _cancellationSource;

        public HttpResponse Response { get; set; }
        
        public Func<RequestData, bool> Where { get; set; }
        
        public Task<RequestData> Task => _taskCompletionSource.Task;

        internal ExpectedData(TimeSpan tokenTimeout)
        {
            // Since we can't directly assign token to a Task created with FromResult,
            // and we can't work with token in SendAsync because it may or may not be called after this method,
            // we are registering anonymous cancellation token here.
            // The only weak point can occur if there's a significant delay between actual Task creation time and calling this method,
            // but since we only use that in tests, that can be ignored.
            _cancellationSource = new CancellationTokenSource(tokenTimeout);
            _cancellationSource.Token.Register(() => _taskCompletionSource.TrySetCanceled());
        }

        public void SetResult(RequestData requestData)
        {
            _taskCompletionSource.TrySetResult(requestData);
        }

        public void Dispose()
        {
            _cancellationSource.Dispose();
        }
    }
}
