// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using Windows.System.Diagnostics;

namespace Microsoft.AppCenter.Crashes.Utils
{
    class ProcessInformation : IProcessInformation
    {
        public DateTime? ProcessStartTime => ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.DateTime;

        public int? ProcessId => (int?)ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId;

        public string ProcessName => ProcessDiagnosticInfo.GetForCurrentProcess().ExecutableFileName;

        public int? ParentProcessId => (int?)ProcessDiagnosticInfo.GetForCurrentProcess().Parent?.ProcessId;

        public string ParentProcessName => ProcessDiagnosticInfo.GetForCurrentProcess().Parent?.ExecutableFileName;

        /// <remarks>
        /// ARM64 was added to ProcessorArchitecture enum (that can be received by Package.Current.Id.Architecture call) only in SDK version 18362, 
        /// so casting to string is incorrect on lower versions.
        /// </remarks>
        public string ProcessArchitecture => RuntimeInformation.ProcessArchitecture.ToString();
    }
}