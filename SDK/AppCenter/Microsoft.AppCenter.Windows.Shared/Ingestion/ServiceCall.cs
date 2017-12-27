using System;
using System.Threading;

namespace Microsoft.AppCenter.Ingestion
{
    internal class ServiceCall : IServiceCall
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Action<IServiceCall> _continuationAction;
        private readonly object _lock = new object();

        public bool IsCanceled => CancellationToken.IsCancellationRequested;
        public bool IsCompleted { get; private set; }
        public bool IsFaulted => Exception != null;

        public string Result { get; private set; }
        public Exception Exception { get; private set; }

        public CancellationToken CancellationToken => _tokenSource.Token;
        
        public void ContinueWith(Action<IServiceCall> continuationAction)
        {
            lock (_lock)
            {
                if (!IsCompleted && !IsCanceled)
                {
                    _continuationAction += continuationAction;
                    return;
                }
            }

            // If completed or canceled call it right now.
            continuationAction(this);
        }

        public void CopyState(IServiceCall source)
        {
            if (source.IsCanceled)
            {
                Cancel();
                return;
            }
            if (source.IsFaulted)
            {
                SetException(source.Exception);
                return;
            }
            SetResult(source.Result);
        }

        public void SetResult(string result)
        {
            Action<IServiceCall> continuationAction;
            lock (_lock)
            {
                if (IsCompleted || IsCanceled)
                {
                    return;
                }
                IsCompleted = true;
                Result = result;
                continuationAction = _continuationAction;
                _continuationAction = null;
            }

            // Invoke the continuations.
            continuationAction?.Invoke(this);
        }

        public void SetException(Exception exception)
        {
            Action<IServiceCall> continuationAction;
            lock (_lock)
            {
                if (IsCompleted || IsCanceled)
                {
                    return;
                }
                IsCompleted = true;
                Exception = exception;
                continuationAction = _continuationAction;
                _continuationAction = null;
            }

            // Invoke the continuations.
            continuationAction?.Invoke(this);
        }

        public void Cancel()
        {
            Action<IServiceCall> continuationAction;
            lock (_lock)
            {
                if (IsCompleted || IsCanceled)
                {
                    return;
                }
                _tokenSource.Cancel();
                continuationAction = _continuationAction;
                _continuationAction = null;
            }

            // Invoke the continuations.
            continuationAction?.Invoke(this);
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
        }
    }
}
