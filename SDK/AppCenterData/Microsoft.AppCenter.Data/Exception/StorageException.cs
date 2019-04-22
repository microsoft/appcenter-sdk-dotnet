using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{
    public class StorageException : Exception
    {
        public StorageException(string message, Exception cause) : base(message, cause)
        {
            throw new Exception();
        }

        public StorageException(string message) : base(message)
        {
            throw new Exception();
        }
    }
}