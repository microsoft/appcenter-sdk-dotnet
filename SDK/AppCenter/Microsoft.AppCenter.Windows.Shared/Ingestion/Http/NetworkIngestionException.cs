namespace Microsoft.AppCenter.Ingestion.Http
{
    public class NetworkIngestionException : IngestionException
    {
        public override bool IsRecoverable => true;
    }
}
