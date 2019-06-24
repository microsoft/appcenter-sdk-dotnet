// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Storage;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Microsoft.AppCenter.Test.Storage
{
    [TestClass]
    public class StorageAdapterTest
    {
        [TestMethod]
        public void CreateStorageAdapterCreatesStorageDirectory()
        {
            const string databaseDirectory = "databaseDirectory";
            var databasePath = Path.Combine(databaseDirectory, "database.db");
            var callCount = 0;
            string actualDirectory = null;
            using (ShimsContext.Create())
            {
                System.IO.Fakes.ShimDirectory.CreateDirectoryString = directory =>
                {
                    actualDirectory = directory;
                    ++callCount;
                    return new DirectoryInfo(directory);
                };
                var _ = new StorageAdapter(databasePath);
            }
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(databaseDirectory, actualDirectory);
        }

        [TestMethod]
        public void CreateStorageAdapterDoesNotCreateDirectoryWhenThereIsNone()
        {
            const string databasePath = "database.db";
            var callCount = 0;
            using (ShimsContext.Create())
            {
                System.IO.Fakes.ShimDirectory.CreateDirectoryString = path =>
                {
                    ++callCount;
                    return new DirectoryInfo(path);
                };
                var _ = new StorageAdapter(databasePath);
            }
            Assert.AreEqual(0, callCount);
        }

        [TestMethod]
        public void CreateStorageAdapterExceptionIsWrapped()
        {
            const string databaseDirectory = "databaseDirectory";
            var databasePath = Path.Combine(databaseDirectory, "database.db");
            Exception actualException = null;
            using (ShimsContext.Create())
            {
                System.IO.Fakes.ShimDirectory.CreateDirectoryString = path => throw new PathTooLongException();
                try
                {
                    var _ = new StorageAdapter(databasePath);
                }
                catch (Exception ex)
                {
                    actualException = ex;
                }
            }
            Assert.IsInstanceOfType(actualException, typeof(StorageException));
            Assert.IsInstanceOfType(actualException?.InnerException, typeof(PathTooLongException));
        }
    }
}
