// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Newtonsoft.Json;
using Directory = Microsoft.AppCenter.Utils.Files.Directory;
using File = Microsoft.AppCenter.Utils.Files.File;

namespace Microsoft.AppCenter.Storage
{
    /// <summary>
    /// Manages the database of App Center logs on disk
    /// </summary>
    internal sealed class Storage : IStorage
    {
        internal class LogEntry
        {
            public long Id { get; set; }

            // The name of the channel that emitted the log
            public string Channel { get; set; }

            // The serialized json text of the log
            public string Log { get; set; }
        }

        // Const for storage data.
        private const string TableName = "LogEntry";
        private const string ColumnChannelName = "Channel";
        private const string ColumnLogName = "Log";
        private const string ColumnIdName = "Id";
        private const string DbIdentifierDelimiter = "@";

        private readonly IStorageAdapter _storageAdapter;
        private readonly string _databasePath;
        private readonly Dictionary<string, IList<long>> _pendingDbIdentifierGroups = new Dictionary<string, IList<long>>();
        private readonly HashSet<long> _pendingDbIdentifiers = new HashSet<long>();

        // Blocking collection is thread safe.
        private readonly BlockingCollection<Task> _queue = new BlockingCollection<Task>();
        private readonly SemaphoreSlim _flushSemaphore = new SemaphoreSlim(0);
        private readonly Task _queueFlushTask;

        /// <summary>
        /// Creates an instance of Storage.
        /// </summary>
        public Storage() : this(DefaultAdapter(), Constants.AppCenterDatabasePath)
        {
        }

        /// <summary>
        /// Creates an instance of Storage given a connection object.
        /// </summary>
        internal Storage(IStorageAdapter adapter, string databasePath)
        {
            AppCenterLog.Debug(AppCenterLog.LogTag, $"Creating database at: {databasePath}");
            _storageAdapter = adapter;
            _databasePath = databasePath;
            _queue.Add(new Task(InitializeDatabase));
            _queueFlushTask = Task.Run(FlushQueueAsync);
        }

        private static IStorageAdapter DefaultAdapter()
        {
            return new StorageAdapter();
        }

