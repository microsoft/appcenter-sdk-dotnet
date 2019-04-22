// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Data
{


    /// <summary>
    /// Document metadata.
    /// </summary>
    public class DocumentMetadata
    {
        private String mPartition;

        private String mDocumentId;

        private String mETag;

        public DocumentMetadata(string partition, string documentId, string eTag)
        {
            mPartition = partition;
            mDocumentId = documentId;
            mETag = eTag;
        }

        public string GetPartition()
        {
            return mPartition;
        }

        public string GetDocumentId()
        {
            return mDocumentId;
        }

        public string GetETag()
        {
            return mETag;
        }
    }
}