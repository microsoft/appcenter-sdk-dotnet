// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Windows.Shared.Storage;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Storage
{
    public interface IStorageAdapter
    {
        Task InitializeStorageAsync();
        Task<List<List<object>>> GetAsync(string tableName, string whereClause, int limit);
        Task CreateTableAsync(string tableName, List<ColumnMap> columnMaps);
        Task<int> CountAsync(string tableName, string whereClause);
        Task<int> InsertAsync(string tableName, List<List<ColumnValueMap>> valueMaps);
        Task<int> DeleteAsync(string tableName, string whereClause);
        Task DeleteDatabaseFileAsync();
    }
}
