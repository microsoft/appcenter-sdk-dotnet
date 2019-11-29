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
        Task<List<Dictionary<string, object>>> GetAsync(string tableName, Dictionary<string, object> scheme, string pred, int? limit);
        Task CreateTableAsync(string tableName, List<ColumnMap> scheme);
        Task<int> CountAsync(string tableName, Dictionary<string, object> scheme, string pred);
        Task<int> InsertAsync(string tableName, Dictionary<string, object> scheme);
        Task<int> DeleteAsync(string tableName, string columnName, List<object> values);
        Task<int> DeleteAsync(string tableName, Dictionary<string, object> values, string pred);
        Task DeleteDatabaseFileAsync();
    }
}
