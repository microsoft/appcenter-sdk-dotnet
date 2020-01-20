// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
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

        private HttpResponse defaultHttpResponse = new HttpResponse
        {
            StatusCode = 200,
            Content = ""
        };

        private List<ExpectedData> ExpectedDataList = new List<ExpectedData>();

        internal int CallCount { get; private set; }

        public Task<RequestData> MockRequestByLogType(string logType, HttpResponse response = null)
        {
            Func<RequestData, bool> logTypeRule = (RequestData arg) =>
            {

                var result = arg.JsonContent.SelectTokens($"$.logs[?(@.type == '{logType}')]").ToList().Count > 0;
                return result;
            };
            return MockRequest(logTypeRule, response);
        }

        public Task<RequestData> MockRequest(Func<RequestData, bool> where, HttpResponse response = null)
        {
            if (response == null)
            {
                response = defaultHttpResponse;
            }
            var ct = new CancellationTokenSource(200000);
            var expectedData = new ExpectedData
            {
                Response = response,
                Where = where,
                Task = new TaskCompletionSource<RequestData>(ct)
            };
            ExpectedDataList.Add(expectedData);
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
                        ExpectedDataList.Remove(rule);
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
