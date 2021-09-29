using Foundation;
using Microsoft.AppCenter.Crashes.MacOS.Bindings;

namespace Microsoft.AppCenter.Crashes
{
    public partial class ErrorAttachmentLog
    {
        internal MSACErrorAttachmentLog internalAttachment { get; }

        ErrorAttachmentLog(MSACErrorAttachmentLog macAttachment)
        {
            internalAttachment = macAttachment;
        }

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            MSACErrorAttachmentLog macAttachment = MSACErrorAttachmentLog.AttachmentWithText(text, fileName);
            return new ErrorAttachmentLog(macAttachment);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSACErrorAttachmentLog macAttachment = MSACErrorAttachmentLog.AttachmentWithBinaryData(nsdata, filename, contentType);
            return new ErrorAttachmentLog(macAttachment);
        }
    }
}