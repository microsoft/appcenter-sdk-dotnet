// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Crashes.Utils
{
    public class StackTraceHelper
    {
        public static string GenerateFullStackTrace(System.Exception e)
        {
            return e.StackTrace;
        }
    }
}
