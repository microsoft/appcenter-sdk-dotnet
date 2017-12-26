using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http
{
    internal sealed class RetryableIngestion : IngestionDecorator
    {
        private static readonly TimeSpan[] DefaultIntervals =
            {
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMinutes(.5),
                TimeSpan.FromMinutes(20)
            };


        private readonly Random _random = new Random();
        private readonly IDictionary<ServiceCallDecorator, Timer> _calls = new Dictionary<ServiceCallDecorator, Timer>();
        private readonly TimeSpan[] _retryIntervals;

        public RetryableIngestion(IIngestion decoratedApi)
            : this(decoratedApi, DefaultIntervals)
        {
        }

        public RetryableIngestion(IIngestion decoratedApi, TimeSpan[] retryIntervals)
            : base(decoratedApi)
        {
            _retryIntervals = retryIntervals ?? throw new ArgumentNullException(nameof(retryIntervals));
        }

        private Timer IntervalCall(int retry, Action action)
        {
            var interval = (int)(_retryIntervals[retry - 1].TotalMilliseconds / 2.0);
            lock (_random)
            {
                interval += _random.Next(interval);
            }
            AppCenterLog.Warn(AppCenterLog.LogTag, $"Try #{retry} failed and will be retried in {interval} ms");
            return new Timer(state => action(), null, interval, Timeout.Infinite);
        }

        private void RetryCall(ServiceCallDecorator call, int retry)
        {
            var result = base.Call(call.AppSecret, call.InstallId, call.Logs);
            // TODO Cancel result on cancel call
            result.ContinueWith(_ => RetryCallContinuation(call, result, retry + 1));
        }

        private void RetryCallContinuation(ServiceCallDecorator call, IServiceCall result, int retry)
        {
            // Canceled.
            if (call.IsCanceled)
            {
                return;
            }
            if (result.IsCanceled)
            {
                call.Cancel();
                return;
            }

            // Faulted.
            if (result.IsFaulted)
            {
                var isRecoverable = result.Exception is IngestionException ingestionException && ingestionException.IsRecoverable;
                if (!isRecoverable || retry - 1 >= _retryIntervals.Length)
                {
                    call.SetException(result.Exception);
                    return;
                }

                // Shedule next retry.
                var timer = IntervalCall(retry, () =>
                {
                    lock (_calls)
                    {
                        if (!_calls.Remove(call))
                        {
                            return;
                        }
                    }
                    RetryCall(call, retry);
                });
                lock (_calls)
                {
                    _calls.Add(call, timer);
                }
                return;
            }

            // Succeeded.
            call.SetResult(result.Result);
        }

        public override IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
        {
            var call = new ServiceCallDecorator
            {
                AppSecret = appSecret,
                InstallId = installId,
                Logs = logs
            };
            RetryCall(call, 0);
            return call;
        }

        public override void Close()
        {
            CancelAllCalls();
            base.Close();
        }

        public override void Dispose()
        {
            CancelAllCalls();
            base.Dispose();
        }

        private void CancelAllCalls()
        {
            lock (_calls)
            {
                foreach (var call in _calls)
                {
                    call.Key.Cancel();
                    call.Value.Dispose();
                }
                _calls.Clear();
            }
        }
    }
}
