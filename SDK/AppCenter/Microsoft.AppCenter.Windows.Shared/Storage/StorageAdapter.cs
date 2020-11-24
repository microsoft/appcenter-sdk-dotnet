// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLitePCL;

namespace Microsoft.AppCenter.Storage
{
    internal class StorageAdapter : IStorageAdapter
    {
        private sqlite3 _db;

        static StorageAdapter()
        {
            try
            {
                Batteries_V2.Init();
            }
            catch (Exception e)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Failed to initialize sqlite3 provider.", e);
            }
        }

        public void Initialize(string databasePath)
        {
            var result = raw.sqlite3_open(databasePath, out _db);
            if (result != raw.SQLITE_OK)
            {
                throw ToStorageException(result, "Failed to open database connection");
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        private void BindParameter(sqlite3_stmt stmt, int index, object value)
        {
            int result;
            if (value is string)
            {
                result = raw.sqlite3_bind_text(stmt, index, (string)value);
            }
            else if (value is int)
            {
                result = raw.sqlite3_bind_int(stmt, index, (int)value);
            }
            else if (value is long)
            {
                result = raw.sqlite3_bind_int64(stmt, index, (long)value);
            }
            else
            {
                throw new NotSupportedException($"Type {value.GetType().FullName} not supported.");
            }
            if (result != raw.SQLITE_OK)
            {
                throw ToStorageException(result, $"Failed to bind {index} parameter");
            }
        }

        private void BindParameters(sqlite3_stmt stmt, IList<object> values)
        {
            for (var i = 0; i < values?.Count; i++)
            {
                // Parameters in statement are 1-based. See https://www.sqlite.org/c3ref/bind_blob.html
                BindParameter(stmt, i + 1, values[i]);
            }
        }

        private object GetColumnValue(sqlite3_stmt stmt, int index)
        {
            var columnType = raw.sqlite3_column_type(stmt, index);
            switch (columnType)
            {
                case raw.SQLITE_INTEGER:
                    return raw.sqlite3_column_int64(stmt, index);
                case raw.SQLITE_TEXT:
                    return raw.sqlite3_column_text(stmt, index).utf8_to_string();
            }
            AppCenterLog.Error(AppCenterLog.LogTag, $"Attempt to get unsupported column value {columnType}.");
            return null;
        }

        private int ExecuteNonSelectionSqlQuery(string query, IList<object> args = null)
        {
            var db = _db ?? throw new StorageException("The database wasn't initialized.");
            var result = raw.sqlite3_prepare_v2(db, query, out var stmt);
            if (result != raw.SQLITE_OK)
            {
                throw ToStorageException(result, "Failed to prepare SQL query");
            }
            try
            {
                BindParameters(stmt, args);
                result = raw.sqlite3_step(stmt);
                if (result != raw.SQLITE_DONE)
                {
                    throw ToStorageException(result, "Failed to run query");
                }
            }
            finally
            {
                result = raw.sqlite3_finalize(stmt);
                if (result != raw.SQLITE_OK)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, $"Failed to finalize statement, result={result}");
                }
            }
            return result;
        }

