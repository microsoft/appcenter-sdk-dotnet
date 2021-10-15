// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Contoso.Forms.Puppet.MacOS;
using Foundation;
using MobileCoreServices;
using Xamarin.Forms;

// Make this class visible to the DependencyService manager.
[assembly: Dependency(typeof(FilePicker))]

namespace Contoso.Forms.Puppet.MacOS
{
    public class FilePicker : IFilePicker
    {
        private string mockUri = "file://mock.url";

        public FilePicker()
        {
        }

        public string GetFileDescription(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }
            var fileSize = file.Length;
            return NSByteCountFormatter.Format(fileSize, NSByteCountFormatterCountStyle.Binary);
        }

        public async Task<string> PickFile()
        {
            var taskCompletionSource = new TaskCompletionSource<string>();
            taskCompletionSource.SetResult(mockUri);
            return await taskCompletionSource.Task;
        }

        public Tuple<byte[], string, string> ReadFile(string file)
        {
            var dataUti = file;
            var data = new NSData(file, NSDataBase64DecodingOptions.None);
            var extension = "txt";
            var uti = UTType.CreatePreferredIdentifier(UTType.TagClassFilenameExtension, extension, null);
            var mime = UTType.GetPreferredTag(uti, UTType.TagClassMIMEType);
            var dataBytes = new byte[data.Length];
            Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));
            var result = new Tuple<byte[], string, string>(dataBytes, dataUti, mime);
            return result;
        }
    }
}
