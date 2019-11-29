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
using Microsoft.AppCenter.Windows.Shared.Storage;

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

        private int SQLCreateTable(sqlite3 db, string tableName, List<ColumnMap> scheme)
        {
            var queryString = $"CREATE TABLE IF NOT EXISTS {tableName} (";
            foreach (var column in scheme)
            {
                queryString += $"{column.ColumnName} {column.ColumnType} ";
                if (column.IsPrimarykey)
                {
                    queryString += "PRIMARY KEY ";
                }
                if (column.IsAutoIncrement)
                {
                    queryString += "AUTOINCREMENT ";
                }
            }
            queryString += ");";
            return ExecuteNonSelectionSqlQuery(db, queryString);
        }

        public Task CreateTableAsync(string tableName, List<ColumnMap> scheme)
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

        private int ExecuteNonSelectionSqlQuery(sqlite3 db, string query)
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

        private List<Dictionary<string, object>> ExecuteSelectionSqlLQuery(sqlite3 db, string query)
        {
            sqlite3_stmt stmt;
            List<Dictionary<string, object>> resultQuery = new List<Dictionary<string, object>>();
            int result = raw.sqlite3_prepare_v2(db, query, out stmt);
            while (result == raw.SQLITE_ROW)
            {
                while (raw.sqlite3_step(stmt) == raw.SQLITE_OK)
                {
                    Dictionary<string, object> rowData = new Dictionary<string, object>();
                    var count = raw.sqlite3_column_count(stmt);
                    for (var i = 0; i < count; i++)
                    {
                        var nameCol = raw.sqlite3_column_table_name(stmt, i);
                        var typeCol = raw.sqlite3_column_type(stmt, i);
                        object valCol;
                        switch (typeCol)
                        {
                            case (int)SqlDataType.SQLITE_FLOAT:
                                valCol = raw.sqlite3_column_double(stmt, i);
                                break;
                            case (int)SqlDataType.SQLITE_INTEGER:
                                valCol = raw.sqlite3_column_int(stmt, i);
                                break;
                            case (int)SqlDataType.SQLITE_TEXT:
                                valCol = raw.sqlite3_column_text(stmt, i);
                                break;
                            default:
                                valCol = null;
                                break;
                        }
                        rowData.Add(nameCol, valCol);
                    }
                    resultQuery.Add(rowData);
                }
            }
            raw.sqlite3_finalize(stmt);
            return resultQuery;
        }

        public async Task<List<Dictionary<string, object>>> GetAsync(string tableName, Dictionary<string, object> scheme, string pred = "AND", int? limit = null)
        {
            var queryCondition = string.Join($" {pred} ", scheme.Select(x => x.Key + " IN " + x.Value).ToList());
            if ( limit != null)
            {
                queryCondition += $" LIMIT {limit}";
            }
            var queryString = String.Format("SELECT * FROM {0} WHERE {1}", tableName, queryCondition);
            return ExecuteSelectionSqlLQuery(_db, queryString);
        }

        private async Task<int> ExecuteCountSqlQuery(sqlite3 db, string tableName, Dictionary<string, object> scheme, string pred)
        {
            var queryCondition = string.Join($" {pred} ", scheme.Select(x => x.Key + " IN " + x.Value).ToList());
            return ExecuteNonSelectionSqlQuery(db, String.Format("SELECT COUNT(*) FROM {0} WHERE {1}", tableName, queryCondition));
        }

        public Task<int> CountAsync(string tableName, Dictionary<string, object> scheme, string pred)
        {
            return ExecuteCountSqlQuery(_db, tableName, scheme, pred);
        }

        private int SQLInsert(sqlite3 db, string tableName, Dictionary<string, object> scheme)
        {
            var queryCondition = string.Join(",", scheme.Select(x => x.Key + " IN " + x.Value).ToList());
            return ExecuteNonSelectionSqlQuery(db, String.Format("INSERT INTO \"{0}\" (\"{1}\")", tableName, queryCondition));
        }

        public Task<int> InsertAsync(string tableName, Dictionary<string, object> scheme)
        {

            return Task.FromResult(SQLInsert(_db, tableName, scheme));
        }

        private static StorageException ToStorageException(int erroeCode)
        {
            return new StorageException($"SQLite errorCode={ErrorCodeToRawSQLite3ConstName(erroeCode)}");
        }

        private int SQLDelete(sqlite3 db, string tableName, string columnName, List<object> values)
        {
            var numDeleted = 0;
            foreach (var val in values)
            {
                int result = ExecuteNonSelectionSqlQuery(db, String.Format("DELETE FROM \"{0}\" WHERE {1} = {2}", tableName, columnName, val));
                if (result == raw.SQLITE_DONE)
                {
                    numDeleted++;
                }
            }
            return numDeleted;
        }


        public Task<int> DeleteAsync(string tableName, string columnName, List<object> values)
        {
            return Task.FromResult(SQLDelete(_db, tableName, columnName, values));
        }

        private int SQLDeleteWithPredicate(sqlite3 db, string tableName, Dictionary<string, object> values, string pred)
        {
            var queryCondition = string.Join($" {pred} ", values.Select(x => x.Key + " IN " + x.Value).ToList());
            return ExecuteNonSelectionSqlQuery(db, String.Format("DELETE FROM \"{0}\" WHERE {1}", tableName, values));
        }

        public Task<int> DeleteAsync(string tableName, Dictionary<string, object> values, string pred)
        {
            return Task.FromResult(SQLDeleteWithPredicate(_db, tableName, values, pred));
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
