// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Document<T>
    {

        /// <summary>
        /// Data information.
        /// </summary>
        private String mPartition;

        private String mId;

        private String mETag;

        private long mTimestamp;

        private T mDocument;

        private DocumentError mDocumentError;

        private Boolean mFromCache;

        private String mPendingOperation;

        public Document()
        {

        }

        public Document(T document, String partition, String id)
        {
            mPartition = partition;
            mId = id;
            mDocument = document;
        }

        public Document(T document, String partition, String id, String eTag, long timestamp) : this(document, partition, id)
        {
            mETag = eTag;
            mTimestamp = timestamp;
            mDocument = document;
        }

        public Document(Exception exception)
        {
            mDocumentError = new DocumentError(exception);
        }

        public Document(String message, Exception exception)
        {
            mDocumentError = new DocumentError(new StorageException(message, exception));
        }

      
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <returns>Deserialized document.</returns>
        public T GetDocument()
        {
            return mDocument;
        }

         /// <summary>
         /// Gets the document error.
         /// </summary>
         /// <returns>The document error.</returns>
        public DocumentError GetDocumentError()
        {
            return mDocumentError;
        }

          /// <summary>
          /// Gets the partition.
          /// </summary>
          /// <returns>The partition.</returns>
        public String GetPartition()
        {
            return mPartition;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        public String GetId()
        {
            return mId;
        }

       /// <summary>
       /// Gets the ET ag.
       /// </summary>
       /// <returns>The ET ag.</returns>
        public String GetETag()
        {
            return mETag;
        }

        /// <summary>
        /// Get document generated in UTC unix epoch.
        /// </summary>
        /// <returns>UTC unix timestamp.</returns>
        public long GetTimestamp()
        {
            return mTimestamp;
        }

     
        /// <summary>
        /// Get the document in string.
        /// </summary>
        /// <returns>Serialized document..</returns>
        public override string ToString()
        {
            return base.ToString();
        }

      
        /// <summary>
        /// Get the flag indicating if data was retrieved from the local cache (for offline mode)
        /// </summary>
        /// <returns><c>true</c>, if from cache was ised, <c>false</c> otherwise.</returns>
        public Boolean IsFromCache()
        {
            return mFromCache;
        }

        /// <summary>
        ///Set the flag indicating if data was retrieved from the local cache (for offline mode)
        /// </summary>
        /// <param name="fromCache">If set to <c>true</c> from cache.</param>
        public void SetFromCache(Boolean fromCache)
        {
            mFromCache = fromCache;
        }


        /// <summary>
        /// Get the pending operation value.
        /// </summary>
        /// <returns>The pending operation.</returns>
        public String GetPendingOperation()
        {
            return mPendingOperation;
        }

        /// <summary>
        ///  Set the pending operation value.
        /// </summary>
        /// <param name="pendingOperation">The pending operation saved in the local storage.</param>
        public void SetPendingOperation(String pendingOperation)
        {
            mPendingOperation = pendingOperation;
        }

        /// <summary>
        /// Hases the failed.
        /// </summary>
        /// <returns>whether the document has an error associated with it.</returns>
        public Boolean HasFailed()
        {
            return GetDocumentError() != null;
        }
    }

}
