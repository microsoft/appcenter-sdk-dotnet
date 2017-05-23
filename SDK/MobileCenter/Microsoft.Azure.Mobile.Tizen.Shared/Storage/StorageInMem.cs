using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
//using System.Data.Common;
using Microsoft.Azure.Mobile.Ingestion.Models;

//using System.Core.System.Data.Linq;
//using System.Core;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Mobile.Storage
{
    // TODO Add docs

    /// <summary>
    /// Manages the database of Mobile Center logs on disk
    /// </summary>
    internal sealed class StorageInMem : IStorage
    {
        internal sealed class LogRecord
        {
            public string channel { get; set; }
            public Log log { get; set; }
            public Int64 rowid { get; set; }
        }

        private static Int64 _currentRowId = 0;

        private List<LogRecord> _logs = new List<LogRecord>();

        /// <summary>
        /// Creates an instance of Storage
        /// </summary>
        public StorageInMem()
        {
        }

        public async Task ClearPendingLogStateAsync(string channelName)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "ClearPendingLogStateAsync(1) Entered, channelName: " + channelName);
            await Task.Factory.StartNew(() =>
            {
                throw new NotImplementedException();
            });
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "[OK] ClearPendingLogStateAsync(1) DONE");
        }

        public async Task<int> CountLogsAsync(string channelName)
        {
            int count = 0;
            await Task.Factory.StartNew(() =>
            {
                count = _logs.Count(n => n.channel == channelName);
            });
            return count;
        }

        public async Task DeleteLogsAsync(string channelName, string batchId)
        {
            await DeleteLogsAsync(channelName);
        }

        public async Task DeleteLogsAsync(string channelName)
        {
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "DeleteLogsAsync(1) Entered, channelName: " + channelName);
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    List<LogRecord> newLogs = new List<LogRecord>();
                    foreach (var log in _logs)
                    {
                        if (log.channel == channelName)
                        {
                            MobileCenterLog.Assert(MobileCenterLog.LogTag, "DeleteLogsAsync(1) deleting log record with rowId: " + log.rowid);
                        }
                        else
                        {
                            newLogs.Add(log);
                        }
                    }
                    MobileCenterLog.Assert(MobileCenterLog.LogTag, "DeleteLogsAsync(1) re-assigning log storage");
                    _logs = newLogs;

                    /*
                    // Query the database for the rows to be deleted.
                    var deleteLogs = from log in _logs
                                     where log.channel == channelName
                                     select log;

                    // Delete selected rows
                    foreach (var log in deleteLogs)
                    {
                        _logs.Remove(log);
                    }
                    */

                    MobileCenterLog.Assert(MobileCenterLog.LogTag, "DeleteLogsAsync(1) all selected logs deleted");
                }
                catch (Exception e)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "DeleteLogsAsync(1) exception: " + e.GetType() + "\n" + e.Message);
                }
            });
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "[OK] DeleteLogsAsync(1) DONE");
        }

        /// <summary>
        /// Asynchronously delete a single log from storage
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the log</param>
        /// <param name="rowId">The row id of the log</param>
        /// <exception cref="StorageException"/>
        private async Task DeleteLogAsync(string channelName, long rowId)
        {
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "DeleteLogsAsync(2) Entered, channelName: " + channelName + ", rowId: " + rowId);
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    List<LogRecord> newLogs = new List<LogRecord>();
                    foreach (var log in _logs)
                    {
                        if ((log.channel == channelName) && (log.rowid == rowId))
                        {
                            MobileCenterLog.Assert(MobileCenterLog.LogTag, "DeleteLogsAsync(1) deleting log record with rowId: " + log.rowid);
                        }
                        else
                        {
                            newLogs.Add(log);
                        }
                    }
                    MobileCenterLog.Assert(MobileCenterLog.LogTag, "DeleteLogsAsync(1) re-assigning log storage");
                    _logs = newLogs;

                    /*// Query the database for the rows to be deleted.
                    var deleteLogs =
                        from log in _logs
                        where log.channel == channelName && log.rowid == rowId
                        select log;

                    // Delete selected rows
                    foreach (var log in deleteLogs)
                    {
                        _logs.Remove(log);
                    }*/
                }
                catch (Exception e)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "DeleteLogsAsync(2) exception: " + e.GetType() + "\n" + e.Message);
                }
            });
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "[OK] DeleteLogsAsync(2) DONE");
        }

        public async Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "GetLogsAsync(3) Entered, channelName: " + channelName + ", limit: " + limit);
            var batchId = "";
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    // Erase previously used logs (if any)
                    logs?.Clear();

                    // Execute the query
                    var queryLogs =
                        (from log in _logs
                         where log.channel == channelName
                         select log.log).Take(limit);

                    // Process the results
                    batchId = Guid.NewGuid().ToString();
                    logs?.AddRange(queryLogs.ToList<Log>());
                    MobileCenterLog.Assert(MobileCenterLog.LogTag, "GetLogsAsync(3) extracted: " + logs.Count() + " logs, batchId: " + batchId);
                }
                catch (Exception e)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "DeleteLogsAsync(2) exception: " + e.GetType() + "\n" + e.Message);
                }
            });
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "[OK] GetLogsAsync(3) DONE");
            return batchId;
        }

        public async Task PutLogAsync(string channelName, Log log)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "PutLogAsync(2) Entered, channelName: " + channelName);
            //await Task.Yield();
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    _logs.Add(new LogRecord
                    {
                        channel = channelName,
                        log = log,
                        rowid = _currentRowId++
                    });
                }
                catch (Exception e)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "DeleteLogsAsync(2) exception: " + e.GetType() + "\n" + e.Message);
                }
            });
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "[OK] PersistLogAsync(2) DONE");
        }

        public bool Shutdown(TimeSpan timeout)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Shutdown(1) Entered, timeour: " + timeout);
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StorageSQLiteNetPCL() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
