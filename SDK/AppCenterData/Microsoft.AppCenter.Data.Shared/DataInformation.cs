// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public partial class Data<T>
    {
        /// <summary>
        /// Sets the API URL.
        /// </summary>
        /// <param name="apiUrl">API URL.</param>
        public static void SetApiUrl(string apiUrl)
        {

        }

        /// <summary>
        /// Ises the enabled.
        /// </summary>
        /// <returns>The enabled.</returns>
        public static Task<bool> IsEnabled()
        {
            throw new Exception();
        }


        /// <summary>
        /// Sets the enabled.
        /// </summary>
        /// <returns>The enabled.</returns>
        /// <param name="enabled">If set to <c>true</c> enabled.</param>
        public static Task SetEnabled(Boolean enabled)
        {
            throw new Exception();
        }

        /// <summary>
        /// Read the specified partition, documentId and documentType.
        /// </summary>
        /// <returns>The read.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="documentType">Document type.</param>
        public static Task<T> Read(string partition, string documentId, T documentType)
        {
            throw new Exception();
        }

        /// <summary>
        /// Read the specified partition, documentId, documentType and readOptions.
        /// </summary>
        /// <returns>The read.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="readOptions">Read options.</param>
        public static Task<T> Read(string partition, string documentId, T documentType, ReadOptions readOptions)
        {
            throw new Exception();
        }

        /// <summary>
        /// List the specified partition and documentType.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentType">Document type.</param>
        public static Task<T> List(string partition, T documentType)
        {
            throw new Exception();
        }

        /// <summary>
        /// Create the specified partition, documentId, document and documentType.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="document">Document.</param>
        /// <param name="documentType">Document type.</param>
        public static Task<T> Create(string partition, string documentId, T document, T documentType)
        {
            throw new Exception();
        }

        /// <summary>
        /// Create the specified partition, documentId, document, documentType and writeOptions.
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="document">Document.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="writeOptions">Write options.</param>
        public static Task<T> Create(string partition, string documentId, T document, T documentType, WriteOptions writeOptions)
        {
            throw new Exception();
        }

        /// <summary>
        /// Selete the specified partition and documentId.
        /// </summary>
        /// <returns>The selete.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentId">Document identifier.</param>
        public static Task<T> Selete(string partition, string documentId)
        {
            throw new Exception();
        }

        /// <summary>
        /// Replace the specified partition, documentId, document and documentType.
        /// </summary>
        /// <returns>The replace.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="document">Document.</param>
        /// <param name="documentType">Document type.</param>
        public static Task<T> Replace(string partition, string documentId, T document, T documentType)
        {
            throw new Exception();
        }

        /// <summary>
        /// Replace the specified partition, documentId, document, documentType and writeOptions.
        /// </summary>
        /// <returns>The replace.</returns>
        /// <param name="partition">Partition.</param>
        /// <param name="documentId">Document identifier.</param>
        /// <param name="document">Document.</param>
        /// <param name="documentType">Document type.</param>
        /// <param name="writeOptions">Write options.</param>
        public static Task<Data<T>> Replace(string partition, string documentId, T document, T documentType, WriteOptions writeOptions)
        {
            throw new Exception();
        }

        /// <summary>
        /// Sets the data store remote operation listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        public static void SetDataStoreRemoteOperationListener(IDataStoreEventListener listener)
        {
            throw new Exception();
        }
    }
}
