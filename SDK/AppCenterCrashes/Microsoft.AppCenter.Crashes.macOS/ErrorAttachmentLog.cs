using Foundation;
using Microsoft.AppCenter.Crashes.macOS.Bindings;

namespace Microsoft.AppCenter.Crashes
{
    public partial class ErrorAttachmentLog
    {
        internal MSErrorAttachmentLog internalAttachment { get; }

        ErrorAttachmentLog(MSErrorAttachmentLog macAttachment)
        {
            internalAttachment = macAttachment;
        }

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            MSErrorAttachmentLog macAttachment = MSErrorAttachmentLog.AttachmentWithText(text, fileName);
            return new ErrorAttachmentLog(macAttachment);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSErrorAttachmentLog macAttachment = MSErrorAttachmentLog.AttachmentWithBinaryData(nsdata, filename, contentType);
            return new ErrorAttachmentLog(macAttachment);
        }
    }
}
