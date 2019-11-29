// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Windows.Shared.Storage
{
    public class ColumnMap
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public bool IsPrimarykey { get; set; }
        public bool IsAutoIncrement { get; set; }
    }
}
