using System;
using System.Linq;
using SQLitePCL;

namespace Microsoft.AppCenter.Test.Windows.Utils
{
    class StorageTestUtils
    {
        private const string TableName = "TestTable";
        private const string ColumnIdName = "ID";
        private const string Column1Name = "TestCol1Name";
        private const string Column2Name = "TestCol2Name";

        private string dbPath;

        public StorageTestUtils(string dbPath)
        {
            this.dbPath = dbPath;
        }

        /// <summary>
        /// Get current storage size in bytes.
        /// </summary>
        /// <returns>Current storage size in bytes.</returns>
        public long GetDataLengthInBytes()
        {
            raw.sqlite3_open_v2(dbPath, out sqlite3 db, raw.SQLITE_OPEN_READONLY, null);
            raw.sqlite3_prepare_v2(db, "PRAGMA page_count;", out var stmt);
            raw.sqlite3_step(stmt);
            var pageCount = raw.sqlite3_column_int(stmt, 0);
            raw.sqlite3_finalize(stmt);
            raw.sqlite3_prepare_v2(db, "PRAGMA page_size;", out stmt);
            raw.sqlite3_step(stmt);
            var pageSize = raw.sqlite3_column_int(stmt, 0);
            raw.sqlite3_finalize(stmt);
            raw.sqlite3_close(db);
            return (long)pageCount * pageSize;
        }

        /// <summary>
        /// Fill storage with a test logs.
        /// </summary>
        /// <param name="dataSize">Storage capacity.</param>
        public void FillStorageWithTestData(long dataSize)
        {
            var db = OpenDatabaseAndCreateTable();
            while (GetDataLengthInBytes() < dataSize)
            {
                AddTestDataToStorage(1000, db);
            }
            raw.sqlite3_close(db);
        }

        private void AddTestDataToStorage(int count, sqlite3 db)
        {
            for (int i = 0; i < count; i++)
            {
                var query = $"INSERT INTO {TableName} ({ColumnIdName}, {Column1Name}, {Column2Name}) VALUES ({i}, 'col1-{i}', 'col2-{i}')";
                var insertResult = raw.sqlite3_exec(db, query);
            }
        }

        private sqlite3 OpenDatabase()
        {
            raw.sqlite3_open(dbPath, out sqlite3 db);
            return db;
        }

        private sqlite3 OpenDatabaseAndCreateTable()
        {
            var db = OpenDatabase();
            var columnNames = new[] { ColumnIdName, Column1Name, Column2Name };
            var columnTypes = new[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT NOT NULL", "TEXT NOT NULL" };
            var cols = string.Join(",", Enumerable.Range(0, columnNames.Length).Select(i => $"{columnNames[i]} {columnTypes[i]}"));
            var createResult = raw.sqlite3_exec(db, $"CREATE TABLE IF NOT EXISTS {TableName} ({cols});");
            Console.WriteLine($"created : {createResult == raw.SQLITE_OK}");
            return db;
        }
    }
}
