// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{

    /// <summary>
    /// A listener that is going to be notified about remote operations completion status.
    /// </summary>
    public class IDataStoreEventListener
    {
        /// <summary>
        /// Will be called on device network status changing from offline to online as storage operations are attempted to be sent to Cosmos DB
        /// </summary>
        /// <param name="operation">Operation name.</param>
        /// <param name="document">Metadata.</param>
        /// <param name="error">Error details. If null, then the operation was successful.</param>
        void onDataStoreOperationResult(object operation, DocumentEventArgs e)
        {

        }
    }

}