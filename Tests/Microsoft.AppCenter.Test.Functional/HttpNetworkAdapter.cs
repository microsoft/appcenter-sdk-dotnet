// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.Functional
{
    internal class HttpNetworkAdapter : IHttpNetworkAdapter
    {
        struct ExpectedData
        {
            public HttpResponse Response;
            public Func<RequestData, bool> Where;
            public TaskCompletionSource<RequestData> Task;
        }

        private HttpResponse defaultHttpResponse;

        private List<ExpectedData> ExpectedDataList = new List<ExpectedData>();

        internal int CallCount { get; private set; }

        internal HttpNetworkAdapter()
        {
            defaultHttpResponse = new HttpResponse
            {
                StatusCode = 200,
                Content = ""
            };
        }

        public Task<RequestData> MockRequest(Func<RequestData, bool> where, HttpResponse response)
        {
            var ct = new CancellationTokenSource(200000);
            var expectedData = new ExpectedData
            {
                Response = response,
                Where = where,
                Task = new TaskCompletionSource<RequestData>(ct)
            };
            return expectedData.Task.Task;
        }

        public Task<HttpResponse> SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken)
        {
            lock (this)
            {
                var requestData = new RequestData(uri, method, headers, jsonContent);
                foreach (var rule in ExpectedDataList)
                {
                    var result = rule.Where(requestData);
                    if (result)
                    {
                        CallCount++;
                        rule.Task.TrySetResult(requestData);
                        return Task.FromResult(rule.Response);
                    }
                }
                return Task.FromResult(defaultHttpResponse);
            }
        }

        public void Dispose()
        {
        }
    }
}
