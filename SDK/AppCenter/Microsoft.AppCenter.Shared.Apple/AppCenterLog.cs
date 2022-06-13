// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if __IOS__
using Microsoft.AppCenter.iOS.Bindings;
#elif __MACOS__
using Microsoft.AppCenter.MacOS.Bindings;
#endif

namespace Microsoft.AppCenter
{
    public static partial class AppCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public const string LogTag = nameof(AppCenter) + "Xamarin";

        public static void Verbose(string tag, string message)
        {
            MSACLogMessageProvider msg_provider = () => { return message; };
            MSACWrapperLogger.MSACWrapperLog(msg_provider, tag, MSACLogLevel.Verbose);
        }

        public static void Debug(string tag, string message)
        {
            MSACLogMessageProvider msg_provider = () => { return message; };
            MSACWrapperLogger.MSACWrapperLog(msg_provider, tag, MSACLogLevel.Debug);
        }

        public static void Info(string tag, string message)
        {
            MSACLogMessageProvider msg_provider = () => { return message; };
            MSACWrapperLogger.MSACWrapperLog(msg_provider, tag, MSACLogLevel.Info);
        }

        public static void Warn(string tag, string message)
        {
            MSACLogMessageProvider msg_provider = () => { return message; };
            MSACWrapperLogger.MSACWrapperLog(msg_provider, tag, MSACLogLevel.Warning);
        }

        public static void Error(string tag, string message)
        {
            MSACLogMessageProvider msg_provider = () => { return message; };
            MSACWrapperLogger.MSACWrapperLog(msg_provider, tag, MSACLogLevel.Error);
        }

        public static void Assert(string tag, string message)
        {
            MSACLogMessageProvider msg_provider = () => { return message; };
            MSACWrapperLogger.MSACWrapperLog(msg_provider, tag, MSACLogLevel.Assert);
        }
    }
}
