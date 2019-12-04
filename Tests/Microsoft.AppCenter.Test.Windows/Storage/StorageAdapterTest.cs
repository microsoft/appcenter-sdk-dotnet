// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Storage;
using Microsoft.AppCenter.Utils.Files;
using Microsoft.AppCenter.Windows.Shared.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AppCenter.Test.Storage
{
    [TestClass]
    public class StorageAdapterTest
    {
        private StorageAdapter adapter;

        // Const for storage data.
        private const string StorageTestChannelName = "storageTestChannelName";
        private const string TableName = "LogEntry";
        private const string ColumnChannelName = "Channel";
        private const string ColumnLogName = "Log";
        private const string ColumnIdName = "Id";
        private const string ColumnFakeName1 = "ColumnFakeName1";
        private const string ColumnFakeName2 = "ColumnFakeName2";

        [TestInitialize]
        public void InitializeStorageTest()
        {
            adapter = new StorageAdapter("path/to/database/file.db");
            adapter._databaseDirectory = Mock.Of<Directory>();
        }

        [TestMethod]
        public void InitializeStorageCreatesStorageDirectory()
        {
            var adapter = new StorageAdapter("path/to/database/file.db");

            // Verify that a directory object was created.
            Assert.IsNotNull(adapter._databaseDirectory);

            // Replace the directory with a mock and initialize.
            adapter._databaseDirectory = Mock.Of<Directory>();
            try
            {
                adapter.InitializeStorageAsync().Wait();
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            Mock.Get(adapter._databaseDirectory).Verify(directory => directory.Create());
        }

        [TestMethod]
        public void CreateStorageAdapterDoesNotCreateDirectoryWhenNull()
        {
            var adapter = new StorageAdapter("databaseAtRoot.db");

            // Verify that a directory object was not created.
            Assert.IsNull(adapter._databaseDirectory);

            // Should not crash even if directory is null.
            adapter.InitializeStorageAsync().Wait();
        }

        [TestMethod]
        public void CreateStorageAdapterExceptionIsWrapped()
        {
            var adapter = new StorageAdapter("path/to/database/file.db")
            {
                _databaseDirectory = Mock.Of<Directory>()
            };
            var sourceException = new System.IO.PathTooLongException();

            // Mock the directory to throw when created.
            Mock.Get(adapter._databaseDirectory).Setup(directory => directory.Create()).Throws(sourceException);
            const string databaseDirectory = "databaseDirectory";
            var databasePath = System.IO.Path.Combine(databaseDirectory, "database.db");
            Exception actualException = null;
            try
            {
                adapter.InitializeStorageAsync().Wait();
            }
            catch (AggregateException ex)
            {
                actualException = ex.InnerException;
            }
            Assert.IsInstanceOfType(actualException, typeof(StorageException));
            Assert.IsInstanceOfType(actualException?.InnerException, typeof(System.IO.PathTooLongException));
        }

        /// <summary>
        /// Verify that database is not initilaze.
        /// </summary>
        [TestMethod]
        public void DatabaseIsNotInitilazeWhenCallCount()
        {
            // Prepare data.
            var exception = new StorageException("The database wasn't initialized.");
            string whereClause = $"{ColumnChannelName} = \"{StorageTestChannelName}\"";
            try
            {
                // Try get data before database initialize.
                adapter.CountAsync(TableName, whereClause).RunNotAsync();
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual(exception.Message, e.Message);
            }
            try
            {
                // Initialize database.
                adapter.InitializeStorageAsync().Wait();
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            CreateTableHelper();
            try
            {
                // Try get data after database initialize.
                adapter.CountAsync(TableName, whereClause).RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't have thrown exception");
            }
        }

        [TestMethod]
        public void FaildToPrepareDatabaseWhenCount()
        {
            // Prepare data.
            string whereClause = $"{ColumnChannelName} = 'faild-value'.";
            try
            {
                adapter.InitializeStorageAsync().Wait();
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            try
            {
                adapter.CountAsync(TableName, whereClause).RunNotAsync();
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Failed to prepare SQL query"));
            }
        }

        [TestMethod]
        public void FaildToPrepareDatabaseWhenDelete()
        {
            string whereClause = $"{ColumnChannelName} = 'faild-value'.";
            try
            {
                adapter.InitializeStorageAsync().Wait();
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            try
            {
                adapter.DeleteAsync(TableName, whereClause).RunNotAsync();
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Failed to prepare SQL query"));
            }
        }

        [TestMethod]
        public void DatabaseIsNotInitilazeWhenCallDelete()
        {
            // Prepare data.
            var exception = new StorageException("The database wasn't initialized.");
            string whereClause = $"{ColumnChannelName} = \"{StorageTestChannelName}\"";
            try
            {
                // Try get data before database initialize.
                adapter.DeleteAsync(TableName, whereClause).RunNotAsync();
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual(exception.Message, e.Message);
            }
            try
            {
                // Initialize database.
                adapter.InitializeStorageAsync().Wait();
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            CreateTableHelper();
            try
            {
                // Try get data after database initialize.
                adapter.DeleteAsync(TableName, whereClause).RunNotAsync();
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't have thrown exception");
            }
        }

        [TestMethod]
        public void CreateTable()
        {
            // Prepare data.
            var whereClause = $"{ColumnChannelName} = \'{StorageTestChannelName}\'";
            try
            {
                adapter.InitializeStorageAsync().Wait();
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }

            // Create test table.
            CreateTableHelper();

            // Insert test data.
            InsertToTableHelper();

            // Verify.
            var testEntries = adapter.GetAsync(TableName, whereClause, 100).GetAwaiter().GetResult();
            Assert.AreEqual(testEntries.Count, 1);
            var entryId = 0;
            testEntries.ForEach(entry => {
                entryId = (int) entry[0];
                Assert.AreEqual(entry[1], StorageTestChannelName);
                Assert.AreEqual(entry[2], "");
                Assert.AreEqual(entry[3], 1);
                Assert.AreEqual(entry[4], 1);
            });
            var count = adapter.DeleteAsync(TableName, $"{ColumnIdName} = {entryId}").GetAwaiter().GetResult();
            Assert.AreEqual(count, 1);
        }

        private void CreateTableHelper()
        {
            var scheme = new List<ColumnMap>
            {
                new ColumnMap { ColumnName = ColumnIdName, ColumnType = raw.SQLITE_INTEGER, IsAutoIncrement = true, IsPrimaryKey = true },
                new ColumnMap { ColumnName = ColumnChannelName, ColumnType = raw.SQLITE_TEXT, IsAutoIncrement = false, IsPrimaryKey = false },
                new ColumnMap { ColumnName = ColumnLogName, ColumnType = raw.SQLITE_TEXT, IsAutoIncrement = false, IsPrimaryKey = false },
                new ColumnMap { ColumnName = ColumnFakeName1, ColumnType = raw.SQLITE_INTEGER, IsAutoIncrement = false, IsPrimaryKey = false },
                new ColumnMap { ColumnName = ColumnFakeName2, ColumnType = raw.SQLITE_INTEGER, IsAutoIncrement = false, IsPrimaryKey = false }
            };
            adapter.CreateTableAsync(TableName, scheme).Wait();
        }

        private void InsertToTableHelper()
        {
            var columnsMapList = new List<List<ColumnValueMap>>(){ new List<ColumnValueMap>()
            {
                new ColumnValueMap() { ColumnName = ColumnIdName, ColumnValue = 100, ColumnType = raw.SQLITE_INTEGER },
                new ColumnValueMap() { ColumnName = ColumnChannelName, ColumnValue = StorageTestChannelName, ColumnType = raw.SQLITE_TEXT },
                new ColumnValueMap() { ColumnName = ColumnLogName, ColumnValue = "", ColumnType = raw.SQLITE_TEXT },
                new ColumnValueMap() { ColumnName = ColumnFakeName1, ColumnValue = 1, ColumnType = raw.SQLITE_INTEGER },
                new ColumnValueMap() { ColumnName = ColumnFakeName2, ColumnValue = 1, ColumnType = raw.SQLITE_INTEGER },
            }};
            adapter.InsertAsync(TableName, columnsMapList).Wait();
        }

        [TestCleanup]
        public void Despose()
        {
            try
            {
                adapter.DeleteDatabaseFileAsync().Wait();
                adapter = null;
            }
            catch (Exception ignore)
            {
                // Handle exception, database is not created with Mock.
            }
        }
    }
}
