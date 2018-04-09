using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Storage;
using Microsoft.AppCenter.Test.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Channel
{
    using Channel = Microsoft.AppCenter.Channel.Channel;

    [TestClass]
    public class ChannelTest
    {
        private AggregateException _unobservedTaskException;
        private Mock<IIngestion> _mockIngestion;
        private Channel _channel;
        private IStorage _storage;

        private const string ChannelName = "channelName";
        private const int MaxLogsPerBatch = 3;
        private const int MaxParallelBatches = 3;

        // We wait tasks now and don't need wait more
        private const int DefaultWaitTime = 500;

        // Event semaphores for invokation verification
        private const int SendingLogSemaphoreIdx = 0;
        private const int SentLogSemaphoreIdx = 1;
        private const int FailedToSendLogSemaphoreIdx = 2;
        private const int EnqueuingLogSemaphoreIdx = 3;
        private const int FilteringLogSemaphoreIdx = 4;
        private List<SemaphoreSlim> _eventSemaphores;

        public ChannelTest()
        {
            LogSerializer.AddLogType(TestLog.JsonIdentifier, typeof(TestLog));
        }

        [TestInitialize]
        public void InitializeChannelTest()
        {
            _unobservedTaskException = null;
            _mockIngestion = new Mock<IIngestion>();
            SetupIngestionCallSucceed();
            SetChannelWithTimeSpan(TimeSpan.FromSeconds(1));
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            AppCenterLog.Level = LogLevel.Verbose;
        }

        [TestCleanup]
        public void CleanupAppCenterTest()
        {
            EnsureAllTasksAreFinishedInChannel();

            // The UnobservedTaskException will only happen if a Task gets collected by the GC with an exception unobserved
            GC.Collect();
            GC.WaitForPendingFinalizers();
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;

            if (_unobservedTaskException != null)
            {
                throw _unobservedTaskException;
            }
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _unobservedTaskException = e.Exception;
        }

        /// <summary>
        /// Verify that channel is enabled by default
        /// </summary>
        [TestMethod]
        public void ChannelEnabledByDefault()
        {
            Assert.IsTrue(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that disabling channel has the expected effect
        /// </summary>
        [TestMethod]
        public void DisableChannel()
        {
            _channel.SetEnabled(false);

            Assert.IsFalse(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that enabling the channel has the expected effect
        /// </summary>
        [TestMethod]
        public void EnableChannel()
        {
            _channel.SetEnabled(false);
            _channel.SetEnabled(true);

            Assert.IsTrue(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that enqueuing a log passes the same log reference to enqueue event argument
        /// </summary>
        [TestMethod]
        public async Task EnqueuedLogsAreSame()
        {
            var log = new TestLog();
            var sem = new SemaphoreSlim(0);
            _channel.EnqueuingLog += (sender, args) =>
            {
                Assert.AreSame(log, args.Log);
                sem.Release();
            };

            await _channel.EnqueueAsync(log);
            Assert.IsTrue(sem.Wait(DefaultWaitTime));
        }

        /// <summary>
        /// Verify that when a batch reaches its capacity, it is sent immediately
        /// </summary>
        [TestMethod]
        public async Task EnqueueMaxLogs()
        {
            SetChannelWithTimeSpan(TimeSpan.FromHours(1));
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            VerifySendingLog(1);
        }

        /// <summary>
        /// Verify that when channel is disabled, sent event is not triggered
        /// </summary>
        [TestMethod]
        public async Task EnqueueWhileDisabled()
        {
            _channel.SetEnabled(false);
            var log = new TestLog();
            await _channel.EnqueueAsync(log);
            VerifyFailedToSendLog(1);
            VerifyEnqueuingLog(0);
        }

        [TestMethod]
        public async Task ChannelInvokesFilteringLogEvent()
        {
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            VerifyFilteringLog(MaxLogsPerBatch);
        }

        /// <summary>
        /// Validate filtering out a log
        /// </summary>
        [TestMethod]
        public async Task FilterLogShouldNotSend()
        {
            _channel.FilteringLog += (sender, args) => args.FilterRequested = true;
            for (int i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            VerifyFilteringLog(MaxLogsPerBatch);
            VerifySendingLog(0);
            VerifySentLog(0);
        }

        /// <summary>
        /// Validate filters can cancel each other
        /// </summary>
        [TestMethod]
        public async Task FilterLogThenCancelFilterLogInAnotherHandlerShouldSend()
        {
            _channel.FilteringLog += (sender, args) => args.FilterRequested = true;
            _channel.FilteringLog += (sender, args) => args.FilterRequested = false;
            for (int i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            VerifyFilteringLog(MaxLogsPerBatch);
            VerifySendingLog(MaxLogsPerBatch);
            VerifySentLog(MaxLogsPerBatch);
        }

        [TestMethod]
        public async Task ChannelInvokesSendingLogEvent()
        {
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            VerifySendingLog(MaxLogsPerBatch);
        }

        [TestMethod]
        public async Task ChannelInvokesSentLogEvent()
        {
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            VerifySentLog(MaxLogsPerBatch);
        }

        [TestMethod]
        public async Task ChannelInvokesFailedToSendLogEvent()
        {
            SetupIngestionCallFail(new Exception());
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            VerifyFailedToSendLog(MaxLogsPerBatch);
        }

        /// <summary>
        /// Validate that links are same on an error and a log
        /// </summary>
        [TestMethod]
        public void FailedToSendLogEventArgsAreSame()
        {
            var ex = new Exception();
            var log = new TestLog();
            var failedEventLogArgs = new FailedToSendLogEventArgs(log, ex);
            Assert.AreSame(log, failedEventLogArgs.Log);
            Assert.AreSame(ex, failedEventLogArgs.Exception);
        }

        /// <summary>
        /// Validate that channel will send log after enabling
        /// </summary>
        [TestMethod]
        public async Task ChannelInvokesSendingLogEventAfterEnabling()
        {
            await _channel.ShutdownAsync();
            for (int i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }
            _channel.SetEnabled(true);
            VerifySendingLog(MaxLogsPerBatch);
        }

        /// <summary>
        /// Validate that FailedToSendLog calls when channel is disabled
        /// </summary>
        [TestMethod]
        public async Task ChannelInvokesFailedToSendLogEventAfterDisabling()
        {
            _channel.SetEnabled(false);
            for (int i = 0; i < MaxLogsPerBatch; ++i)
            {
                await _channel.EnqueueAsync(new TestLog());
            }

            VerifySendingLog(MaxLogsPerBatch);
            VerifyFailedToSendLog(MaxLogsPerBatch);
        }

        /// <summary>
        /// Validate that all logs removed
        /// </summary>
        [TestMethod]
        public async Task TaskClearLogs()
        {
            await _channel.ShutdownAsync();
            await _channel.EnqueueAsync(new TestLog());

            await _channel.ClearAsync();
            _channel.SetEnabled(true);

            VerifySendingLog(0);
        }

        /// <summary>
        /// Validate that channel's mutex is disposed
        /// </summary>
        [TestMethod]
        public void DisposeChannelTest()
        {
            EnsureAllTasksAreFinishedInChannel();
            _channel.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => _channel.SetEnabled(true));
        }

        /// <summary>
        /// Validate that StorageException is processing without exception
        /// </summary>
        [TestMethod]
        public async Task ThrowStorageExceptionInDeleteLogsTime()
        {
            var log = new TestLog();
            var storageException = new StorageException();

            // Make sure that the storage exception is "observed" to
            // avoid the exception propagating to the point where the
            // test fails.
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                if (e.Exception.InnerException == storageException)
                {
                    e.SetObserved();
                }
            };
            var storage = new Mock<IStorage>();
            storage.Setup(s => s.DeleteLogs(It.IsAny<string>(), It.IsAny<string>())).Throws(storageException);
            storage.Setup(s => s.GetLogsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<Log>>()))
                .Callback((string channelName, int limit, List<Log> logs) => logs.Add(log))
                .Returns(() => Task.FromResult("test-batch-id"));

            var appSecret = Guid.NewGuid().ToString();
            _channel = new Channel(ChannelName, 1, TimeSpan.FromSeconds(1), 1, appSecret, _mockIngestion.Object, storage.Object);
            SetupEventCallbacks();

            // Shutdown channel and store some log
            await _channel.ShutdownAsync();
            await _channel.EnqueueAsync(log);

            _channel.SetEnabled(true);

           VerifySentLog(1);

            // Not throw any exception
        }

        /// <summary>
        /// Verify that when a recoverable http error occurs, ingestion stays open
        /// </summary>
        [TestMethod]
        public async Task IngestionNotClosedOnRecoverableHttpError()
        {
            SetupIngestionCallFail(new RecoverableIngestionException());
            await _channel.EnqueueAsync(new TestLog());

            // wait for SendingLog event
            _eventSemaphores[SendingLogSemaphoreIdx].Wait();
            // wait up to 20 seconds for suspend to finish
            VerifyChannelDisable(TimeSpan.FromSeconds(20));
            _mockIngestion.Verify(ingestion => ingestion.Close(), Times.Never);
        }

        /// <summary>
        /// Verify that if a non-recoverable http error occurs, ingestion is closed
        /// </summary>
        [TestMethod]
        public async Task IngestionClosedOnNonRecoverableHttpError()
        {
            SetupIngestionCallFail(new NonRecoverableIngestionException());
            await _channel.EnqueueAsync(new TestLog());

            // wait up to 20 seconds for suspend to finish
            VerifyChannelDisable(TimeSpan.FromSeconds(20));
            Assert.IsFalse(_channel.IsEnabled);
            _mockIngestion.Verify(ingestion => ingestion.Close(), Times.Once);
        }

        private void SetChannelWithTimeSpan(TimeSpan timeSpan)
        {
            _storage = new MockStorage();
            var appSecret = Guid.NewGuid().ToString();
            _channel = new Channel(ChannelName, MaxLogsPerBatch, timeSpan, MaxParallelBatches,
                appSecret, _mockIngestion.Object, _storage);
            SetupEventCallbacks();
        }

        private void EnsureAllTasksAreFinishedInChannel()
        {
            try
            {
                // We need to wait for a continuation task in Channel constructor 
                // to be executed so we wait until we acquire a lock to let this happen
                bool dummy = _channel.IsEnabled;
            }
            catch (ObjectDisposedException) { }
        }

        private void SetupIngestionCallFail(Exception exception = null)
        {
            _mockIngestion
                .Setup(ingestion => ingestion.Call(
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<IList<Log>>()))
                .Returns((string appSecret, Guid installId, IList<Log> logs) =>
                {
                    var call = new ServiceCall(appSecret, installId, logs);
                    call.SetException(exception);
                    return call;
                });
        }

        private void SetupIngestionCallSucceed(string result = null)
        {
            _mockIngestion
                .Setup(ingestion => ingestion.Call(
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<IList<Log>>()))
                .Returns((string appSecret, Guid installId, IList<Log> logs) =>
                {
                    var call = new ServiceCall(appSecret, installId, logs);
                    call.SetResult(result);
                    return call;
                });
        }

        private void SetupEventCallbacks()
        {
            _eventSemaphores = new List<SemaphoreSlim>
            {
                new SemaphoreSlim(0), new SemaphoreSlim(0), new SemaphoreSlim(0), new SemaphoreSlim(0), new SemaphoreSlim(0)
            };
            _channel.SendingLog += (sender, args) => { _eventSemaphores[SendingLogSemaphoreIdx].Release(); };
            _channel.SentLog += (sender, args) => { _eventSemaphores[SentLogSemaphoreIdx].Release(); };
            _channel.FailedToSendLog += (sender, args) => { _eventSemaphores[FailedToSendLogSemaphoreIdx].Release(); };
            _channel.EnqueuingLog += (sender, args) => { _eventSemaphores[EnqueuingLogSemaphoreIdx].Release(); };
            _channel.FilteringLog += (sender, args) => { _eventSemaphores[FilteringLogSemaphoreIdx].Release(); };
        }

        private void VerifySendingLog(int expectedTimes, int waitTime = DefaultWaitTime) =>
            Assert.AreEqual(expectedTimes, EventWithSemaphoreOccurred(_eventSemaphores[SendingLogSemaphoreIdx], expectedTimes, waitTime),
                $"Failed on verify {nameof(Channel.SendingLog)} event call times");

        private void VerifySentLog(int expectedTimes, int waitTime = DefaultWaitTime) =>
            Assert.AreEqual(expectedTimes, EventWithSemaphoreOccurred(_eventSemaphores[SentLogSemaphoreIdx], expectedTimes, waitTime),
                $"Failed on verify {nameof(Channel.SentLog)} event call times");

        private void VerifyFailedToSendLog(int expectedTimes, int waitTime = DefaultWaitTime) =>
            Assert.AreEqual(expectedTimes, EventWithSemaphoreOccurred(_eventSemaphores[FailedToSendLogSemaphoreIdx], expectedTimes, waitTime),
                $"Failed on verify {nameof(Channel.FailedToSendLog)} event call times");

        private void VerifyEnqueuingLog(int expectedTimes, int waitTime = DefaultWaitTime) =>
            Assert.AreEqual(expectedTimes, EventWithSemaphoreOccurred(_eventSemaphores[EnqueuingLogSemaphoreIdx], expectedTimes, waitTime),
                $"Failed on verify {nameof(Channel.EnqueuingLog)} event call times");

        private void VerifyFilteringLog(int expectedTimes, int waitTime = DefaultWaitTime) =>
            Assert.AreEqual(expectedTimes, EventWithSemaphoreOccurred(_eventSemaphores[FilteringLogSemaphoreIdx], expectedTimes, waitTime),
                $"Failed on verify {nameof(Channel.FilteringLog)} event call times");

        private void VerifyChannelDisable(TimeSpan timeout)
        {
            var task = Task.Run(async () =>
            {
                while (_channel.IsEnabled)
                {
                    await Task.Delay(10);
                }
            });
            Assert.IsTrue(task.Wait(timeout));
        }

        private static int EventWithSemaphoreOccurred(SemaphoreSlim semaphore, int numTimes, int waitTime)
        {
            for (var i = 0; i < numTimes; ++i)
            {
                if (!semaphore.Wait(waitTime))
                {
                    return i;
                }
            }
            return numTimes;
        }
    }
}
