// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    using AndroidAppCenterLog = Com.Microsoft.Appcenter.Utils.AppCenterLog;

    public static partial class AppCenterLog
    {
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public const string LogTag = nameof(AppCenter) + "Xamarin";

        public static void Verbose(string tag, string message)
        {
            AndroidAppCenterLog.Verbose(tag, message);
        }

        public static void Debug(string tag, string message)
        {
            AndroidAppCenterLog.Debug(tag, message);
        }

        public static void Info(string tag, string message)
        {
            AndroidAppCenterLog.Info(tag, message);
        }

        public static void Warn(string tag, string message)
        {
            AndroidAppCenterLog.Warn(tag, message);
        }

        public static void Error(string tag, string message)
        {
            AndroidAppCenterLog.Error(tag, message);
        }

        public static void Assert(string tag, string message)
        {
            AndroidAppCenterLog.LogAssert(tag, message);
        }
    }
}
