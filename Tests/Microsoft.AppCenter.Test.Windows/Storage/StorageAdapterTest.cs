// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Storage;
using Microsoft.AppCenter.Utils.Files;
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
        private StorageAdapter _adapter;

        // Const for storage data.
        private const string StorageTestChannelName = "storageTestChannelName";
        private const string TableName = "LogEntry";
        private const string ColumnChannelName = "Channel";
        private const string ColumnLogName = "Log";
        private const string ColumnIdName = "Id";
        private const string DatabasePath = "databaseAtRoot.db";

        [TestInitialize]
        public void InitializeStorageTest()
        {
            _adapter = new StorageAdapter();
        }

        [TestMethod]
        public void InitializeStorageCreatesStorageDirectory()
        {
            var adapter = new StorageAdapter();
            Microsoft.AppCenter.Utils.Constants.AppCenterFilesDirectoryPath = Environment.CurrentDirectory;
            Microsoft.AppCenter.Utils.Constants.AppCenterDatabasePath = DatabasePath;
            try
            {
                adapter.Initialize(DatabasePath);
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            Assert.IsTrue(System.IO.File.Exists(DatabasePath));
        }

        [TestMethod]
        public void FaildToOpenDatabaseWhenNameWrong()
        {
            var adapter = new StorageAdapter();
            try
            {
                adapter.Initialize("test://test.txt");
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Failed to open database connection"));
            }
        }

        [TestMethod]
        public void DatabaseDisposed()
        {
            // Prepare data.
            var adapter = new StorageAdapter();
            try
            {
                adapter.Initialize(DatabasePath);
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't have thrown exception");
            }
            try
            {
                // Try get data before database initialize.
                CreateTableHelper(adapter);
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't have thrown exception");
            }
            adapter.Dispose();
            try
            {
                // Try get data before database initialize.
                CreateTableHelper(_adapter);
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.IsFalse(e.Message.Contains("Should have thrown exception"));
            }
        }

        /// <summary>
        /// Verify that database is not initilaze.
        /// </summary>
        [TestMethod]
        public void DatabaseIsNotInitilazeWhenCallCount()
        {
            // Prepare data.
            var exception = new StorageException("The database wasn't initialized.");
            try
            {
                // Try get data before database initialize.
                _adapter.Count(TableName, ColumnChannelName, StorageTestChannelName);
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual(exception.Message, e.Message);
            }
            try
            {
                // Initialize database.
                _adapter.Initialize(DatabasePath);
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            CreateTableHelper(_adapter);
            try
            {
                // Try get data after database initialize.
                _adapter.Count(TableName, ColumnChannelName, StorageTestChannelName);
            }
            catch (Exception e)
            {
                Assert.Fail("Shouldn't have thrown exception");
            }
        }

        [TestMethod]
        public void NotSupportedTypeException()
        {
            // Prepare data.
            try
            {
                _adapter.Initialize(DatabasePath);
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            try
            {
                _adapter.Count(TableName, $"{ColumnChannelName}", true);
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("not supported"));
            }
            
        }

        [TestMethod]
        public void FaildToPrepareDatabaseWhenDelete()
        {
            string whereClause = $"{ColumnChannelName} = 'faild-value'.";
            try
            {
                _adapter.Initialize(DatabasePath);
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            try
            {
                _adapter.Delete(TableName, whereClause);
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
            try
            {
                // Try get data before database initialize.
                _adapter.Delete(TableName, ColumnChannelName, new object[] { StorageTestChannelName });
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual(exception.Message, e.Message);
            }
            try
            {
                // Initialize database.
                _adapter.Initialize(DatabasePath);
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }
            CreateTableHelper(_adapter);
            try
            {
                // Try get data after database initialize.
                _adapter.Delete(TableName, ColumnChannelName, new object[] { StorageTestChannelName });
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
            try
            {
                _adapter.Initialize(DatabasePath);
            }
            catch
            {
                // Handle exception, database is not created with Mock.
            }

            // Create test table.
            CreateTableHelper(_adapter);

            // Insert test data.
            InsertToTableHelper(_adapter);
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
            _adapter.Delete(TableName, ColumnIdName, new object[] { entryId });
            count = _adapter.Count(TableName, ColumnChannelName, StorageTestChannelName);
            Assert.AreEqual(count, 0);
        }

        private void CreateTableHelper(IStorageAdapter adapter)
        {
            string[] tables = new string[] { ColumnIdName, ColumnChannelName, ColumnLogName };
            string[] types = new string[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT NOT NULL", "TEXT NOT NULL" };
            adapter.CreateTable(TableName, tables, types);
        }

        private void InsertToTableHelper(IStorageAdapter adapter)
        {
            adapter.Insert(TableName,
            new[] { ColumnChannelName, ColumnLogName },
            new List<object[]> {
                new object[] {StorageTestChannelName, ""}
            });
        }

        [TestCleanup]
        public void Despose()
        {
            try
            {
                _adapter.Delete(TableName, ColumnChannelName, new object[] { StorageTestChannelName });
                _adapter.Dispose();
                _adapter = null;
            }
            catch (Exception ignore)
            {
                // Handle exception, database is not created with Mock.
            }
        }
    }
}
