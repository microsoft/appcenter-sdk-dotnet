using System;
using System.Threading;

namespace Microsoft.AppCenter.Ingestion
{
    internal class ServiceCall : IServiceCall
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public bool IsCanceled { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsFaulted { get; private set; }

        public string Result { get; private set; }
        public Exception Exception { get; private set; }

        public CancellationToken CancellationToken => _tokenSource.Token;
        
        public void ContinueWith(Action<IServiceCall> continuationAction)
        {
            // TODO
        }

        public void SetResult(string result)
        {
            // TODO
        }

        public void SetException(Exception exception)
        {
            // TODO
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
        }
    }
}
