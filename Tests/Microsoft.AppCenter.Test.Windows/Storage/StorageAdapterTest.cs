// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AppCenter.Test.Windows.Storage
{
    [TestClass]
    public class StorageAdapterTest
    {
        private StorageAdapter _adapter;

        // Constants data mocks.
        private const string StorageTestChannelName = "storageTestChannelName";
        private const string TableName = "LogEntry";
        private const string ColumnChannelName = "Channel";
        private const string ColumnLogName = "Log";
        private const string ColumnIdName = "Id";
        private const string DatabasePath = "databaseAtRoot.db";

        [TestInitialize]
        public void TestInitialize()
        {
            Microsoft.AppCenter.Utils.Constants.AppCenterDatabasePath = DatabasePath;
            try
            {
                System.IO.File.Delete(DatabasePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail! {0}", e.Message);
            }
            _adapter = new StorageAdapter();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                _adapter.Dispose();
                _adapter = null;
            }
            catch (Exception e)
            {
                Assert.Fail("Failed to dispose storage adapter: {0}", e.Message);
            }
            try
            {
                System.IO.File.Delete(DatabasePath);
            }
            catch
            {
                // Db file might not exist or might fail to be deleted.
            }
        }

        /// <summary>
        /// Verify that database file is created when Initialize() is called.
        /// </summary>
        [TestMethod]
        public void CreateDbDiskImageOnInitialization()
        {
            InitializeStorageAdapter();
        }

        /// <summary>
        /// Verify that Initialize would fail with incorrect db path passed.
        /// </summary>
        [TestMethod]
        public void FailOnOpenDatabaseWithWrongName()
        {
            var exceptionThrown = false;
            try
            {
                _adapter.Initialize("test://test.txt");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Failed to open database connection"));
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        /// <summary>
        /// Verify that storage adapter is disposed when calling Dispose().
        /// </summary>
        [TestMethod]
        public void StorageAdapterDisposedOnDisposeCall()
        {
            InitializeStorageAdapter();
            _adapter.Dispose();
            var exceptionThrown = false;
            try
            {
                CreateTable();
            }
            catch
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        /// <summary>
        /// Verify that database is not initialized.
        /// </summary>
        [TestMethod]
        public void DatabaseIsNotInitializedWhenCallCount()
        {
            // Prepare data.
            var exceptionThrown = false;
            try
            {
                // Try to get data before database initialization.
                _adapter.Count(TableName, ColumnChannelName, StorageTestChannelName);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
                Assert.AreEqual("The database wasn't initialized.", e.Message);
            }
            Assert.IsTrue(exceptionThrown);
            InitializeStorageAdapter();
            CreateTable();
            _adapter.Count(TableName, ColumnChannelName, StorageTestChannelName);
        }

        /// <summary>
        /// Verify that we throw NotSupportedException when passing unsupported value for binding.
        /// </summary>
        [TestMethod]
        public void NotSupportedTypeException()
        {
            // Prepare data.
            InitializeStorageAdapter();
            CreateTable();
            InsertMockDataToTable();
            var exceptionThrown = false;
            try
            {
                _adapter.Count(TableName, $"{ColumnChannelName}", true);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
                Assert.IsInstanceOfType(e, typeof(NotSupportedException));
                Assert.AreEqual("Type System.Boolean not supported.", e.Message);
            }
            Assert.IsTrue(exceptionThrown);
            exceptionThrown = false;
            try
            {
                _adapter.Count(TableName, $"{ColumnChannelName}", 0.42d);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
                Assert.IsInstanceOfType(e, typeof(NotSupportedException));
                Assert.AreEqual("Type System.Double not supported.", e.Message);
            }
            Assert.IsTrue(exceptionThrown);
        }

        /// <summary>
        /// Verify that exception is thrown on trying to prepare incorrect delete SQL query.
        /// </summary>
        [TestMethod]
        public void FailToPrepareIncorrectDeleteQuery()
        {
            var exceptionThrown = false;
            string whereClause = $"{ColumnChannelName} = 'field-value'.";
            InitializeStorageAdapter();
            try
            {
                _adapter.Delete(TableName, whereClause);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
                Assert.IsTrue(e.Message.Contains("Failed to prepare SQL query"));
            }
            Assert.IsTrue(exceptionThrown);
        }

        /// <summary>
        /// Verify that Delete query only works after storage adapter initialization and table creation, and not before.
        /// </summary>
        [TestMethod]
        public void DeleteFailsBeforeStorageAdapterInitAndTablePrepare()
        {
            // Prepare data.
            var exceptionThrown = false;
            const string exceptionMessage = "The database wasn't initialized.";
            try
            {
                // Try get data before database initialize.
                _adapter.Delete(TableName, ColumnChannelName, StorageTestChannelName);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
                Assert.AreEqual(exceptionMessage, e.Message);
            }
            Assert.IsTrue(exceptionThrown);
            InitializeStorageAdapter();
            CreateTable();
            _adapter.Delete(TableName, ColumnChannelName, StorageTestChannelName);
        }

        /// <summary>
        /// Verify that table is creating and inserting to table work.
        /// </summary>
        [TestMethod]
        public void CreateTableAndInsertWorking()
        {
            // Prepare data.
            InitializeStorageAdapter();

            // Create test table.
            CreateTable();

            // Insert test data.
            InsertMockDataToTable();
            var count = _adapter.Count(TableName, ColumnChannelName, StorageTestChannelName);
            Assert.AreEqual(1, count);

            // Verify.
            var testEntries = _adapter.Select(TableName, ColumnChannelName, StorageTestChannelName, null, null).ToList();
            Assert.AreEqual(1, testEntries.Count);
            var entryId = 0L;
            testEntries.ForEach(entry =>
            {
                entryId = (long)entry[0];
                Assert.AreEqual(entry[1], StorageTestChannelName);
                Assert.AreEqual(entry[2], "");
            });
            _adapter.Delete(TableName, ColumnIdName, entryId);
            count = _adapter.Count(TableName, ColumnChannelName, StorageTestChannelName);
            Assert.AreEqual(0, count);
        }

        #region Helper methods

        private void CreateTable()
        {
            var tables = new[] { ColumnIdName, ColumnChannelName, ColumnLogName };
            var types = new[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT NOT NULL", "TEXT NOT NULL" };
            _adapter.CreateTable(TableName, tables, types);
        }

        private void InsertMockDataToTable()
        {
            _adapter.Insert(TableName,
                new[] { ColumnChannelName, ColumnLogName },
                new List<object[]>
                {
                    new object[] {StorageTestChannelName, ""}
                }
            );
        }

        private void InitializeStorageAdapter()
        {
            Assert.IsFalse(System.IO.File.Exists(DatabasePath));
            try
            {
                _adapter.Initialize(DatabasePath);
            }
            catch (Exception e)
            {
                Assert.Fail("Failed to initialize storage adapter: {0}", e.Message);
            }
            Assert.IsTrue(System.IO.File.Exists(DatabasePath));
        }

        #endregion
    }
}
