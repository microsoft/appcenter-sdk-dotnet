using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.Mobile.Crashes.Ingestion.Models
{
    [JsonObject(JsonIdentifier)]
    public class ModelStackFrame
    {
        internal const string JsonIdentifier = "stack_frame";

        internal static ModelStackFrame Empty = new ModelStackFrame();

        public ModelStackFrame()
        { }

        public ModelStackFrame(StackFrame stackFrame)
        {
            className = stackFrame.GetMethod()?.DeclaringType?.FullName;
            methodName = stackFrame.GetMethod()?.Name;
            fileName = stackFrame.GetFileName();
            if (stackFrame.GetFileLineNumber() != 0)
            {
                lineNumber = stackFrame.GetFileLineNumber();
            }
        }

        [JsonProperty(PropertyName = "class_name")]
        public string className { get; set; }

        [JsonProperty(PropertyName = "method_name")]
        public string methodName { get; set; }

        [JsonProperty(PropertyName = "line_number")]
        public int lineNumber { get; set; }

        [JsonProperty(PropertyName = "file_name")]
        public string fileName { get; set; }
    }
}
