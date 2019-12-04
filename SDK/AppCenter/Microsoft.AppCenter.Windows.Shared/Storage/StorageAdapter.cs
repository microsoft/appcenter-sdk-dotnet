// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLitePCL;
using Microsoft.AppCenter.Windows.Shared.Storage;

namespace Microsoft.AppCenter.Storage
{
    internal class StorageAdapter : IStorageAdapter
    {
        private const string RawTextTypeName = "TEXT";
        private const string RawFloatTypeName = "FLOAT";
        private const string RawIntegerTypeName = "INTEGER";
        private const string RawAutoincrementSuffix = "AUTOINCREMENT";
        private const string RawPrimaryKeySuffix = "PRIMARY KEY";

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

        private int SqlQueryCreateTable(sqlite3 db, string tableName, List<ColumnMap> scheme)
        {
            var columnsList = new List<string>();
            foreach (var column in scheme)
            {
                var stringBuilder = new StringBuilder($"{column.ColumnName} ");
                switch (column.ColumnType)
                {
                    case raw.SQLITE_TEXT:
                        stringBuilder.Append(RawTextTypeName);
                        break;
                    case raw.SQLITE_INTEGER:
                        stringBuilder.Append(RawIntegerTypeName);
                        break;
                    case raw.SQLITE_FLOAT:
                        stringBuilder.Append(RawFloatTypeName);
                        break;
                }
                if (column.IsPrimaryKey)
                {
                    stringBuilder.Append(RawPrimaryKeySuffix);
                }
                if (column.IsAutoIncrement)
                {
                    stringBuilder.Append(RawAutoincrementSuffix);
                }
                columnsList.Add(stringBuilder.ToString());
            }
            var tableClause = string.Join(",", columnsList.ToArray());
            var queryString = $"CREATE TABLE IF NOT EXISTS {tableName} ({tableClause});";
            return ExecuteNonSelectionSqlQuery(db, queryString);
        }
        
        public Task CreateTableAsync(string tableName, List<ColumnMap> columnMaps)
        {
            return Task.Run(() =>
            {
                var result = SqlQueryCreateTable(_db, tableName, columnMaps);
                if (result != raw.SQLITE_DONE)
                {
                    var errMsg = raw.sqlite3_errmsg(_db);
                    throw new StorageException($"Failed to create table, result={result}\n\t{errMsg}");
                }
            });

        }

        private int ExecuteNonSelectionSqlQuery(sqlite3 db, string query)
        {
            if (db == null)
            {
                throw new StorageException("The database wasn't initialized.");
            }
            var result = raw.sqlite3_prepare_v2(db, query, out var stmt);
            if (result != raw.SQLITE_OK)
            {
                var errMsg = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to prepare SQL query, result={result}\n\t{errMsg}");
            }
            result = raw.sqlite3_step(stmt);
            raw.sqlite3_finalize(stmt);
            return result;
        }

        private List<List<object>> ExecuteSelectionSqlQuery(sqlite3 db, string query)
        {
            if (db == null)
            {
                throw new StorageException("The database wasn't initialized.");
            }
            var entries = new List<List<object>>();
            var queryResult = raw.sqlite3_prepare_v2(db, query, out var stmt);
            if (queryResult != raw.SQLITE_OK)
            {
                var errMsg = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to prepare SQL query, result={queryResult}\n\t{errMsg}");
            }
            while (raw.sqlite3_step(stmt) == raw.SQLITE_ROW)
            {
                var entry = new List<object>();
                var count = raw.sqlite3_column_count(stmt);
                for (var i = 0; i < count; i++)
                {
                    var typeCol = raw.sqlite3_column_type(stmt, i);
                    object valCol;
                    switch (typeCol)
                    {
                        case raw.SQLITE_FLOAT:
                            valCol = raw.sqlite3_column_double(stmt, i);
                            break;
                        case raw.SQLITE_INTEGER:
                            valCol = raw.sqlite3_column_int(stmt, i);
                            break;
                        case raw.SQLITE_TEXT:
                            valCol = raw.sqlite3_column_text(stmt, i);
                            break;
                        default:
                            valCol = null;
                            break;
                    }
                    entry.Add(valCol);
                }
                if (entry.Count > 0)
                {
                    entries.Add(entry);
                }
            }
            raw.sqlite3_finalize(stmt);
            return entries;
        }

        public Task<List<List<object>>> GetAsync(string tableName, string whereClause, int? limit = null)
        {
            var limitClause = limit != null ? $" LIMIT {limit}" : string.Empty;
            var query = $"SELECT * FROM {tableName} WHERE {whereClause}{limitClause};";
            return Task.FromResult(ExecuteSelectionSqlQuery(_db, query));
        }

        public Task<int> CountAsync(string tableName, string whereClause)
        {
            return Task.FromResult((int)ExecuteSelectionSqlQuery(_db,
                $"SELECT COUNT(*) FROM {tableName} WHERE {whereClause};")[0][0]);
        }

        private int SqlQueryInsert(sqlite3 db, string tableName, string columnsClause, string valuesClause)
        {
            return ExecuteNonSelectionSqlQuery(db, $"INSERT INTO {tableName}{columnsClause} VALUES {valuesClause};");
        }

        public Task<int> InsertAsync(string tableName, List<List<ColumnValueMap>> valueMaps)
        {
            var stringValues = new List<string>();
            var columnsHashSet = new HashSet<string>();
            foreach (var entry in valueMaps)
            {
                var stringValue = string.Join(",", entry.Select(x =>
                {
                    columnsHashSet.Add(x.ColumnName);
                    if (x.ColumnType == raw.SQLITE_TEXT)
                    {
                        return $"\'{x.ColumnValue}\'";
                    }
                    return x.ColumnValue;
                }));
                stringValues.Add($"({stringValue})");
            }
            var valuesClause = string.Join(",", stringValues);
            var columnsClause = $"({string.Join(",", columnsHashSet)})";
            return Task.FromResult(SqlQueryInsert(_db, tableName, columnsClause, valuesClause));
        }

        private int SqlQueryDelete(sqlite3 db, string tableName, string whereClause)
        {
            var numDeleted = (int)ExecuteSelectionSqlQuery(db,
                $"SELECT COUNT(*) FROM {tableName} WHERE {whereClause};")[0][0];
            var result = ExecuteNonSelectionSqlQuery(db, $"DELETE FROM {tableName} WHERE {whereClause};");
            if (result != raw.SQLITE_DONE && result != raw.SQLITE_OK)
            {
                var errMsg = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to delete SQL query, result={result}\n\t{errMsg}");
            }
            return numDeleted;
        }

        public Task<int> DeleteAsync(string tableName, string whereClause)
        {
            return Task.FromResult(SqlQueryDelete(_db, tableName, whereClause));
        }

        public Task InitializeStorageAsync()
        {
            return Task.Run(() =>
            {
                if (_databaseDirectory != null)
                {
                    try
                    {
                        _databaseDirectory.Create();
                    }
                    catch (Exception e)
                    {
                        throw new StorageException("Failed to open database connection.", e);
                    }
                }
                var result = raw.SQLITE_ERROR;
                try
                {
                    raw.SetProvider(new SQLite3Provider_e_sqlite3());
                    result = raw.sqlite3_open(_databasePath, out _db);
                }
                catch (Exception e)
                {
                    throw new StorageException("Failed to open database connection.", e);
                }
                if (result != raw.SQLITE_OK)
                {
                    var errMsg = raw.sqlite3_errmsg(_db);
                    throw new StorageException($"Failed to open database connection, result={result}\n\t{errMsg}");
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
