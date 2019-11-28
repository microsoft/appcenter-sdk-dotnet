// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Storage
{
    public interface IStorageAdapter
    {
        Task InitializeStorageAsync();
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> pred, int limit) where T : new();
        Task CreateTableAsync(string tableName, Dictionary<string, string> scheme);
        Task<int> CountAsync(string tableName, string columnName, List<int> values);
        Task<int> InsertAsync(string tableName, Dictionary<string, string> scheme);
        Task<int> DeleteAsync(string tableName, string columnName, List<int> values);
        Task DeleteDatabaseFileAsync();
    }
}
