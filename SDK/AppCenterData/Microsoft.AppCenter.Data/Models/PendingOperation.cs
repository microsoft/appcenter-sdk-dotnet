using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
     /// <summary>
     /// Pending operation.
     /// </summary>
    public class PendingOperation
    {
        private String Table;

        private String Operation;

        private String Partition;

        private String DocumentId;

        private String Document;

        private String ETag;

        private long ExpirationTime;

        public PendingOperation(string table, string operation, string partition, string documentId, string document, long expirationTime)
        {
            Table = table;
            Operation = operation;
            Partition = partition;
            DocumentId = documentId;
            Document = document;
            ExpirationTime = expirationTime;
        }


        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <returns>Table name the operation is performed on.</returns>
        public String GetTable()
        {
            return Table;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <returns>The operation.</returns>
        public String GetOperation()
        {
            return Operation;
        }

        /// <summary>
        /// Gets the partition.
        /// </summary>
        /// <returns>The partition.</returns>
        public String GetPartition()
        {
            return Partition;
        }

        /// <summary>
        /// Gets the document identifier.
        /// </summary>
        /// <returns>The document identifier.</returns>
        public String GetDocumentId()
        {
            return DocumentId;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <returns>The document.</returns>
        public String GetDocument()
        {
            return Document;
        }

        /// <summary>
        /// Sets the document.
        /// </summary>
        /// <param name="document">Document.</param>
        public void SetDocument(String document)
        {
            Document = document;
        }

        /// <summary>
        /// Gets the ET ag.
        /// </summary>
        /// <returns>The ET ag.</returns>
        public String GetETag()
        {
            return ETag;
        }

        /// <summary>
        /// Sets the ET ag.
        /// </summary>
        /// <param name="eTag">E tag.</param>
        public void SetETag(String eTag)
        {
            ETag = eTag;
        }

        /// <summary>
        /// Gets the expiration time.
        /// </summary>
        /// <returns>The expiration time.</returns>
        public long GetExpirationTime()
        {
            return ExpirationTime;
        }
    }
}