using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Microsoft.Azure.Mobile.Crashes.Ingestion.Models
{
    [JsonObject(JsonIdentifier)]
    public class ModelException
    {
        internal const string JsonIdentifier = "exception";

        internal static ModelException Empty = new ModelException();

        public ModelException()
        { }

        public ModelException(Exception exception)
        {
            type = exception.GetType().FullName;
            message = exception.Message;
            rawStackTrace = exception.StackTrace;

            var aggregateException = exception as AggregateException;
            if (aggregateException?.InnerExceptions != null)
            {
                innerExceptions = new List<ModelException>();
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    innerExceptions.Add(new ModelException(innerException));
                }
            }
            else if (exception.InnerException != null)
            {
                innerExceptions = new List<ModelException> { new ModelException(exception.InnerException) };
            }

            StackTrace stackTrace = new StackTrace(exception, true);
            StackFrame[] stackFrames = stackTrace.GetFrames();
            if (stackFrames != null)
            {
                frames = new List<ModelStackFrame>();
                foreach (StackFrame frame in stackFrames)
                {
                    frames.Add(new ModelStackFrame(frame));
                }
            }

            wrapperSdkName = WrapperSdk.Name;
        }

        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string message { get; set; }

        [JsonProperty(PropertyName = "stack_trace")]
        public string rawStackTrace { get; set; }

        [JsonProperty(PropertyName = "frames")]
        public List<ModelStackFrame> frames { get; set; }

        [JsonProperty(PropertyName = "inner_exceptions")]
        public List<ModelException> innerExceptions { get; set; }

        [JsonProperty(PropertyName = "wrapper_sdk_name")]
        public string wrapperSdkName { get; set; }

    }
}
