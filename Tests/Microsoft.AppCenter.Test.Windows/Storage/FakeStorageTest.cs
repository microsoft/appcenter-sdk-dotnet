// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test
{
    [TestClass]
    public class FakeStorageTest
    {
        private const string TableName = "LogEntry";
        private const string StorageTestChannelName = "storageTestChannelName";

        /// <summary>
        /// Verify that shutdown fails when tasks exceed time limit
        /// </summary>
        [TestMethod]
        public void ShutdownTimeout()
        {
            var mockConnection = new Mock<IStorageAdapter>();
            mockConnection.Setup(
                    c => c.Insert(TableName, It.IsAny<string[]>(), It.IsAny<List<object[]>>()))
                    .Callback(() => Task.Delay(TimeSpan.FromDays(1)).Wait());
            var storage = new Microsoft.AppCenter.Storage.Storage(mockConnection.Object, It.IsAny<string>());

            // Ignore warnings because we just want to "fire and forget"
#pragma warning disable 4014
            storage.PutLog(StorageTestChannelName, new TestLog());
            storage.PutLog(StorageTestChannelName, new TestLog());
#pragma warning restore 4014

            var result = storage.ShutdownAsync(TimeSpan.FromTicks(1)).RunNotAsync();
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verify that shutdown is successful when tasks complete in time
        /// </summary>
        [TestMethod]
        public void ShutdownSucceed()
        {
            var mockConnection = new Mock<IStorageAdapter>();
            mockConnection.Setup(
                    c => c.Insert(TableName, It.IsAny<string[]>(), It.IsAny<List<object[]>>()))
                .Callback(() => Task.Delay(TimeSpan.FromSeconds(2)).Wait());
            var storage = new Microsoft.AppCenter.Storage.Storage(mockConnection.Object, It.IsAny<string>());

            // Ignore warnings because we just want to "fire and forget"
#pragma warning disable 4014
            storage.PutLog(StorageTestChannelName, new TestLog());
            storage.PutLog(StorageTestChannelName, new TestLog());
#pragma warning restore 4014

            var result = storage.ShutdownAsync(TimeSpan.FromSeconds(100)).RunNotAsync();
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verify that new tasks are not started after shutdown
        /// </summary>
        [TestMethod]
        public void ShutdownPreventsNewTasks()
        {
            var mockConnection = new Mock<IStorageAdapter>();
            var storage = new Microsoft.AppCenter.Storage.Storage(mockConnection.Object, It.IsAny<string>());
            var result = storage.ShutdownAsync(TimeSpan.FromSeconds(10)).RunNotAsync();
            Assert.IsTrue(result);
            Assert.ThrowsException<StorageException>(
                () => storage.GetLogsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<Log>>()).RunNotAsync());
        }

        /// <summary>
        /// Verify that if an error occurs while executing a query (from GetLogs), a StorageException gets thrown.
        /// </summary>
        [TestMethod]
        public void GetLogsQueryError()
        {
            var mockAdapter = new Mock<IStorageAdapter>();
            mockAdapter.Setup(
                    a => a.Select(TableName, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<int?>()))
                    .Throws(new StorageException());
            var fakeStorage = new Microsoft.AppCenter.Storage.Storage(mockAdapter.Object, It.IsAny<string>());
            var logs = new List<Log>();
            Assert.ThrowsException<StorageException>(() =>
                fakeStorage.GetLogsAsync(StorageTestChannelName, 1, logs).RunNotAsync());
        }

        /// <summary>
        /// Verify that storage throws StorageException if something went wrong
        /// </summary>
        [TestMethod]
        public void StorageThrowsStorageException()
        {
            var mockAdapter = new Mock<IStorageAdapter>();
            mockAdapter.Setup(
                    a => a.Select(TableName, It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<int?>()))
                .Throws(new StorageException());
            mockAdapter.Setup(c => c.Insert(TableName, It.IsAny<string[]>(), It.IsAny<IList<object[]>>()))
                .Throws(new StorageException());
            mockAdapter.Setup(c => c.Delete(TableName, It.IsAny<string>()))
                 .Throws(new StorageException());
            mockAdapter.Setup(c => c.Count(TableName, It.IsAny<string>(), It.IsAny<object>()))
                 .Throws(new StorageException());
            var fakeStorage = new Microsoft.AppCenter.Storage.Storage(mockAdapter.Object, It.IsAny<string>());
            Assert.ThrowsException<StorageException>(() => fakeStorage.PutLog(StorageTestChannelName, new TestLog()).RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.DeleteLogs(StorageTestChannelName, string.Empty).RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.CountLogsAsync(StorageTestChannelName).RunNotAsync());
            Assert.ThrowsException<StorageException>(() => fakeStorage.GetLogsAsync(StorageTestChannelName, 1, new List<Log>()).RunNotAsync());
        }
    }
}
