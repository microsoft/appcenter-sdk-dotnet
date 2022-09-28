// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Java.Util;

namespace Microsoft.AppCenter.Crashes
{
    using AndroidExceptionDataManager = Android.WrapperSdkExceptionManager;
    using AndroidErrorReport = Android.Model.ErrorReport;

    public partial class ErrorReport
    {
        internal ErrorReport(AndroidErrorReport androidReport)
        {
            Id = androidReport.Id;
            AppStartTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppStartTime.Time);
            AppErrorTime = DateTimeOffset.FromUnixTimeMilliseconds(androidReport.AppErrorTime.Time);
            Device = androidReport.Device == null ? null : new Device(androidReport.Device);
            var androidStackTrace = androidReport.StackTrace;
            AndroidDetails = new AndroidErrorDetails(androidStackTrace, androidReport.ThreadName);
            AppleDetails = null;
            string exceptionString = AndroidExceptionDataManager.LoadWrapperExceptionData(UUID.FromString(Id));
            if (exceptionString != null)
            {
                StackTrace = exceptionString;
            }
        }
    }
}