        /// <summary>
        /// Asynchronously adds a log to storage
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the log</param>
        /// <param name="log">The log to add</param>
        /// <exception cref="StorageException"/>
        public Task PutLog(string channelName, Log log)
        {
            return AddTaskToQueue(() =>
            {
                var logJsonString = LogSerializer.Serialize(log);
                var maxSize = _storageAdapter.GetMaxStorageSize();
                var logSize = Encoding.UTF8.GetBytes(logJsonString).Length;
                if (maxSize == -1)
                {
                    throw new StorageException("Failed to store a log to the database.");
                }
                if (maxSize <= logSize)
                {
                    throw new StorageException($"Log is too large ({logSize} bytes) to store in database. " +
                            $"Current maximum database size is {maxSize} bytes.");
                }
                while (true)
                {
                    try
                    {
                        _storageAdapter.Insert(TableName,
                            new[] { ColumnChannelName, ColumnLogName },
                            new List<object[]> {
                                new object[] {channelName, logJsonString}
                            });
                        return;
                    }
                    catch (StorageFullException)
                    {
                        var oldestLog = _storageAdapter.Select(TableName, ColumnChannelName, channelName, string.Empty, null, 1, new string[] { ColumnIdName });
                        if (oldestLog != null && oldestLog.Count > 0)
                        {
                            _storageAdapter.Delete(TableName, ColumnIdName, oldestLog[0][0]);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Asynchronously deletes all logs in a particular batch
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the batch</param>
        /// <param name="batchId">The batch identifier</param>
        /// <exception cref="StorageException"/>
        public Task DeleteLogs(string channelName, string batchId)
        {
            return AddTaskToQueue(() =>
            {
                try
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag,
                        $"Deleting logs from storage for channel '{channelName}' with batch id '{batchId}'");
                    var identifiers = _pendingDbIdentifierGroups[GetFullIdentifier(channelName, batchId)];
                    _pendingDbIdentifierGroups.Remove(GetFullIdentifier(channelName, batchId));
                    var deletedIdsMessage = "The IDs for deleting log(s) is/are:";
                    foreach (var identifier in identifiers)
                    {
                        deletedIdsMessage += "\n\t" + identifier;
                        _pendingDbIdentifiers.Remove(identifier);
                    }
                    AppCenterLog.Debug(AppCenterLog.LogTag, deletedIdsMessage);
                    _storageAdapter.Delete(TableName, ColumnIdName, identifiers.Cast<object>().ToArray());
                }
                catch (KeyNotFoundException e)
                {
                    throw new StorageException(e);
                }
            });
        }

        /// <summary>
        /// Asynchronously deletes all logs for a particular channel
        /// </summary>
        /// <param name="channelName">Name of the channel to delete logs for</param>
        /// <exception cref="StorageException"/>
        public Task DeleteLogs(string channelName)
        {
            return AddTaskToQueue(() =>
            {
                try
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag,
                        $"Deleting all logs from storage for channel '{channelName}'");
                    ClearPendingLogStateWithoutEnqueue(channelName);
                    _storageAdapter.Delete(TableName, ColumnChannelName, channelName);
                }
                catch (KeyNotFoundException e)
                {
                    throw new StorageException(e);
                }
            });
        }

        /// <summary>
        /// Asynchronously counts the number of logs stored for a particular channel
        /// </summary>
        /// <param name="channelName">The name of the channel to count logs for</param>
        /// <returns>The number of logs found in storage</returns>
        /// <exception cref="StorageException"/>
        public Task<int> CountLogsAsync(string channelName)
        {
            return AddTaskToQueue(() => _storageAdapter.Count(TableName, ColumnChannelName, channelName));
        }

        /// <summary>
        /// Asynchronously clears the stored state of logs that have been retrieved
        /// </summary>
        /// <param name="channelName"></param>
        public Task ClearPendingLogState(string channelName)
        {
            return AddTaskToQueue(() =>
            {
                ClearPendingLogStateWithoutEnqueue(channelName);
                AppCenterLog.Debug(AppCenterLog.LogTag, $"Clear pending log states for channel {channelName}");
            });
        }

        private void ClearPendingLogStateWithoutEnqueue(string channelName)
        {
            var fullIdentifiers = new List<string>();

            foreach (var fullIdentifier in _pendingDbIdentifierGroups.Keys)
            {
                if (!ChannelMatchesIdentifier(channelName, fullIdentifier))
                {
                    continue;
                }
                foreach (var id in _pendingDbIdentifierGroups[fullIdentifier])
                {
                    _pendingDbIdentifiers.Remove(id);
                }
                fullIdentifiers.Add(fullIdentifier);
            }
            foreach (var fullIdentifier in fullIdentifiers)
            {
                _pendingDbIdentifierGroups.Remove(fullIdentifier);
            }
        }

        /// <summary>
        /// Asynchronously retrieves logs from storage and flags them to avoid duplicate retrievals on subsequent calls
        /// </summary>
        /// <param name="channelName">Name of the channel to retrieve logs from</param>
        /// <param name="limit">The maximum number of logs to retrieve</param>
        /// <param name="logs">A list to which the retrieved logs will be added</param>
        /// <returns>A batch ID for the set of returned logs; null if no logs are found</returns>
        /// <exception cref="StorageException"/>
        public Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            return AddTaskToQueue(() =>
            {
                logs?.Clear();
                var retrievedLogs = new List<Log>();
                AppCenterLog.Debug(AppCenterLog.LogTag,
                    $"Trying to get up to {limit} logs from storage for {channelName}");
                var idPairs = new List<Tuple<Guid?, long>>();
                var failedToDeserializeALog = false;
                var objectEntries = _storageAdapter.Select(TableName, ColumnChannelName, channelName, ColumnIdName, _pendingDbIdentifiers.Cast<object>().ToArray(), limit);
                var retrievedEntries = objectEntries.Select(entries =>
                    new LogEntry()
                    {
                        Id = (long)entries[0],
                        Channel = (string)entries[1],
                        Log = (string)entries[2]
                    }
                ).ToList();
                foreach (var entry in retrievedEntries)
                {
                    try
                    {
                        var log = LogSerializer.DeserializeLog(entry.Log);
                        retrievedLogs.Add(log);
                        idPairs.Add(Tuple.Create(log.Sid, Convert.ToInt64(entry.Id)));
                    }
                    catch (JsonException e)
                    {
                        AppCenterLog.Error(AppCenterLog.LogTag, "Cannot deserialize a log in storage", e);
                        failedToDeserializeALog = true;
                        _storageAdapter.Delete(TableName, ColumnIdName, entry.Id);
                    }
                }
                if (failedToDeserializeALog)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, "Deleted logs that could not be deserialized");
                }
                if (idPairs.Count == 0)
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag,
                        $"No available logs in storage for channel '{channelName}'");
                    return null;
                }

