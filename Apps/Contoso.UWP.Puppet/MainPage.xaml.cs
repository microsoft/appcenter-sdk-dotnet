// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Contoso.UtilClassLibrary;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Contoso.UWP.Puppet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Application.Current.UnhandledException += OnUnhandledException;
            object storageSize;
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue("StorageMaxSize", out storageSize))
            {
                StorageMaxSizeTextBox.Text = storageSize.ToString();
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = HandleExceptions.IsOn;
        }

        private void TrackEvent(object sender, RoutedEventArgs e)
        {
            Analytics.TrackEvent("Test");
        }

        private async void ThrowException(object sender, RoutedEventArgs e)
        {
            // This app is just for smoke testing.
            // Also this app uses min SDK version to 10240, which changes the .NET native generated code to have missing symbols for handled errors.
            // Handled errors in the forms app never hit that case because we need to use v16299 there.
            await GenerateComplexException(2);
        }

        private void ClassLibraryException(object sender, RoutedEventArgs e)
        {
            try
            {
                CrashUtils.BackgroundExceptionTask().RunSynchronously();
            }
            catch (Exception ex) when (HandleExceptions.IsOn)
            {
                Crashes.TrackError(ex);
            }
        }

        private async Task GenerateComplexException(int loop)
        {
            if (loop == 0)
            {
                try
                {
                    try
                    {
                        throw new ArgumentException("Hello, I'm an inner exception!");
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Hola! I'm an outer exception!", ex);
                    }
                }
                catch (Exception ex)
                {
                    if (HandleExceptions.IsOn)
                    {
                        Crashes.TrackError(ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                await Task.Run(() => { });
                await GenerateComplexException(loop - 1);
            }
        }

        private void StorageMaxSize_LostFocus(object sender, RoutedEventArgs e)
        {
            HandleStorageMaxSizeChange();
        }

        private void HandleStorageMaxSizeChange()
        {
            var storageSize = StorageMaxSizeTextBox.Text;
            var size = 10L * 1024 * 1024;
            long.TryParse(storageSize, out size);
            AppCenter.SetMaxStorageSizeAsync(size);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["StorageMaxSize"] = size;
        }

        private void StorageMaxSize_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                HandleStorageMaxSizeChange();
            }
        }
    }
}
