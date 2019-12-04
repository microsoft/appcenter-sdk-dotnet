// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AppCenter.Windows.Shared.Storage;
using SQLitePCL;

namespace Microsoft.AppCenter.Storage
{
    internal class StorageAdapter : IStorageAdapter
    {
        private sqlite3 _db;
        internal Directory _databaseDirectory;
        
        public void CreateTable(string tableName, string[] columnNames, string[] columnTypes)
        {
            var tableClause = string.Join(",", Enumerable.Range(0, columnNames.Length).Select(i => $"{columnNames[i]} {columnTypes[i]}"));
            var result = ExecuteNonSelectionSqlQuery(_db, $"CREATE TABLE IF NOT EXISTS {tableName} ({tableClause});");
            if (result != raw.SQLITE_DONE)
            {
                var errorMessage = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to create table, result={result}\n\t{errorMessage}");
            }
        }

        private void BindSqlParams(sqlite3_stmt stmt, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case string stringValue:
                        raw.sqlite3_bind_text(stmt, i + 1, stringValue);
                        break;
                    case int intVal:
                        raw.sqlite3_bind_int(stmt, i + 1, intVal);
                        break;
                    default:
                        // TODO log
                        break;
                }
            }
        }

        private int ExecuteNonSelectionSqlQuery(sqlite3 db, string query, params object[] args)
        {
            if (db == null)
            {
                throw new StorageException("The database wasn't initialized.");
            }
            var result = raw.sqlite3_prepare_v2(db, query, out var stmt);
            BindSqlParams(stmt, args);
            if (result != raw.SQLITE_OK)
            {
                var errorMessage = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to prepare SQL query, result={result}\n\t{errorMessage}");
            }
            result = raw.sqlite3_step(stmt);
            raw.sqlite3_finalize(stmt);
            return result;
        }

        private List<object[]> ExecuteSelectionSqlQuery(sqlite3 db, string query, params object[] args)
        {
            if (db == null)
            {
                throw new StorageException("The database wasn't initialized.");
            }
            var entries = new List<object[]>();
            var queryResult = raw.sqlite3_prepare_v2(db, query, out var stmt);
            BindSqlParams(stmt, args);
            if (queryResult != raw.SQLITE_OK)
            {
                var errorMessage = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to prepare SQL query, result={queryResult}\n\t{errorMessage}");
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
                        case raw.SQLITE_INTEGER:
                            valCol = raw.sqlite3_column_int(stmt, i);
                            break;
                        case raw.SQLITE_TEXT:
                            valCol = raw.sqlite3_column_text(stmt, i);
                            break;
                        default:
                            // TODO log
                            valCol = null;
                            break;
                    }
                    entry.Add(valCol);
                }
                if (entry.Count > 0)
                {
                    entries.Add(entry.ToArray<object>());
                }
            }
            raw.sqlite3_finalize(stmt);
            return entries;
        }

        public IList<object[]> Select(string tableName, string whereClause, int? limit = null, params object[] args)
        {
            var limitClause = limit != null ? $" LIMIT {limit}" : string.Empty;
            var query = $"SELECT * FROM {tableName} WHERE {whereClause}{limitClause};";
            return ExecuteSelectionSqlQuery(_db, query, args);
        }

        public int Count(string tableName, string columnName, object value)
        {
            var result = ExecuteSelectionSqlQuery(_db, $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = ?;", value);
            return (int)result[0][0];
        }

        public void Insert(string tableName, string[] columnNames, IList<object[]> values)
        {
            var columnsClause = string.Join(",", columnNames);
            var valueClause = string.Join(",", Enumerable.Repeat("?", values[0].Length));
            var valuesClause = string.Join(",", Enumerable.Repeat($"({valueClause})", values.Count));
            var valuesArray = values.SelectMany(i => i).ToArray();
            var result = ExecuteNonSelectionSqlQuery(_db, $"INSERT INTO {tableName}({columnsClause}) VALUES {valuesClause};", valuesArray);
            if (result != raw.SQLITE_OK)
            {
                var errorMessage = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to delete SQL query, result={result}\n\t{errorMessage}");
            }
        }

        public void Delete(string tableName, string columnName, params object[] values)
        {
            var whereMask = $"{columnName} IN ({string.Join(",", Enumerable.Repeat("?", values.Length))})";
            var result = ExecuteNonSelectionSqlQuery(_db, $"DELETE FROM {tableName} WHERE {whereMask};", values);
            if (result != raw.SQLITE_OK)
            {
                var errorMessage = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to delete SQL query, result={result}\n\t{errorMessage}");
            }
        }

        public void Initialize(string databasePath)
        {
            int result;
            try
            {
                raw.SetProvider(new SQLite3Provider_e_sqlite3());
                result = raw.sqlite3_open(databasePath, out _db);
            }
            catch (Exception e)
            {
                throw new StorageException("Failed to open database connection.", e);
            }
            if (result != raw.SQLITE_OK)
            {
                var errorMessage = raw.sqlite3_errmsg(_db);
                throw new StorageException($"Failed to open database connection, result={result}\n\t{errorMessage}");
            }
        }

        public void Dispose()
        {
            raw.sqlite3_close(_db);
            _db.Dispose();
        }
    }
}
