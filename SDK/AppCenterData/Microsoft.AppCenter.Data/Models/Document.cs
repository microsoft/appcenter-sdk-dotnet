// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Document<T>
    {

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <returns>Deserialized document.</returns>
        public T GetDocument()
        {
            throw new Exception();
        }

        /// <summary>
        /// Gets the document error.
        /// </summary>
        /// <returns>The document error.</returns>
        public DocumentError GetDocumentError()
        {
            throw new Exception();
        }

        /// <summary>
        /// Gets the partition.
        /// </summary>
        /// <returns>The partition.</returns>
        public String GetPartition()
        {
            throw new Exception();
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        public String GetId()
        {
            throw new Exception();
        }

        /// <summary>
        /// Gets the ET ag.
        /// </summary>
        /// <returns>The ET ag.</returns>
        public String GetETag()
        {
            throw new Exception();
        }

        /// <summary>
        /// Get document generated in UTC unix epoch.
        /// </summary>
        /// <returns>UTC unix timestamp.</returns>
        public long GetTimestamp()
        {
            throw new Exception();
        }


        /// <summary>
        /// Get the document in string.
        /// </summary>
        /// <returns>Serialized document..</returns>
        public override string ToString()
        {
            throw new Exception();
        }


        /// <summary>
        /// Get the flag indicating if data was retrieved from the local cache (for offline mode)
        /// </summary>
        /// <returns><c>true</c>, if from cache was ised, <c>false</c> otherwise.</returns>
        public Boolean IsFromCache()
        {
            throw new Exception();
        }

        /// <summary>
        ///Set the flag indicating if data was retrieved from the local cache (for offline mode)
        /// </summary>
        /// <param name="fromCache">If set to <c>true</c> from cache.</param>
        public void SetFromCache(Boolean fromCache)
        {
            throw new Exception();
        }


        /// <summary>
        /// Get the pending operation value.
        /// </summary>
        /// <returns>The pending operation.</returns>
        public String GetPendingOperation()
        {
            throw new Exception();
        }

        /// <summary>
        ///  Set the pending operation value.
        /// </summary>
        /// <param name="pendingOperation">The pending operation saved in the local storage.</param>
        public void SetPendingOperation(String pendingOperation)
        {
            throw new Exception();
        }

        /// <summary>
        /// Hases the failed.
        /// </summary>
        /// <returns>whether the document has an error associated with it.</returns>
        public Boolean HasFailed()
        {
            throw new Exception();
        }
    }
}
