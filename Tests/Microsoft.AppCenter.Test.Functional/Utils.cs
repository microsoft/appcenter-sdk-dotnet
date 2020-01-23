// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Microsoft.AppCenter.Test.Functional
{
    public static class Utils
    {
        private const string LogTag = "TestUtils";

        public static void DeleteDatabase()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.iOS:
                    DeleteIos();
                    break;
                case Xamarin.Forms.Device.Android:
                    DeleteAndroid();
                    break;
                default:
                    break;
            }
        }

        private static void DeleteAndroid()
        {
            try
            {
                var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "../databases");
                Directory.Delete(dbFolder, true);
            } catch (DirectoryNotFoundException e)
            {
                AppCenterLog.Error(LogTag, $"Android DB not found {e.ToString()}.");
            }
            catch (IOException e)
            {
                AppCenterLog.Error(LogTag, $"Encountered IOException when tried to delete Android DB: {e.ToString()}.");
            }
        }

        private static void DeleteIos()
        {
            try
            {
                var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "../Library/Application Support/com.microsoft.appcenter");
                Directory.Delete(dbFolder, true);
            }
            catch (DirectoryNotFoundException e)
            {
                AppCenterLog.Error(LogTag, $"iOS DB not found {e.ToString()}.");
            }
            catch (IOException e)
            {
                AppCenterLog.Error(LogTag, $"Encountered IOException when tried to delete iOS DB: {e.ToString()}.");
            }
        }
    }
}