        private List<object[]> ExecuteSelectionSqlQuery(string query, IList<object> args = null)
        {
            var db = _db ?? throw new StorageException("The database wasn't initialized.");
            var result = raw.sqlite3_prepare_v2(db, query, out var stmt);
            if (result != raw.SQLITE_OK)
            {
                throw ToStorageException(result, "Failed to prepare SQL query");
            }
            try
            {
                var entries = new List<object[]>();
                BindParameters(stmt, args);
                while (raw.sqlite3_step(stmt) == raw.SQLITE_ROW)
                {
                    var count = raw.sqlite3_column_count(stmt);
                    entries.Add(Enumerable.Range(0, count).Select(i => GetColumnValue(stmt, i)).ToArray());
                }
                return entries;
            }
            finally
            {
                result = raw.sqlite3_finalize(stmt);
                if (result != raw.SQLITE_OK)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, $"Failed to finalize statement, result={result}");
                }
            }
        }

        private long GetMaxPageCount()
        {
            return GetPragmaValue("max_page_count");
        }

        private long GetPageCount()
        {
            return GetPragmaValue("page_count");
        }

        private long GetPageSize()
        {
            return GetPragmaValue("page_size");
        }

        private long GetPragmaValue(string valueName)
        {
            var result = ExecuteSelectionSqlQuery($"PRAGMA {valueName};");
            var count = (long)(result.FirstOrDefault()?.FirstOrDefault() ?? 0L);
            return count;
        }

        public long GetMaxStorageSize()
        {
            try
            {
                return GetMaxPageCount() * GetPageSize();
            }
            catch (StorageException)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, $"Could not get max storage size.");
                return -1;
            }
        }

        public bool SetMaxStorageSize(long sizeInBytes)
        {
            bool success;
            var db = _db ?? throw new StorageException("The database wasn't initialized.");

            // Check the current number of pages in the database to determine whether the requested size will shrink the database.
            var currentPageCount = GetPageCount();
            var pageSize = GetPageSize();
            AppCenterLog.Info(AppCenterLog.LogTag, $"Found {currentPageCount} pages in the database.");
            var requestedMaxPageCount = Convert.ToBoolean(sizeInBytes % pageSize) ? sizeInBytes / pageSize + 1 : sizeInBytes / pageSize;
            if (currentPageCount > requestedMaxPageCount)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, $"Cannot change database size to {sizeInBytes} bytes as it would cause a loss of data. " +
                    "Maximum database size will not be changed.");
                success = false;
            }
            else
            {
                // Attempt to set the limit and check the page count to make sure the given limit works.
                var result = raw.sqlite3_exec(db, $"PRAGMA max_page_count = {requestedMaxPageCount};");
                if (result != raw.SQLITE_OK)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, $"Could not change maximum database size to {sizeInBytes} bytes. SQLite error code: {result}.");
                    success = false;
                }
                else
                {
                    var currentMaxPageCount = GetMaxPageCount();
                    var actualMaxSize = currentMaxPageCount * pageSize;
                    if (requestedMaxPageCount != currentMaxPageCount)
                    {
                        AppCenterLog.Error(AppCenterLog.LogTag, $"Could not change maximum database size to {sizeInBytes} bytes, current maximum size is {actualMaxSize} bytes.");
                        success = false;
                    }
                    else
                    {
                        if (sizeInBytes == actualMaxSize)
                        {
                            AppCenterLog.Info(AppCenterLog.LogTag, $"Changed maximum database size to {actualMaxSize} bytes.");
                        }
                        else
                        {
                            AppCenterLog.Info(AppCenterLog.LogTag, $"Changed maximum database size to {actualMaxSize} bytes (next multiple of 4KiB).");
                        }
                        success = true;
                    }
                }
            }
            return success;
        }

        public void CreateTable(string tableName, string[] columnNames, string[] columnTypes)
        {
            var tableClause = string.Join(",", Enumerable.Range(0, columnNames.Length).Select(i => $"{columnNames[i]} {columnTypes[i]}"));
            ExecuteNonSelectionSqlQuery($"CREATE TABLE IF NOT EXISTS {tableName} ({tableClause});");
        }

        public int Count(string tableName, string columnName, object value)
        {
            var result = ExecuteSelectionSqlQuery($"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = ?;", new[] { value });
            var count = (long)(result.FirstOrDefault()?.FirstOrDefault() ?? 0L);
            return (int)count;
        }

        public IList<object[]> Select(string tableName, string columnName, object value, string excludeColumnName, object[] excludeValues, int? limit = null, string[] orderList = null)
        {
            var whereClause = $"{columnName} = ?";
            var args = new List<object> { value };
            if (excludeValues?.Length > 0)
            {
                whereClause += $" AND {excludeColumnName} NOT IN ({BuildBindingMask(excludeValues.Length)})";
                args.AddRange(excludeValues);
            }
            var limitClause = limit != null ? $" LIMIT {limit}" : string.Empty;
            var orderClause = orderList != null && orderList.Length > 0 ? $" ORDER BY {string.Join(",", orderList)} ASC" : string.Empty;
            var query = $"SELECT * FROM {tableName} WHERE {whereClause}{orderClause}{limitClause};";
            return ExecuteSelectionSqlQuery(query, args);
        }

        public void Insert(string tableName, string[] columnNames, ICollection<object[]> values)
        {
            var columnsClause = string.Join(",", columnNames);
            var valueClause = $"({BuildBindingMask(values.First().Length)})";
            var valuesClause = string.Join(",", Enumerable.Repeat(valueClause, values.Count));
            var valuesArray = values.SelectMany(i => i).ToArray();
            ExecuteNonSelectionSqlQuery($"INSERT INTO {tableName}({columnsClause}) VALUES {valuesClause};", valuesArray);
        }

        public void Delete(string tableName, string columnName, params object[] values)
        {
            var whereMask = $"{columnName} IN ({BuildBindingMask(values.Length)})";
            ExecuteNonSelectionSqlQuery($"DELETE FROM {tableName} WHERE {whereMask};", values);
        }

        private StorageException ToStorageException(int result, string message)
        {
            var errorMessage = raw.sqlite3_errmsg(_db).utf8_to_string();
            var exceptionMessage = $"{message}, result={result}\n\t{errorMessage}";
            switch(result)
            {
                case raw.SQLITE_CORRUPT:
                case raw.SQLITE_NOTADB:
                    return new StorageCorruptedException(exceptionMessage);
                case raw.SQLITE_FULL:
                    return new StorageFullException(exceptionMessage);
                default:
                    return new StorageException(exceptionMessage);
            }
        }

        private static string BuildBindingMask(int amount)
        {
            return string.Join(",", Enumerable.Repeat("?", amount));
        }
    }
}
