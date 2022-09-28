// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Storage
{
    internal class StorageFullException : StorageException
    {
        private const string DefaultMessage = "The database is full.";

        public StorageFullException() : base(DefaultMessage) { }

        public StorageFullException(string message) : base(message) { }
    }
}
