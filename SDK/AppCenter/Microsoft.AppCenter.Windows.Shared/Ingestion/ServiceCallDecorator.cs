using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion
{
    internal class ServiceCallDecorator : ServiceCall
    {
        public string AppSecret { get; set; }
        public Guid InstallId { get; set; }
        public IList<Log> Logs { get; set; }
    }
}
