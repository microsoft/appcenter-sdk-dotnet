// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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
    public class RetryableTest : HttpIngestionTest
    {
        private static readonly TimeSpan[] Intervals =
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1) 
        };
        private IIngestion _retryableIngestion;
        private readonly Mock<IApplicationSettings> _settingsMock = new Mock<IApplicationSettings>();

        [TestInitialize]
        public void InitializeRetryableTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _retryableIngestion = new RetryableIngestion(new IngestionHttp(_adapter.Object), Intervals);
            AppCenter.Instance = null;
#pragma warning disable 612
            AppCenter.SetApplicationSettingsFactory(new MockApplicationSettingsFactory(_settingsMock));
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.AllowedNetworkRequestsKey, It.IsAny<bool>())).Returns(true);
#pragma warning restore 612
        }

        /// <summary>
        /// Verify behaviour without exceptions.
        /// </summary>
        [TestMethod]
        public async Task RetryableIngestionSuccess()
        {
            SetupAdapterSendResponse(HttpStatusCode.OK);
            var call = _retryableIngestion.Call(AppSecret, InstallId, Logs);
            await call.ToTask();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }

        /// <summary>
        /// Verify that retrying on recoverable exceptions.
        /// </summary>
        [TestMethod]
        public async Task RetryableIngestionRepeat1()
        {

            // RequestTimeout - retryable
            SetupAdapterSendResponse(HttpStatusCode.RequestTimeout, HttpStatusCode.OK);
            var start = DateTime.Now;
            var call = _retryableIngestion.Call(AppSecret, InstallId, Logs);
            await Task.Delay(start.AddSeconds(0.5) - DateTime.Now);
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(1));

            await Task.Delay(start.AddSeconds(1.5) - DateTime.Now);
            Assert.IsTrue(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(2));
        }

        /// <summary>
        /// Verify that retrying on recoverable exceptions.
        /// </summary>
        [TestMethod]
        public async Task RetryableIngestionRepeat3()
        {

            // RequestTimeout - retryable
            SetupAdapterSendResponse(HttpStatusCode.RequestTimeout, HttpStatusCode.RequestTimeout, HttpStatusCode.RequestTimeout, HttpStatusCode.OK);
            var start = DateTime.Now;
            var call = _retryableIngestion.Call(AppSecret, InstallId, Logs);
            await Task.Delay(start.AddSeconds(0.5) - DateTime.Now);
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(1));

            await Task.Delay(start.AddSeconds(1.5) - DateTime.Now);
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(2));

            await Task.Delay(start.AddSeconds(2.5) - DateTime.Now);
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(3));

            await Task.Delay(start.AddSeconds(3.5) - DateTime.Now);
            Assert.IsTrue(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(4));
        }

        /// <summary>
        /// Verify service call canceling.
        /// </summary>
        [TestMethod]
        public async Task RetryableIngestionCancel()
        {

            // RequestTimeout - retryable
            SetupAdapterSendResponse(HttpStatusCode.RequestTimeout);
            var start = DateTime.Now;
            var call = _retryableIngestion.Call(AppSecret, InstallId, Logs);
            await Task.Delay(start.AddSeconds(0.5) - DateTime.Now);
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(1));

            await Task.Delay(start.AddSeconds(1.5) - DateTime.Now);
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(2));

            call.Cancel();

            await Task.Delay(start.AddSeconds(2.5) - DateTime.Now);
            Assert.IsFalse(call.IsCompleted);
            VerifyAdapterSend(Times.Exactly(2));
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => call.ToTask());
        }

        /// <summary>
        /// Verify that not retrying not recoverable exceptions.
        /// </summary>
        [TestMethod]
        public async Task RetryableIngestionException()
        {
            SetupAdapterSendResponse(HttpStatusCode.BadRequest);
            var call = _retryableIngestion.Call(AppSecret, InstallId, Logs);
            await Assert.ThrowsExceptionAsync<HttpIngestionException>(() => call.ToTask());
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Validate that constructor throws correct exception type with nullable timespan array
        /// </summary>
        [TestMethod]
        public void RetryableIngestionWithNullIntervals()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RetryableIngestion(new IngestionHttp(_adapter.Object), null));
        }
    }
}
