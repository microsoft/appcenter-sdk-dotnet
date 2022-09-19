// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Threading.Tasks;

namespace MAUI.Demo;

static class FakeService
{
    internal static async Task DoStuffInBackground()
    {
        await Task.Run(() => throw new IOException("Server did not respond"));
    }
}
