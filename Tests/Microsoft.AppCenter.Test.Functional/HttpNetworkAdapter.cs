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
            var expectedData = new ExpectedData
            {
                Response = response ?? DefaultHttpResponse,
                Where = where,
                TaskCompletionSource = new TaskCompletionSource<RequestData>()
            };
            expectedDataList.Add(expectedData);

            // Since we can't directly assign token to a Task created with FromResult,
            // and we can't work with token in SendAsync because it may or may not be called after this method,
            // we are registering anonymous cancellation token here.
            // The only weak point can occur if there's a significant delay between actual Task creation time and calling this method,
            // but since we only use that in tests, that can be ignored.
            new CancellationTokenSource(TimeSpan.FromSeconds(delayTimeInSeconds)).Token.Register(() => expectedData.TaskCompletionSource.TrySetCanceled());
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
