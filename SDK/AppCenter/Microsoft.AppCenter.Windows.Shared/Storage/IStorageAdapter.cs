// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.AppCenter.Storage
{
    public interface IStorageAdapter : IDisposable
    {
        /// <summary>
        /// Initialize this storage adapter.
        /// </summary>
        void Initialize(string databasePath);

        /// <summary>
        /// Create table in database.
        /// </summary>
        /// <param name="tableName">Name of a table to create.</param>
        /// <param name="columnNames">Array with columns' mappings.</param>
        /// /// <param name="columnTypes">Array with columns' mappings.</param>
        void CreateTable(string tableName, string[] columnNames, string[] columnTypes);

        /// <summary>
        /// Returns amount of rows matching given criteria.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns>Amount of rows matching given criteria.</returns>
        int Count(string tableName, string columnName, object value);

        /// <summary>
        /// Returns collection of columns with values matching given criteria.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="whereMask"></param>
        /// <param name="limit">Parameters' mask.</param>
        /// <param name="args">Arguments to be bound to the mask.</param>
        /// <returns>IList of columns with values matching given criteria</returns>
        IList<object[]> Select(string tableName, string columnName, object value, string excludeColumnName, object[] excludeValues, int? limit = null);

        /// <summary>
        /// Inserts data to table.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="columnNames">Array of columns used to insert data.</param>
        /// <param name="values">IList of columns with values.</param>
        void Insert(string tableName, string[] columnNames, ICollection<object[]> values);

        /// <summary>
        /// Deletes rows matching given criteria.
        /// </summary>
        /// <param name="tableName">Name of a table to run query on.</param>
        /// <param name="columnName">Name of column to check value by.</param>
        /// <param name="values">Values to match.</param>
        void Delete(string tableName, string columnName, params object[] values);
    }
}
