// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace Contoso.MacOS.Puppet.ModulePages
{
    public partial class AppCenter : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public AppCenter(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public AppCenter(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion
    }
}
