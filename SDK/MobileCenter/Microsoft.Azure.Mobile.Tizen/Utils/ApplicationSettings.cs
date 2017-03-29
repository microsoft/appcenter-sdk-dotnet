using System.Collections.Generic;

namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationSettings : IApplicationSettings
    {
        public static Dictionary<string, object> LocalSettings = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                // return Windows.Storage.ApplicationData.Current.LocalSettings.Values[key];
                return LocalSettings[key];
            }

            set
            {
                // Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = value;
                LocalSettings[key] = value;
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object result;
            bool found = LocalSettings.TryGetValue(key, out result);
            if (!found)
            {
                this[key] = defaultValue;
                return defaultValue;
            }
            return (T)result;
        }

        public void Remove(string key)
        {
            // Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(key);
            LocalSettings.Remove(key);
        }
    }
}
