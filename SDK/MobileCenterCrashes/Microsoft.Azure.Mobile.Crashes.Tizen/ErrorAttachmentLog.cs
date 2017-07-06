using Newtonsoft.Json;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System;
using System.Text;

namespace Microsoft.Azure.Mobile.Crashes
{
    // TIZEN process error attachment log in Crashes
    // Implement callback for ErrorAttachmentLog in Crashes
    // Add checks for logtype when typecasting to ManagedErrorLog in Crashes
    [JsonObject(JsonIdentifier)]
    public partial class ErrorAttachmentLog : Log
    {
        internal const string JsonIdentifier = "error_attachment";

        private static string CONTENT_TYPE_PLAIN_TEXT = "text/plain";

        public ErrorAttachmentLog() { }

        internal static ErrorAttachmentLog Empty = new ErrorAttachmentLog();

        public ErrorAttachmentLog(long toffset, Mobile.Ingestion.Models.Device device, byte[] data, string fileName, string contentType,
            System.Guid? sid = default(System.Guid?))
            :base(toffset, device, sid)
        {
            Data = data;
            FileName = FileName;
            ContentType = contentType;
        }

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            return PlatformAttachmentWithBinary(Encoding.UTF8.GetBytes(text), fileName, CONTENT_TYPE_PLAIN_TEXT);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string fileName, string contentType)
        {
            return new ErrorAttachmentLog(0, null, data, fileName, contentType);
        }

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "error_id")]
        public Guid ErrorId { get; set; }

        [JsonProperty(PropertyName = "content_type")]
        public string ContentType { get; set; }

        [JsonProperty("data")]
        public byte[] Data { get; set; }

        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }
    }
}
