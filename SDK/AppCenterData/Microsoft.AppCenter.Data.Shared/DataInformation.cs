// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Data<T>
    {
        /**
        * Change the base URL used to make API calls.
        *
        * @param apiUrl API base URL.
        */
        // TODO Remove suppress warnings after reflection removed in test app
        public static void SetApiUrl(String apiUrl)
        {

        }

        /**
         * Check whether Storage service is enabled or not.
         *
         * @return future with result being <code>true</code> if enabled, <code>false</code> otherwise.
         * @see AppCenterFuture
         */
        public static Task<Boolean> IsEnabled()
        {
            return null;
        }

        /**
         * Enable or disable Storage service.
         *
         * @param enabled <code>true</code> to enable, <code>false</code> to disable.
         * @return future with null result to monitor when the operation completes.
         */
        public static Task SetEnabled(Boolean enabled)
        {
            return null;
        }

        /**
         * Read a document.
         * The document type (T) must be JSON deserializable.
         */
        public static Task<Data<T>> Read(String partition, String documentId, T documentType)
        {
            return Read(partition, documentId, documentType, new ReadOptions());
        }

        /**
         * Read a document.
         * The document type (T) must be JSON deserializable.
         */
        public static Task<Data<T>> Read(String partition, String documentId, T documentType, ReadOptions readOptions)
        {
            return null;
        }

        /**
         * List (need optional signature to configure page size).
         * The document type (T) must be JSON deserializable.
         */
        public static Task<PaginatedDocuments<T>> List(String partition, T documentType)
        {
            return null;
        }

        /**
         * Create a document.
         * The document instance (T) must be JSON serializable.
         */
        public static Task<Data<T>> Create(String partition, String documentId, T document, T documentType)
        {
            return null;
        }

        /**
         * Create a document.
         * The document instance (T) must be JSON serializable.
         */
        public static Task<T> Create(String partition, String documentId, T document,  T documentType, WriteOptions writeOptions)
        {
            return null;
        }

        /**
         * Delete a document.
         */
        public static Task<T> Selete(String partition, String documentId)
        {
            return null;
        }

        /**
         * Replace a document.
         * The document instance (T) must be JSON serializable.
         */
        public static Task<Data<T>> Replace(String partition, String documentId, T document, T documentType)
        {
            return null;
        }

        /**
         * Replace a document.
         * The document instance (T) must be JSON serializable.
         */
        public static Task<Data<T>> Replace(String partition, String documentId, T document, T documentType, WriteOptions writeOptions)
        {
            return null;
        }

        /**
         * Sets a listener that will be invoked on network status change to notify of pending operations execution status.
         * Pass null to unregister.
         *
         * @param listener to notify on remote operations or null to unregister the previous listener.
         */
        public static void SetDataStoreRemoteOperationListener(IDataStoreEventListener listener)
        {

        }
    }
}
