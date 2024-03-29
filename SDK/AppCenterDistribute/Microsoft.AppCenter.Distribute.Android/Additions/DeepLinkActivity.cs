// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.Runtime;

namespace Microsoft.AppCenter.Distribute
{
    [Preserve]
    class DeepLinkActivity : Android.DeepLinkActivity
    {
        /* We don't use that subclass, it's only for preserving the parent Java class when linker is used. */
    }
}
