// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using SQLitePCL;

namespace Microsoft.AppCenter.Storage
{
    internal class StorageAdapter : IStorageAdapter
    {
        private sqlite3 _db;
        internal Directory _databaseDirectory;
        private readonly string _databasePath;

        public StorageAdapter(string databasePath)
        {
            _databasePath = databasePath;
            var databaseDirectoryPath = System.IO.Path.GetDirectoryName(databasePath);
            if (databaseDirectoryPath != string.Empty)
            {
                _databaseDirectory = new Directory(databaseDirectoryPath);
            }
        }

        private static StorageException ErrorCodeToRawSQLite3ConstName(int resultCode)
        {
            foreach (FieldInfo field in typeof(Constants).GetFields().Where(f => (f.Name.StartsWith("SQLITE_") && (int)f.GetValue(null) == resultCode)))
            {
                return new StorageException($"SQLite errorCode={resultCode} ({field.Name})");
            }
            return new StorageException(($"SQLite errorCode={resultCode}"));
        }

        private int SQLCreateTable(sqlite3 db, string tableName, Dictionary<string, string> scheme)
        {
            return ExecuteNonReturningSQLQuery(db, String.Format("CREATE TABLE \"{0}\" (\"{1}\")", tableName, string.Join(",", scheme)));
        }

        public Task CreateTableAsync(string tableName, Dictionary<string, string> scheme)
        {
            //todo tableName
            return Task.Run(() =>
            {
                int result = SQLCreateTable(_db, tableName, scheme);
                if (result != raw.SQLITE_DONE)
                {
                    throw new StorageException($"Failed to create table: {ErrorCodeToRawSQLite3ConstName(result)} ({result})");
                }
            });
            
        }

        private int ExecuteNonReturningSQLQuery(sqlite3 db, string query)
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
        private int ExecuteReturningSQLQuery(sqlite3 db, string query)
        {
            using (sqlite3 ssdb = ugly.open(":memory:"))
            {
                sqlite3_stmt stmt = ssdb.prepare("CREATE TABLE foo (x int)");
                stmt.step();
            }
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
        
        private int SQLCount(sqlite3 db, string tableName, string columnName, List<string> values)
        {
            return ExecuteNonReturningSQLQuery(db, String.Format("SELECT COUNT(*) FROM {0} WHERE {1} IN ({2})", tableName, columnName, string.Join(",", values)));
        }

        public Task<int> CountAsync(string tableName, string columnName, List<int> values)
        {
            
            return table.Where(pred).CountAsync();
        }

        private int SQLInsert(sqlite3 db, string tableName, Dictionary<string, string> scheme)
        {
            return ExecuteNonReturningSQLQuery(db, String.Format("INSERT INTO \"{0}\" (\"{1}\")", tableName, string.Join(",", scheme)));
        }

        public Task<int> InsertAsync(string tableName, Dictionary<string, string> scheme)
        {
            try
            {
                return Task.FromResult(SQLInsert(_db, tableName, scheme));
            }
            catch (SQLiteException e)
            {
                throw ToStorageException(e);
            }
        }

        private static StorageException ToStorageException(SQLiteException e)
        {
            return new StorageException($"SQLite errorCode={e.Result}", e);
        }

        private int SQLDelete(sqlite3 db, string tableName, string columnName, List<int> values)
        {
            var numDeleted = 0;
            foreach (var val in values)
            {
                int result = ExecuteNonReturningSQLQuery(db, String.Format("DELETE FROM \"{0}\" WHERE {1} = {2}", tableName, columnName, val));
                if (result == raw.SQLITE_DONE)
                {
                    numDeleted++;
                }
            }
            return numDeleted;
        }

        public Task<int> DeleteAsync(string tableName, string columnName, List<int> values)
        {
            try
            {
                return Task.FromResult(SQLDelete(_db, tableName, columnName, values));
            }
            catch (SQLiteException e)
            {
                throw ToStorageException(e);
            }
        }

        public Task InitializeStorageAsync()
        {
            return Task.Run(() =>
            {
                raw.SetProvider(new SQLite3Provider_e_sqlite3());
                if (raw.sqlite3_initialize() != raw.SQLITE_OK)
                {
                    throw new StorageException("Failed to initialize SQLite library");
                }
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
                if (raw.sqlite3_open(_databasePath, out _db) != raw.SQLITE_OK)
                {
                    throw new StorageException("Failed to open database connection");
                }
            });
        }

        public Task DeleteDatabaseFileAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    raw.sqlite3_close(_db);
                    _db.Dispose();
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
