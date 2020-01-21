// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

namespace Microsoft.AppCenter.Test.Functional
{
    public static class Utils
    {
        private const string TAG = "TestUtils";

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
                AppCenterLog.Error(TAG, $"Android DB not found {e.ToString()}.");
            }
            catch (IOException e)
            {
                AppCenterLog.Error(TAG, $"Encountered IOException when tried to delete Android DB: {e.ToString()}.");
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
                AppCenterLog.Error(TAG, $"iOS DB not found {e.ToString()}.");
            }
            catch (IOException e)
            {
                AppCenterLog.Error(TAG, $"Encountered IOException when tried to delete iOS DB: {e.ToString()}.");
            }
        }
    }
}
