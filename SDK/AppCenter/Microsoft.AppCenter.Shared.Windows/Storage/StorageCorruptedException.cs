// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Storage
{
    internal class StorageCorruptedException : StorageException
    {
        private const string DefaultMessage = "The database disk image is malformed.";

        public StorageCorruptedException() : base(DefaultMessage) { }

        public StorageCorruptedException(string message) : base(message) { }
    }
}
