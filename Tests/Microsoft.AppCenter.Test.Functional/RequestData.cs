// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.AppCenter.Test.Functional
{
    public class RequestData
    {
        public string Uri { get; private set; }
        public string Method { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }
        public JObject JsonContent { get; private set; }

        public RequestData(string uri, string method, IDictionary<string, string> headers, string jsonContent)
        {
            Uri = uri;
            Method = method;
            Headers = headers;
            try
            {
                JsonContent = JObject.Parse(jsonContent == null ? "{ }" : jsonContent);
            }
            catch (JsonReaderException exc)
            {
                JsonContent = new JObject();
            }
        }
    }
}
