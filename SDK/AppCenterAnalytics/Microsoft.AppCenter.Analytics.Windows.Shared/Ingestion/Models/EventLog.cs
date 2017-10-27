// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Analytics.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    /// <summary>
    /// Event log.
    /// </summary>
    [JsonObject(JsonIdentifier)]
    public partial class EventLog : LogWithProperties
    {
        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        public EventLog() { }

        internal const string JsonIdentifier = "event";

        /// <summary>
        /// Initializes a new instance of the EventLog class.
        /// </summary>
        /// <param name="timestamp">Log timestamp.</param>
        /// <param name="device">Description of the device emitting the log.</param>
        /// <param name="id">Unique identifier for this event.</param>
        /// <param name="name">Name of the event.</param>
        /// <param name="sid">When tracking an analytics session, logs can be
        /// part of the session by specifying this identifier.
        /// This attribute is optional, a missing value means the session
        /// tracking is disabled (like when using only error reporting
        /// feature).
        /// Concrete types like StartSessionLog or PageLog are always part of a
        /// session and always include this identifier.
        /// </param>
        /// <param name="properties">Additional key/value pair parameters.
        /// </param>
        public EventLog(DateTime? timestamp, Device device, Guid id, string name, Guid? sid = default(Guid?), IDictionary<string, string> properties = default(IDictionary<string, string>))
            : base(timestamp, device, sid, properties)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets or sets unique identifier for this event.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets name of the event.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
            if (Name == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Name");
            }
        }
    }
}

