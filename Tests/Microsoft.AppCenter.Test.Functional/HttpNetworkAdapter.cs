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
        private static readonly HttpResponse DefaultHttpResponse = new HttpResponse
        {
            StatusCode = 200,
            Content = ""
        };

        private readonly IList<ExpectedData> expectedDataList = new List<ExpectedData>();

        internal int CallCount { get; private set; }

        public Task<RequestData> MockRequestByLogType(string logType, HttpResponse response = null, double delayTimeInSeconds = 20)
        {
            return MockRequest(request => request.JsonContent.SelectTokens($"$.logs[?(@.type == '{logType}')]").ToList().Count > 0, response, delayTimeInSeconds);
        }

        public Task<RequestData> MockRequest(Func<RequestData, bool> where, HttpResponse response = null, double delayTimeInSeconds = 20)
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(delayTimeInSeconds));
            var expectedData = new ExpectedData
            {
                Response = response ?? DefaultHttpResponse,
                Where = where,
                TaskCompletionSource = new TaskCompletionSource<RequestData>(cancellationToken)
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
                return Task.FromResult(DefaultHttpResponse);
            }
        }
        
        public void Dispose()
        {
        }
    }
}
