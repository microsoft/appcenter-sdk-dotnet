// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using SQLitePCL;

namespace Microsoft.AppCenter.Storage
{
    internal class StorageAdapter : IStorageAdapter
    {
        sqlite3 _db;
        internal Directory _databaseDirectory;
        private readonly string _databasePath;
        private static int SqliteConfigurationResult = raw.SQLITE_ERROR;

        public StorageAdapter(string databasePath)
        {
            _databasePath = databasePath;
            var databaseDirectoryPath = System.IO.Path.GetDirectoryName(databasePath);
            if (databaseDirectoryPath != string.Empty)
            {
                _databaseDirectory = new Directory(databaseDirectoryPath);
            }
        }

        public async Task CreateTableAsync (string tableName, Dictionary<string, string> scheme)
        {
            //todo tableName
            int result = CreateTable(_db, tableName, scheme);
            if (result != raw.SQLITE_OK)
            {
                // todo throw
            }
            else if (result == raw.SQLITE_DONE)
            {

            }
            else
            {
                // todo throw
            }
        }

        private int CreateTable(sqlite3 db, string tableName, Dictionary<string, string> scheme)
        {
            return executeQuery(db, String.Format("CREATE TABLE \"{0}\" (\"{1}\")", tableName, string.Join(",", scheme)));
        }

        private int executeQuery(sqlite3 db, string query)
        {
            sqlite3_stmt stmt;
            // todo
            int result = raw.sqlite3_prepare_v2(db, query, out stmt);
            if (result != raw.SQLITE_OK)
            {
                return result;
            }
            result = raw.sqlite3_step(stmt);
            raw.sqlite3_finalize(stmt);
            return result;
        }

        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> pred, int limit) where T : new()
        {
            try
            {
                var table = _dbConnection.Table<T>();
                return await table.Where(pred).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            catch (SQLiteException e)
            {
                throw ToStorageException(e);
            }
        }


        public Task<int> CountAsync<T>(Expression<Func<T, bool>> pred) where T : new()
        {
            var table = _dbConnection.Table<T>();
            return table.Where(pred).CountAsync();
        }

        public void InsertAsync(string tableName, Dictionary<string, string> scheme)
        {
            try
            {
                Insert(_db, tableName, scheme);
            }
            catch (SQLiteException e)
            {
                throw ToStorageException(e);
            }
        }

        private int Insert(sqlite3 db, string tableName, Dictionary<string, string> scheme)
        {
            return executeQuery(db, String.Format("INSERT INTO \"{0}\" (\"{1}\")", tableName, string.Join(",", scheme)));
        }

        private static StorageException ToStorageException(SQLiteException e)
        {
            return new StorageException($"SQLite errorCode={e.Result}", e);
        }

        public async void DeleteAsync<T>(string tableName, string columnName, List<int> values)
        {
            try
            {
                Delete(db, tableName, columnName, values);
            }
            catch (SQLiteException e)
            {
                throw ToStorageException(e);
            }
        }

        private int Delete(sqlite3 db, string tableName, string columnName, List<int> values)
        {
            var numDeleted = 0;
            foreach (var val in values)
            {
                int result = executeQuery(db, String.Format("DELETE FROM \"{0}\" WHERE {1} = {2}", tableName, columnName, val));
                if (result == raw.SQLITE_DONE)
                {
                    numDeleted++;
                }
            }
            return numDeleted;
        }

        public Task InitializeStorageAsync()
        {
            return Task.Run(() =>
            {
                // Create the directory in case it does not exist.
                if (_databaseDirectory != null)
                {
                    try
                    {
                        _databaseDirectory.Create();
                    }
                    catch (Exception e)
                    {
                        throw new StorageException("Cannot initialize SQLite library.", e);
                    }
                }

                // In SQLite-net 1.5.231 constructor parameters were changed.
                // Using reflection to accept newer library version.
                _dbConnection = (SQLiteAsyncConnection)typeof(SQLiteAsyncConnection)
                    .GetConstructor(new[] { typeof(string), typeof(bool) })
                    ?.Invoke(new object[] { _databasePath, true });
                if (_dbConnection == null)
                {
                    _dbConnection = (SQLiteAsyncConnection)typeof(SQLiteAsyncConnection)
                        .GetConstructor(new[] { typeof(string), typeof(bool), typeof(object) })
                        ?.Invoke(new object[] { _databasePath, true, null });
                }
                if (_dbConnection == null)
                {
                    throw new StorageException("Cannot initialize SQLite library.");
                }
            });
        }

        public Task DeleteDatabaseFileAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    // We can't delete the file and recreate without invalidating the connection pool.
                    // This is explained in details at https://chrisriesgo.com/sqlite-net-async-connections-keep-it-clean/.
                    SQLiteAsyncConnection.ResetPool();
                    var prefix = _databaseDirectory == null ? Constants.LocalAppData : "";
                    new File(System.IO.Path.Combine(prefix, _databasePath)).Delete();
                }
                catch (Exception e)
                {
                    throw new StorageException(e);
                }
            });
        }
    }
}
