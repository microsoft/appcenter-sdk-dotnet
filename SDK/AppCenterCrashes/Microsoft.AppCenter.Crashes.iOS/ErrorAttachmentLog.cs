// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using Microsoft.AppCenter.Crashes.iOS.Bindings;

namespace Microsoft.AppCenter.Crashes
{
    public partial class ErrorAttachmentLog
    {
        internal MSACErrorAttachmentLog internalAttachment { get; }

        ErrorAttachmentLog(MSACErrorAttachmentLog iosAttachment)
        {
            internalAttachment = iosAttachment;
        }

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            MSACErrorAttachmentLog iosAttachment = MSACErrorAttachmentLog.AttachmentWithText(text, fileName);
            return new ErrorAttachmentLog(iosAttachment);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            NSData nsdata = NSData.FromArray(data);
            MSACErrorAttachmentLog iosAttachment = MSACErrorAttachmentLog.AttachmentWithBinaryData(nsdata, filename, contentType);
            return new ErrorAttachmentLog(iosAttachment);
        }
    }
}
