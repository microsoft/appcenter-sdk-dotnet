using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace Contoso.MacOS.Puppet.ModulePages
{
    public partial class Crashes : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public Crashes(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public Crashes(NSCoder coder) : base(coder)
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
