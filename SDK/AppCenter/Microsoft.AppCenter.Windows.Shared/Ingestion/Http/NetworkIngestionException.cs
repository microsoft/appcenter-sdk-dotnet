// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkIngestionException : IngestionException
    {
        public override bool IsRecoverable => true;

        public NetworkIngestionException() { }
        public NetworkIngestionException(Exception innerException) : base(innerException) { }
    }
}
