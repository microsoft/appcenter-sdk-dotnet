// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AppKit;

namespace Contoso.Forms.Puppet.MacOS
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppDelegate(); // add this line
            NSApplication.Main(args);
        }
    }
}
