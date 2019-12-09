// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Storage
{
    internal class StorageCorruptException : AppCenterException
    {
        private const string DefaultMessage = "The database disk image is malformed.";
        public StorageCorruptException() : base(DefaultMessage) { }
    }
}
