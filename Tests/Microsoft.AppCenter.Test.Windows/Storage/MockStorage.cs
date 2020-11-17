// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Storage;

namespace Microsoft.AppCenter.Test.Storage
{
    public class MockStorage : IStorage
    {
        private readonly IDictionary<string, List<Log>> _storage = new Dictionary<string, List<Log>>();
        private readonly IDictionary<string, List<Log>> _pending = new Dictionary<string, List<Log>>();

        private List<Log> this[string key]
        {
            get
            {
                if (!_storage.TryGetValue(key, out var logs))
                {
                    _storage.Add(key, logs = new List<Log>());
                }
                return logs;
            }
        }

        public Task ClearPendingLogState(string channelName)
        {
            lock (this)
            {
                return TaskExtension.GetCompletedTask();
            }
        }

        public Task<int> CountLogsAsync(string channelName)
        {
            lock (this)
            {
                return TaskExtension.GetCompletedTask(this[channelName].Count);
            }
        }

        public Task DeleteLogs(string channelName)
        {
            lock (this)
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, $"Storage.DeleteLogs for channelName={channelName}");
                _storage.Remove(channelName);
                return TaskExtension.GetCompletedTask();
            }
        }

        public Task DeleteLogs(string channelName, string batchId)
        {
            lock (this)
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, $"Storage.DeleteLogs for channelName={channelName} and batchId={batchId}");
                var batch = _pending[batchId];
                this[channelName].RemoveAll(log => batch.Contains(log));
                return TaskExtension.GetCompletedTask();
            }
        }

        public Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            lock (this)
            {
                var pending = _pending.SelectMany(i => i.Value).ToList();
                var batchId = Guid.NewGuid().ToString();
                var batch = this[channelName]
                    .Where(log => !pending.Contains(log))
                    .Take(limit).ToList();
                if (batch.Count == 0)
                {
                    return TaskExtension.GetCompletedTask<string>(null);
                }
                _pending.Add(batchId, batch);
                logs?.Clear();
                logs?.AddRange(batch);
                AppCenterLog.Debug(AppCenterLog.LogTag, $"Storage.GetLogsAsync for channelName={channelName}: batchId={batchId} with logs={string.Join(",", logs)}");
                return TaskExtension.GetCompletedTask(batchId);
            }
        }

        public Task PutLog(string channelName, Log log)
        {
            lock (this)
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, $"Storage.PutLog for channelName={channelName}: {log}");
                this[channelName].Add(log);
                return TaskExtension.GetCompletedTask();
            }
        }

        public Task WaitOperationsAsync(TimeSpan timeout)
        {
            lock (this)
            {
                return TaskExtension.GetCompletedTask();
            }
        }

        public Task<bool> ShutdownAsync(TimeSpan timeout)
        {
            lock (this)
            {
                return TaskExtension.GetCompletedTask(true);
            }
        }

        public Task<bool> SetMaxStorageSizeAsync(long sizeInBytes)
        {
            lock (this)
            {
                return TaskExtension.GetCompletedTask(true);
            }
        }

        public void Dispose()
        {
        }
    }
}
