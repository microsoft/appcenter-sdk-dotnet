// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Foundation;
#if __IOS__
using Microsoft.AppCenter.iOS.Bindings;
#elif __MACOS__
using Microsoft.AppCenter.MacOS.Bindings;
#endif

namespace Microsoft.AppCenter
{
    internal class AppleHttpClientAdapter : MSACHttpClientProtocol
    {
        private readonly IHttpNetworkAdapter _httpNetworkAdapter;

        private MSACHttpClientDelegate _httpClientDelegate;

        public AppleHttpClientAdapter(IHttpNetworkAdapter httpNetworkAdapter)
        {
            _httpNetworkAdapter = httpNetworkAdapter;
        }

        public override void SendAsync(NSUrl url, NSString method, NSDictionary<NSString, NSString> headers, NSData data, MSACHttpRequestCompletionHandler completionHandler)
        {
            _httpClientDelegate?.WillSendHTTPRequestToURL(url, headers);
            var managedHeaders = new Dictionary<string, string>();
            if (headers == null)
            {
                managedHeaders = new Dictionary<string, string>();
            }
            else
            {
                foreach (KeyValuePair<NSObject, NSObject> header in headers)
                {
                    managedHeaders[header.Key.ToString()] = header.Value.ToString();
                }
            }
            _httpNetworkAdapter.SendAsync(url.ToString(), method, managedHeaders, data?.ToString(), CancellationToken.None).ContinueWith(t =>
            {
                var innerException = t.Exception?.InnerException;
                if (innerException is HttpException)
                {
                    var response = (innerException as HttpException).HttpResponse;
                    completionHandler(NSData.FromString(response.Content), new NSHttpUrlResponse(url, response.StatusCode, "1.1", new NSDictionary()), null);
                }
                else if (innerException != null)
                {
                    var userInfo = NSDictionary.FromObjectAndKey(new NSString("stackTrace"), new NSString(innerException.ToString()));
                    completionHandler(null, null, new NSError(new NSString(".NET SDK"), 1, userInfo));
                }
                else
                {
                    var response = t.Result;
                    completionHandler(NSData.FromString(response.Content), new NSHttpUrlResponse(url, response.StatusCode, "1.1", new NSDictionary()), null);
                }
            });
        }

        public override void SendAsync(NSUrl url, NSString method, NSDictionary<NSString, NSString> headers, NSData data, NSArray retryIntervals, bool compressionEnabled, MSACHttpRequestCompletionHandler completionHandler)
        {
            SendAsync(url, method, headers, data, completionHandler);
        }

        public override void Pause()
        {
        }

        public override void Resume()
        {
        }

        public override void SetEnabled(bool enabled)
        {
        }

        public override void SetDelegate(MSACHttpClientDelegate httpClientDelegate)
        {
            _httpClientDelegate = httpClientDelegate;
        }
    }
}
