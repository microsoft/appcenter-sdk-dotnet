using System;

namespace Microsoft.AppCenter.Ingestion
{
    public interface IServiceCall : IDisposable
    {
        bool IsCanceled { get; }
        bool IsCompleted { get; }
        bool IsFaulted { get; }

        string Result { get; }
        Exception Exception { get; }

        void ContinueWith(Action<IServiceCall> continuationAction);

        /// <summary>
        /// Cancel the call if possible.
        /// </summary>
        void Cancel();
    }
}
