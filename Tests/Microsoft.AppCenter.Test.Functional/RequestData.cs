// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Microsoft.AppCenter.Test.Functional
{
    public class RequestData
    {
        internal string Uri { get; private set; }
        internal string Method { get; private set; }
        internal IDictionary<string, string> Headers { get; private set; }
        internal JObject JsonContent { get; private set; }

        public RequestData(string uri, string method, IDictionary<string, string> headers, string jsonContent)
        {
            Uri = uri;
            Method = method;
            Headers = headers;
            JsonContent = JObject.Parse(jsonContent);
        }
    }
}
