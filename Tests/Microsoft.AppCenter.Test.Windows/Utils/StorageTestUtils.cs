using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public long GetDataLengthInBytes()
        {
            var db = OpenDatabase();
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

        public void CreateTable()
        {
            var db = OpenDatabase();

            var tables = new[] { ColumnIdName, Column1Name, Column2Name };
            var types = new[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT NOT NULL", "TEXT NOT NULL" };
            var createResult = raw.sqlite3_exec(db, $"CREATE TABLE IF NOT EXISTS {TableName}");
            raw.sqlite3_close(db);
        }


        public void FillStorageWithTestData(long dataSize)
        {
            while (GetDataLengthInBytes() < dataSize)
            {
                AddTestDataToStorage(1000);
            }
        }

        private void AddTestDataToStorage(int count)
        {
            var db = OpenDatabase();

            var columnNames = new[] { ColumnIdName, Column1Name, Column2Name };
            var columnTypes = new[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT NOT NULL", "TEXT NOT NULL" };
            var cols = string.Join(",", Enumerable.Range(0, columnNames.Length).Select(i => $"{columnNames[i]} {columnTypes[i]}"));
            var createResult = raw.sqlite3_exec(db, $"CREATE TABLE IF NOT EXISTS {TableName} ({cols});");

            Console.WriteLine($"created : {createResult == raw.SQLITE_OK}");

            for (int i = 0; i < count; i++)
            {
                var query = $"INSERT INTO {TableName} ({ColumnIdName}, {Column1Name}, {Column2Name}) VALUES ({i}, 'col1-{i}', 'col2-{i}')";
                var insertResult = raw.sqlite3_exec(db, query);
            }

            raw.sqlite3_close(db);
        }

        private sqlite3 OpenDatabase()
        {
            raw.sqlite3_open(dbPath, out sqlite3 db);
            return db;
        }
    }
}
