using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http
{
    internal sealed class NetworkStateIngestion : IngestionDecorator
    {
        private readonly HashSet<ServiceCallDecorator> _calls = new HashSet<ServiceCallDecorator>();
        private readonly INetworkStateAdapter _networkStateAdapter;

        public NetworkStateIngestion(IIngestion decoratedApi, INetworkStateAdapter networkStateAdapter)
            : base(decoratedApi)
        {
            _networkStateAdapter = networkStateAdapter;
            _networkStateAdapter.NetworkStatusChanged += OnNetworkStateChange;
        }

        private void OnNetworkStateChange(object sender, EventArgs e)
        {
            if (!_networkStateAdapter.IsConnected)
            {
                return;
            }
            var calls = new List<ServiceCallDecorator>();
            lock (_calls)
            {
                calls.AddRange(_calls);
                _calls.Clear();
            }
            foreach (var call in calls)
            {
                var result = base.Call(call.AppSecret, call.InstallId, call.Logs);
                // TODO handle error and cancel
                call.SetResult(result.Result);
            }
        }

        public override IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
        {
            if (_networkStateAdapter.IsConnected)
            {
                return base.Call(appSecret, installId, logs);
            }
            var call = new ServiceCallDecorator
            {
                AppSecret = appSecret,
                InstallId = installId,
                Logs = logs
            };
            lock (_calls)
            {
                _calls.Add(call);
            }
            return call;
        }

        public override void Close()
        {
            CancelAllCalls();
            base.Close();
        }

        public override void Dispose()
        {
            _networkStateAdapter.NetworkStatusChanged -= OnNetworkStateChange;
            CancelAllCalls();
            base.Dispose();
        }

        private void CancelAllCalls()
        {
            lock (_calls)
            {
                foreach (var call in _calls)
                {
                    call.Cancel();
                }
                _calls.Clear();
            }
        }
    }
}
