using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    /// <summary>
    /// Pending operation.
    /// </summary>
    public class PendingOperation
    {
        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <returns>Table name the operation is performed on.</returns>
        public String GetTable()
        {
            throw new Exception();
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <returns>The operation.</returns>
        public String GetOperation()
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
        /// Gets the document identifier.
        /// </summary>
        /// <returns>The document identifier.</returns>
        public String GetDocumentId()
        {
            throw new Exception();
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <returns>The document.</returns>
        public String GetDocument()
        {
            throw new Exception();
        }

        /// <summary>
        /// Sets the document.
        /// </summary>
        /// <param name="document">Document.</param>
        public void SetDocument(String document)
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
        /// Sets the ET ag.
        /// </summary>
        /// <param name="eTag">E tag.</param>
        public void SetETag(String eTag)
        {
            throw new Exception();
        }

        /// <summary>
        /// Gets the expiration time.
        /// </summary>
        /// <returns>The expiration time.</returns>
        public long GetExpirationTime()
        {
            throw new Exception();
        }
    }
}