                // Process the results
                var batchId = Guid.NewGuid().ToString();
                ProcessLogIds(channelName, batchId, idPairs);
                logs?.AddRange(retrievedLogs);
                return batchId;
            });
        }

        /// <summary>
        /// Set the maximum size of the storage.
        /// </summary>
        /// <remarks>
        /// This only sets the maximum size of the database, but App Center modules might store additional data.
        /// The value passed to this method is not persisted on disk. The default maximum database size is 10485760 bytes (10 MiB).
        /// </remarks>
        /// <param name="sizeInBytes">
        /// Maximum size of the storage in bytes. This will be rounded up to the nearest multiple of a SQLite page size (default is 4096 bytes).
        /// Values below 20,480 bytes (20 KiB) will be ignored.
        /// </param>
        /// <returns><code>true</code> if changing the size was successful.</returns>
        public Task<bool> SetMaxStorageSizeAsync(long sizeInBytes)
        {
            return AddTaskToQueue(() =>
            {
                try
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag, $"Set max storage size.");
                    return _storageAdapter.SetMaxStorageSize(sizeInBytes);
                }
                catch (Exception e)
                {
                    throw new StorageException(e);
                }
            });
        }

        private void ProcessLogIds(string channelName, string batchId, IEnumerable<Tuple<Guid?, long>> idPairs)
        {
            var ids = new List<long>();
            var message = "The SID/ID pairs for returning logs are:";
            foreach (var idPair in idPairs)
            {
                var sidString = idPair.Item1?.ToString() ?? "(null)";
                message += "\n\t" + sidString + " / " + idPair.Item2;
                _pendingDbIdentifiers.Add(idPair.Item2);
                ids.Add(idPair.Item2);
            }
            _pendingDbIdentifierGroups.Add(GetFullIdentifier(channelName, batchId), ids);
            AppCenterLog.Debug(AppCenterLog.LogTag, message);
        }

        private void InitializeDatabase()
        {
            EnsureDatabaseDirectoryExists();
            try
            {
                _storageAdapter.Initialize(_databasePath);
                _storageAdapter.CreateTable(TableName,
                    new[] { ColumnIdName, ColumnChannelName, ColumnLogName },
                    new[] { "INTEGER PRIMARY KEY AUTOINCREMENT", "TEXT NOT NULL", "TEXT NOT NULL" });
            }
            catch (Exception e)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "An error occurred while initializing storage", e);
            }
        }

        private void EnsureDatabaseDirectoryExists()
        {
            var databaseDirectoryPath = Path.GetDirectoryName(_databasePath);
            if (string.IsNullOrEmpty(databaseDirectoryPath))
            {
                return;
            }
            try
            {
                var databaseDirectory = new Directory(databaseDirectoryPath);
                if (!databaseDirectory.Exists())
                {
                    databaseDirectory.Create();
                }
            }
            catch (Exception e)
            {
                // Not throwing this exception (and not handling it along with DB access error) since there are scenarios when SDK might fail to access database path, but opens database file just fine.
                // So the logic here is only to make sure that directory to contain db file exists - DB file access handled separately.
                AppCenterLog.Error(AppCenterLog.LogTag, "Failed to create database directory.", e);
            }
        }

        /// <summary>
        /// Waits for any running storage operations to complete
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for remaining tasks</param>
        /// <returns>True if remaining tasks completed in time; false otherwise</returns>
        public async Task WaitOperationsAsync(TimeSpan timeout)
        {
            var tokenSource = new CancellationTokenSource();
            try
            {
                var emptyQueueTask = AddTaskToQueue(() => { });
                var timeoutTask = Task.Delay(timeout, tokenSource.Token);
                await Task.WhenAny(emptyQueueTask, timeoutTask).ConfigureAwait(false);
            }
            finally
            {
                tokenSource.Cancel();
            }
        }

        /// <summary>
        /// Waits for any running storage operations to complete and prevents subsequent storage operations from running
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for remaining tasks</param>
        /// <returns>True if remaining tasks completed in time; false otherwise</returns>
        public async Task<bool> ShutdownAsync(TimeSpan timeout)
        {
            _queue.CompleteAdding();
            _flushSemaphore.Release();
            var tokenSource = new CancellationTokenSource();
            try
            {
                var timeoutTask = Task.Delay(timeout, tokenSource.Token);
                return await Task.WhenAny(_queueFlushTask, timeoutTask).ConfigureAwait(false) != timeoutTask;
            }
            finally
            {
                tokenSource.Cancel();
            }
        }

        private static string GetFullIdentifier(string channelName, string identifier)
        {
            return channelName + DbIdentifierDelimiter + identifier;
        }

        private static bool ChannelMatchesIdentifier(string channelName, string identifier)
        {
            var lastDelimiterIndex = identifier.LastIndexOf(DbIdentifierDelimiter, StringComparison.Ordinal);
            return identifier.Substring(0, lastDelimiterIndex) == channelName;
        }

        private Task AddTaskToQueue(Action action)
        {
            var task = new Task(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    throw HandleStorageRelatedException(e);
                }
            });
            AddTaskToQueue(task);
            return task;
        }

        private Task<T> AddTaskToQueue<T>(Func<T> action)
        {
            var task = new Task<T>(() =>
            {
                try
                {
                    return action();
                }
                catch (Exception e)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "The storage operation failed", e);
                    throw HandleStorageRelatedException(e);
                }
            });
            AddTaskToQueue(task);
            return task;
        }

        private Exception HandleStorageRelatedException(Exception e)
        {
            // Re-initialize db file if database is corrupted
            if (e is StorageCorruptedException)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Database corruption detected, deleting the file and starting fresh...", e);
                _storageAdapter.Dispose();
                try
                {
                    new File(_databasePath).Delete();
                }
                catch (IOException fileException)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "Failed to delete database file.", fileException);
                }
                InitializeDatabase();
            }

            // Tasks should already be throwing only storage exceptions, but in case any are missed, 
            // which has happened, catch them here and wrap in a storage exception. This will prevent 
            // the exception from being unobserved.
            return e is StorageException ? e : new StorageException(e);
        }

        private void AddTaskToQueue(Task task)
        {
            try
            {
                _queue.Add(task);
            }
            catch (InvalidOperationException)
            {
                throw new StorageException("The operation has been canceled");
            }
            _flushSemaphore.Release();
        }

        // Flushes the queue
        private async Task FlushQueueAsync()
        {
            while (true)
            {
                while (_queue.Count == 0)
                {
                    if (_queue.IsAddingCompleted)
                    {
                        return;
                    }
                    await _flushSemaphore.WaitAsync().ConfigureAwait(false);
                }
                var t = _queue.Take();
                t.Start();
                try
                {
                    await t.ConfigureAwait(false);
                }
                catch
                {
                    // Can't throw exceptions here because it will cause the FlushQueue to stop
                    // processing, but if the task faults, the exception will be thrown again 
                    // because the original creator of this task will await it too.
                }
            }
        }

        /// <summary>
        /// Disposes the storage object
        /// </summary>
        public void Dispose()
        {
            _queue.CompleteAdding();
            _storageAdapter.Dispose();
        }
    }
}
