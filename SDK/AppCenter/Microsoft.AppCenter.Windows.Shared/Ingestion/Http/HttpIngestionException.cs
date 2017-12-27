using System;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public class HttpIngestionException : IngestionException
    {
        public string Method { get; set; }
        public Uri RequestUri { get; set; }
        public string RequestContent { get; set; }
        public int StatusCode { get; set; }
        public string ResponseContent { get; set; }
        public override bool IsRecoverable => StatusCode >= 500 || StatusCode == 408 || StatusCode == 429;

        public HttpIngestionException(string message)
            : base(message)
        {
        }
    }
}
