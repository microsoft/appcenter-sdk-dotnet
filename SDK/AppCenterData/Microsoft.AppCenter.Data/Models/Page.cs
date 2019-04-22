using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace Microsoft.AppCenter.Data
{
    public class Page<T>
    {
        /// <summary>
        /// Documents in the page.
        /// </summary>
        private List<Document<T>> Items;

        /// <summary>
        /// Document error.
        /// </summary>
        private DocumentError Error;

        public Page()
        {
            
        }

        public Page(Exception exception)
        {
            Error = new DocumentError(exception);
        }

        /// <summary>
        /// Return the documents in the page.
        /// </summary>
        /// <returns>Documents in current page.</returns>
        public List<Document<T>> GetItems()
        {
            return Items;
        }

        public Page<T> SetItems(List<Document<T>> items)
        {
            Items = items;
            return this;
        }

     
        /// <summary>
        /// Get the error if failed to retrieve the page from document db.
        /// </summary>
        /// <returns>The error.</returns>
        public DocumentError GetError()
        {
            return Error;
        }
    }
}