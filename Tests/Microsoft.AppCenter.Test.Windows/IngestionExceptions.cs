using Microsoft.AppCenter.Ingestion;

namespace Microsoft.AppCenter
{
    public class RecoverableIngestionException : IngestionException
    {
        public override bool IsRecoverable => true;
    }

    public class NonRecoverableIngestionException : IngestionException
    {
        public override bool IsRecoverable => false;
    }
}
