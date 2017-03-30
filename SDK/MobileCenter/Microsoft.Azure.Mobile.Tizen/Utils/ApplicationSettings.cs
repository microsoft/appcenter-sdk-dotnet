using System.Collections.Generic;
using Xamarin.Forms;


namespace Microsoft.Azure.Mobile.Utils
{
    public class ApplicationSettings : IApplicationSettings
    {
        public static IDictionary<string, object> LocalSettings = Application.Current.Properties;

        public object this[string key]
        {
            get
            {
                return LocalSettings[key];
            }

            set
            {
                LocalSettings[key] = value;
                Application.Current.SavePropertiesAsync();
            }
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            object result;
            bool found = LocalSettings.TryGetValue(key, out result);
            if (!found)
            {
                this[key] = defaultValue;
                Application.Current.SavePropertiesAsync();
                return defaultValue;
            }
            return (T)result;
        }

        public void Remove(string key)
        {
            LocalSettings.Remove(key);
            Application.Current.SavePropertiesAsync();
        }
    }
}
