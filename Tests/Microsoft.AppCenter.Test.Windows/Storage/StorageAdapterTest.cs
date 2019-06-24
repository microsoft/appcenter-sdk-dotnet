// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Storage;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test
{
    [TestClass]
    public class StorageAdapterTest
    {
        [TestMethod]
        public void CreateStorageAdapterCreatesStorageDirectory()
        {
            var databaseDirectory = "databaseDirectory";
            var databasePath = System.IO.Path.Combine(databaseDirectory, "database.db");
            int callCount = 0;
            string actualDirectory = null;
            using (ShimsContext.Create())
            {
                System.IO.Fakes.ShimDirectory.CreateDirectoryString = directory => {
                    actualDirectory = directory;
                    ++callCount;
                    return new System.IO.DirectoryInfo(directory);
                };
                var _ = new StorageAdapter(databasePath);
                Assert.AreEqual(1, callCount);
                Assert.AreEqual(databaseDirectory, actualDirectory);
            }
        }

        [TestMethod]
        public void CreateStorageAdapterDoesNotCreateDirectoryWhenThereIsNone()
        {
            var databasePath = "database.db";
            int callCount = 0;
            using (ShimsContext.Create())
            {
                System.IO.Fakes.ShimDirectory.CreateDirectoryString = path => {
                    ++callCount;
                    return new System.IO.DirectoryInfo(path);
                };
                var _ = new StorageAdapter(databasePath);
                Assert.AreEqual(0, callCount);
            }
        }
    }
}
