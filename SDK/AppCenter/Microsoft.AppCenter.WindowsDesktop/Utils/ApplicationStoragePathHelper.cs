using System.IO;
using System.Windows.Forms;

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationStoragePathHelper
    {
        // Use parent directory because the folder itself is the version of the app.
        // Application.UserAppDataPath might look like "C:\Users\{username}\AppData\Roaming\Contoso\Puppet\1.8.1-SNAPSHOT".
        public static string ApplicationStoragePath =>
            Directory.GetParent(Application.UserAppDataPath).ToString();
    }
}
