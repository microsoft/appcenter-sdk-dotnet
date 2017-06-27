// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Diagnostics;
using Tizen.System;

namespace Microsoft.Azure.Mobile.Crashes.Ingestion.Models
{
    public abstract class AbstractErrorLog : Log
    {
        protected AbstractErrorLog() { }

        protected AbstractErrorLog(long toffset, Mobile.Ingestion.Models.Device device, bool fatal, long appLaunchTOffset, System.Guid? sid = default(System.Guid?))
            : base(toffset, device, sid)
        {
            Id = Guid.NewGuid();
            ProcessId = Process.GetCurrentProcess().Id;
            ProcessName = Process.GetCurrentProcess().ProcessName;
            Fatal = fatal;
            AppLaunchTOffset = appLaunchTOffset;
            string architecture = "";
            if (SystemInfo.TryGetValue("http://tizen.org/feature/platform.core.cpu.arch", out architecture))
            {
                Architecture = architecture;
            }
        }

        [JsonProperty(PropertyName = "id")]
        public Guid Id;

        [JsonProperty(PropertyName = "process_id")]
        public int ProcessId { get; set; }

        [JsonProperty(PropertyName = "process_name")]
        public string ProcessName { get; set; }

        [JsonProperty(PropertyName = "parent_process_id")]
        public int ParentProcessId { get; set; }

        [JsonProperty(PropertyName = "parent_process_name")]
        public string ParentProcessName { get; set; }

        [JsonProperty(PropertyName = "error_thread_id")]
        public long ErrorThreadId { get; set; }

        [JsonProperty(PropertyName = "error_thread_name")]
        public string ErrorThreadName { get; set; }

        [JsonProperty(PropertyName = "fatal")]
        public bool Fatal { get; set; }

        [JsonProperty(PropertyName = "app_launch_toffset")]
        public long AppLaunchTOffset { get; set; }

        [JsonProperty(PropertyName = "architecture")]
        public string Architecture { get; set; }
    }
}

