using System;
using System.IO;

namespace Microsoft.AppCenter.Test.Functional
{
    public static class Utils
    {
        public static void deleteDatabase()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.iOS:
                    deleteIos();
                    break;
                case Xamarin.Forms.Device.Android:
                    deleteAndroid();
                    break;
                default:
                    break;
            }
        }

        private static void deleteAndroid()
        {
            try
            {
                var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "../databases");
                Directory.Delete(dbFolder, true);
            } catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"Android DB not found {e.ToString()}.");
            }
        }

        private static void deleteIos()
        {
            try
            {
                var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "../Library/Application Support/com.microsoft.appcenter");
                Directory.Delete(dbFolder, true);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"iOS DB not found {e.ToString()}.");
            }
        }
    }
}
