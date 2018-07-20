using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.AppCenter.Utils
{
    public class DefaultApplicationSettings : IApplicationSettings
    {
        private readonly object _configLock = new object();
        private readonly IDictionary<string, string> _current;

        private static readonly string ConfigPath =
            Path.Combine(LocalApplicationStorageHelper.LocalApplicationStoragePath, "AppCenter.config");

        public DefaultApplicationSettings()
        {
            _current = ReadAll();
        }
        
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            lock (_configLock)
            {
                if (_current.ContainsKey(key))
                {
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(_current[key]);
                }
            }
            SetValue(key, defaultValue);
            return defaultValue;
        }

        public void SetValue(string key, object value)
        {
            var invariant = value != null ? TypeDescriptor.GetConverter(value.GetType()).ConvertToInvariantString(value) : null;
            lock (_configLock)
            {
                _current[key] = invariant;
                SaveValue(key, invariant);
            }
        }

        public bool ContainsKey(string key)
        {
            lock (_configLock)
            {
                return _current.ContainsKey(key);
            }
        }

        public void Remove(string key)
        {
            lock (_configLock)
            {
                _current.Remove(key);
                var config = OpenConfiguration();
                config.AppSettings.Settings.Remove(key);
                config.Save();
            }
        }

        private void SaveValue(string key, string value)
        {
            lock (_configLock)
            {
                var config = OpenConfiguration();
                var element = config.AppSettings.Settings[key];
                if (element == null)
                {
                    config.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    element.Value = value;
                }
                config.Save();
            }
        }

        private static IDictionary<string, string> ReadAll()
        {
            var config = OpenConfiguration();
            return config.AppSettings.Settings.Cast<KeyValueConfigurationElement>().ToDictionary(e => e.Key, e => e.Value);
        }

        private static Configuration OpenConfiguration()
        {
            var executionFileMap = new ExeConfigurationFileMap { ExeConfigFilename = ConfigPath };
            return ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);
        }
    }
}
