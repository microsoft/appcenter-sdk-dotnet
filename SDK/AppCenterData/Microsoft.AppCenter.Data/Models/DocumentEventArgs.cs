using System;
namespace Microsoft.AppCenter.Data
{
    public class DocumentEventArgs : EventArgs
    {
        public DocumentError documentError { get; }

        public DocumentMetadata documentMetadata { get; }
    }
}
