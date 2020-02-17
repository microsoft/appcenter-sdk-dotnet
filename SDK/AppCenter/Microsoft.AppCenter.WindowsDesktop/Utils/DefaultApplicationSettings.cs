// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Microsoft.AppCenter.Utils
{
    public class DefaultApplicationSettings : IApplicationSettings
    {
        private const string FileName = "AppCenter.config";
        private const string CorruptedConfigurationWarning = "Configuration is corrupted. App Center could work incorrectly.";
        private static readonly object configLock = new object();
        private static Configuration configuration;

        internal static string FilePath { get; private set; }

        public DefaultApplicationSettings()
        {
            lock (configLock)
            {
                try
                {
                    configuration = OpenConfiguration();
                }
                catch (Exception e)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, "Configuration could be corrupted.", e);
                    RestoreConfigurationFile();
                }
            }
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            lock (configLock)
            {
                if (configuration != null)
                {
                    var setting = configuration.AppSettings.Settings[key];
                    if (setting != null)
                    {
                        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(setting.Value);
                    }
                }
                else
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, $"{CorruptedConfigurationWarning} Method 'GetValue', key:{key}");
                }
            }
            return defaultValue;
        }

        public void SetValue(string key, object value)
        {
            var invariant = value != null ? TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value) : null;
            lock (configLock)
            {
                SaveValue(key, invariant);
            }
        }

        public bool ContainsKey(string key)
        {
            lock (configLock)
            {
                if (configuration != null)
                {
                    return configuration.AppSettings.Settings[key] != null;
                }
                else
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, $"{CorruptedConfigurationWarning} Method 'ContainsKey', key:{key}");
                }
            }
            return false;
        }

        public void Remove(string key)
        {
            lock (configLock)
            {
                if (configuration != null)
                {
                    configuration.AppSettings.Settings.Remove(key);
                    SaveConfiguration();
                }
                else
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, $"{CorruptedConfigurationWarning} Method 'Remove', key:{key}");
                }
            }
        }

        private void SaveValue(string key, string value)
        {
            lock (configLock)
            {
                if (configuration != null)
                {
                    var element = configuration.AppSettings.Settings[key];
                    if (element == null)
                    {
                        configuration.AppSettings.Settings.Add(key, value);
                    }
                    else
                    {
                        element.Value = value;
                    }
                    SaveConfiguration();
                }
                else
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, $"{CorruptedConfigurationWarning} Method 'SaveValue', key:{key}, value:{value}");
                }
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                configuration.Save();
            }
            catch (ConfigurationErrorsException e)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, $"Configuration file can't be saved. Failure reason: {e.Message}");
            }
        }

        private static Configuration OpenConfiguration()
        {
            // Get new config path.
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            var userConfigPath = Path.GetDirectoryName(userConfig.FilePath);

            // Don't have AppCenter.config be reset on each app assembly version, use parent directory.
            var parentDirectory = Path.GetDirectoryName(userConfigPath);
            if (parentDirectory != null)
            {
                userConfigPath = parentDirectory;
            }
            FilePath = Path.Combine(userConfigPath, FileName);

            // If old path exists, migrate.
            try
            {
                // Get old config path.
                var oldLocation = Assembly.GetExecutingAssembly().Location;
                var oldPath = Path.Combine(Path.GetDirectoryName(oldLocation), FileName);
                if (File.Exists(oldPath))
                {
                    // Delete old file if a new one already exists.
                    if (File.Exists(FilePath))
                    {
                        File.Delete(oldPath);
                    }

                    // Or migrate by moving if no new file yet.
                    else
                    {
                        File.Move(oldPath, FilePath);
                    }
                }
            }
            catch (Exception e)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Could not check/migrate old config file", e);
            }

            // Open the configuration (with the new file path).
            var executionFileMap = new ExeConfigurationFileMap { ExeConfigFilename = FilePath };
            return ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);
        }

        private void RestoreConfigurationFile()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                    configuration = OpenConfiguration();
                    AppCenterLog.Info(AppCenterLog.LogTag, "Configuration is successfully restored.");
                }
            }
            catch (Exception e)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Could not restore configuration.", e);
            }
        }
    }
}
