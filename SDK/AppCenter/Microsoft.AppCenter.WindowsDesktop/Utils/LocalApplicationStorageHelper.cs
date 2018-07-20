using System;

namespace Microsoft.AppCenter.Utils
{
    public class LocalApplicationStorageHelper
    {
        public static string LocalApplicationStoragePath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
