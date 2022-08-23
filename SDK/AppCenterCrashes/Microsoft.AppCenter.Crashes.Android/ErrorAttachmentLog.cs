// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Crashes
{
    using AndroidErrorAttachmentLog = Android.Ingestion.Models.ErrorAttachmentLog;

    public partial class ErrorAttachmentLog
    {
        internal AndroidErrorAttachmentLog InternalAttachment { get; }

        ErrorAttachmentLog(AndroidErrorAttachmentLog androidAttachment)
        {
            InternalAttachment = androidAttachment;
        }

        static ErrorAttachmentLog PlatformAttachmentWithText(string text, string fileName)
        {
            AndroidErrorAttachmentLog androidAttachment = AndroidErrorAttachmentLog.AttachmentWithText(text, fileName);
            return new ErrorAttachmentLog(androidAttachment);
        }

        static ErrorAttachmentLog PlatformAttachmentWithBinary(byte[] data, string filename, string contentType)
        {
            AndroidErrorAttachmentLog androidAttachment = AndroidErrorAttachmentLog.AttachmentWithBinary(data, filename, contentType);
            return new ErrorAttachmentLog(androidAttachment);
        }
    }
}
