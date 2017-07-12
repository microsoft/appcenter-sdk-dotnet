// Copyright (c) Microsoft Corporation.  All rights reserved.
using Newtonsoft.Json;
using System;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Crashes.Ingestion.Models
{
    [JsonObject(JsonIdentifier)]
    public partial class ManagedErrorLog : AbstractErrorLog
    {

        public ManagedErrorLog() { }

        internal static ManagedErrorLog Empty = new ManagedErrorLog();

        internal const string JsonIdentifier = "managed_error";

        public ManagedErrorLog(long toffset, Mobile.Ingestion.Models.Device device, bool fatal, Exception exception,
            long initializeTimestamp, System.Guid? sid = default(System.Guid?))
            : base(toffset, device, fatal, TimeHelper.CurrentTimeInMilliseconds() - initializeTimestamp, sid)
        {
            Exception = new ModelException(exception);
            // TODO TIZEN Look into thread stack traces
        }

        // TODO TIZEN Look into thread stack traces

        [JsonProperty(PropertyName = "exception")]
        public ModelException Exception { get; set; }

        public override void Validate()
        {
            base.Validate();
        }
    }
}