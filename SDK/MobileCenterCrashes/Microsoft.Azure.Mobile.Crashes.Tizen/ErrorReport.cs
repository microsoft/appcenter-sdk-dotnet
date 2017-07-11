using Microsoft.Azure.Mobile.Crashes.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Crashes
{
    public partial class ErrorReport
    {
        private readonly IDeviceInformationHelper _deviceInfoHelper = new DeviceInformationHelper();

        public ErrorReport()
        {
        }

        public ErrorReport(ManagedErrorLog log, Exception exception)
        {
            Id = log.Id.ToString();
            AppErrorTime = DateTimeOffset.FromUnixTimeMilliseconds(log.Toffset - log.AppLaunchTOffset);
            AppErrorTime = DateTimeOffset.FromUnixTimeMilliseconds(log.Toffset);
            var task = _deviceInfoHelper.GetDeviceInformationAsync();
            task.Wait();
            Device = new Device(task.Result);
            Exception = exception;
            AndroidDetails = null;
            iOSDetails = null;
            // TODO TIZEN add TizenDetails
        }
        // TODO TIZEN add Class TizenErrorDetails with tizen specific errors if any and add a member here
    }
}
