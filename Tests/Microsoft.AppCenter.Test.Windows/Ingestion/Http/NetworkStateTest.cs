// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Test.Utils;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class NetworkStateTest : HttpIngestionTest
    {
        private NetworkStateAdapter _networkState;
        private NetworkStateIngestion _networkStateIngestion;
        private readonly Mock<IApplicationSettings> _settingsMock = new Mock<IApplicationSettings>();

        [TestInitialize]
        public void InitializeNetworkStateTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _networkState = new NetworkStateAdapter();

            var httpIngestion = new IngestionHttp(_adapter.Object);
            _networkStateIngestion = new NetworkStateIngestion(httpIngestion, _networkState);
            AppCenter.Instance = null;
#pragma warning disable 612
            AppCenter.SetApplicationSettingsFactory(new MockApplicationSettingsFactory(_settingsMock));
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.AllowedNetworkRequestsKey, It.IsAny<bool>())).Returns(true);
#pragma warning restore 612
        }

        /// <summary>
        /// Verify that call executed when network is available.
        /// </summary>
        [TestMethod]
        public async Task NetworkStateIngestionOnlineAsync()
        {
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = true;
            var call = _networkStateIngestion.Call(AppSecret, InstallId, Logs);
            await call.ToTask();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }
        
        /// <summary>
        /// Verify that call not executed when network is not available.
        /// </summary>
        [TestMethod]
        public async Task NetworkStateIngestionOffline()
        {
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = false;
            var call = _networkStateIngestion.Call(AppSecret, InstallId, Logs);
            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Never());
        }

        /// <summary>
        /// Verify that call resent when network is available again.
        /// </summary>
        [TestMethod]
        public async Task NetworkStateIngestionComeBackOnline()
        {
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = false;
            var call = _networkStateIngestion.Call(AppSecret, InstallId, Logs);
            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Never());
            _networkState.IsConnected = true;
            await call.ToTask();
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Verify that multiple calls are resent when network is available again.
        /// </summary>
        [TestMethod]
        public async Task NetworkStateIngestionComeBackOnlineMultipleCalls()
        {
            const int CallsCount = 5;
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = false;
            var calls = new List<IServiceCall>();
            for (var i = 0; i < CallsCount; ++i)
            {
                calls.Add(_networkStateIngestion.Call(AppSecret, InstallId, Logs));
            }
            await Task.Delay(TimeSpan.FromSeconds(3));
            Assert.IsFalse(calls.Any(call => call.IsCompleted));
            _networkState.IsConnected = true;
            await Task.WhenAll(calls.Select(call => call.ToTask()));
            VerifyAdapterSend(Times.Exactly(CallsCount));
            calls.ForEach(call => call.Dispose());
        }
    }
}
