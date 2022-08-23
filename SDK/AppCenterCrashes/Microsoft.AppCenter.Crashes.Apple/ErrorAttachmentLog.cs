// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using Microsoft.AppCenter.Crashes.Apple.Bindings;

namespace Microsoft.AppCenter.Crashes
{
    public partial class ErrorAttachmentLog
    {
        internal MSACErrorAttachmentLog InternalAttachment { get; }

        ErrorAttachmentLog(MSACErrorAttachmentLog appleAttachment)
        {
            InternalAttachment = appleAttachment;
        }

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            MSACErrorAttachmentLog appleAttachment = MSACErrorAttachmentLog.AttachmentWithText(text, fileName);
            return new ErrorAttachmentLog(appleAttachment);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSACErrorAttachmentLog appleAttachment = MSACErrorAttachmentLog.AttachmentWithBinaryData(nsdata, filename, contentType);
            return new ErrorAttachmentLog(appleAttachment);
        }
    }
}
