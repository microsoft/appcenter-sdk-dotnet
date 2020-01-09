// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Threading.Tasks;

namespace Contoso.UtilClassLibrary
{
    public static class CrashUtils
    {
        public static Task BackgroundExceptionTask()
        {
            return Task.Run(() => throw new IOException("The answer is 42"));
        }
    }
}
