using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public class StorageException : Exception
    {
        public StorageException(String message, Exception cause) : base(message, cause)
        {
            
        }

        public StorageException(String message) : base(message)
        {

        }
    }
}