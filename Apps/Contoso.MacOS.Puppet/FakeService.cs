// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;

namespace Contoso.MacOS.Puppet
{
    static class FakeService
    {
        internal async static Task DoStuffInBackground()
        {
            await Task.Run(() => { throw new IOException("Server did not respond"); });
        }
    }
}
