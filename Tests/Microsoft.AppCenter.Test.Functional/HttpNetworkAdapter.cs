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
            public TaskCompletionSource<RequestData> TaskCompletionSource;
        }

        private static readonly HttpResponse defaultHttpResponse = new HttpResponse
        {
            StatusCode = 200,
            Content = ""
        };

        private readonly IList<ExpectedData> expectedDataList = new List<ExpectedData>();

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
            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var expectedData = new ExpectedData
            {
                Response = response ?? defaultHttpResponse,
                Where = where,
                TaskCompletionSource = new TaskCompletionSource<RequestData>(ct)
            };
            expectedDataList.Add(expectedData);
            return expectedData.TaskCompletionSource.Task;
        }

        public Task<HttpResponse> SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken)
        {
            lock (expectedDataList)
            {
                var requestData = new RequestData(uri, method, headers, jsonContent);
                foreach (var rule in expectedDataList)
                {
                    var result = rule.Where(requestData);
                    if (result)
                    {
                        CallCount++;
                        rule.TaskCompletionSource.TrySetResult(requestData);
                        expectedDataList.Remove(rule);
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
