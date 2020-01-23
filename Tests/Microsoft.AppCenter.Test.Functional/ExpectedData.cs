// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.Functional
{
    internal class ExpectedData
    {
        internal HttpResponse Response { get; set; }
        internal Func<RequestData, bool> Where { get; set; }
        internal TaskCompletionSource<RequestData> TaskCompletionSource { get; set; }
    }
}
