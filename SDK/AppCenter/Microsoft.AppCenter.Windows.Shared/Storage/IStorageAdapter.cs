// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Storage
{
    /// <summary>
    /// Interface to work with database storage.
    /// </summary>
    public interface IStorageAdapter : IDisposable
    {
        /// <summary>
        /// Initialize this storage adapter.
        /// </summary>
        /// <param name="databasePath">Path of a database file to connect to.</param>
        void Initialize(string databasePath);

        /// <summary>
        /// Create table in database.
        /// </summary>
        /// <param name="tableName">Name of a table to create.</param>
        /// <param name="columnNames">Array with columns' names.</param>
        /// <param name="columnTypes">Array with columns' type descriptions.</param>
        void CreateTable(string tableName, string[] columnNames, string[] columnTypes);

        /// <summary>
        /// Returns amount of rows matching given criteria.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="columnName">Column to match value by.</param>
        /// <param name="value">Value to match in a query.</param>
        /// <returns>Amount of rows matching given criteria.</returns>
        int Count(string tableName, string columnName, object value);

        /// <summary>
        /// Returns collection of columns with values matching given criteria.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="columnName">Name of a column to match value.</param>
        /// <param name="value">Value to match in a query.</param>
        /// <param name="excludeColumnName">Column name to match excluded values by.</param>
        /// <param name="excludeValues">Excluded values to match in query.</param>
        /// <param name="limit">Maximum amount of items to select.</param>
        /// <returns>Item list with array of objects. Array of objects is object[] representation of columns.</returns>
        IList<object[]> Select(string tableName, string columnName, object value, string excludeColumnName, object[] excludeValues, int? limit = null, string[] orderList = null);

        /// <summary>
        /// Inserts data to table.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="columnNames">Array of columns used to insert data.</param>
        /// <param name="values">ICollection of columns with values.</param>
        void Insert(string tableName, string[] columnNames, ICollection<object[]> values);

        /// <summary>
        /// Deletes rows matching given criteria.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="columnName">Name of column to match value by.</param>
        /// <param name="values">Array of values to match in a query.</param>
        void Delete(string tableName, string columnName, params object[] values);

        /// <summary>
        /// Set the maximum size of the storage.
        /// </summary>
        /// <remarks>
        /// This only sets the maximum size of the database, but App Center modules might store additional data.
        /// The value passed to this method is not persisted on disk. The default maximum database size is 10485760 bytes (10 MiB).
        /// </remarks>
        /// <param name="sizeInBytes">
        /// Maximum size of the storage in bytes. This will be rounded up to the nearest multiple of a SQLite page size (default is 4096 bytes).
        /// Values below 20,480 bytes (20 KiB) will be ignored.
        /// </param>
        /// <returns><code>true</code> if changing the size was successful.</returns>
        bool SetMaxStorageSize(long sizeInBytes);

        /// <summary>
        /// Gets the maximum size of the database.
        /// </summary>
        /// <returns>The maximum size of database in bytes.</returns>
        long GetMaxStorageSize();

        /// <summary>
        /// Gets the current size of the database.
        /// </summary>
        /// <returns>The current size of database in bytes.</returns>
        long GetCurrentStorageSize();
    }
}
