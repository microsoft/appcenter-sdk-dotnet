// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter;

namespace Contoso.Android.Puppet
{
    using Result = global::Android.App.Result;

    public class AppCenterFragment : PageFragment
    {
        const string LogTag = "AppCenterXamarinPuppet";

        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions = new Dictionary<LogLevel, Action<string, string>> {
            { LogLevel.Verbose, AppCenterLog.Verbose },
            { LogLevel.Debug, AppCenterLog.Debug },
            { LogLevel.Info, AppCenterLog.Info },
            { LogLevel.Warn, AppCenterLog.Warn },
            { LogLevel.Error, AppCenterLog.Error }
        };
        private static readonly IDictionary<LogLevel, string> LogLevelNames = new Dictionary<LogLevel, string> {
            { LogLevel.Verbose, Constants.Verbose },
            { LogLevel.Debug, Constants.Debug },
            { LogLevel.Info, Constants.Info },
            { LogLevel.Warn, Constants.Warning },
            { LogLevel.Error, Constants.Error }
        };
        private LogLevel mLogWriteLevel = LogLevel.Verbose;

        private Switch AppCenterEnabledSwitch;
        private Switch AppcenterNetworkRequestsAllowedSwitch;
        private TextView LogLevelLabel;
        private EditText LogWriteMessageText;
        private EditText LogWriteTagText;
        private TextView LogWriteLevelLabel;
        private Button LogWriteButton;
        private EditText UserIdText;
        private Button SaveStorageSizeButton;
        private EditText StorageSizeText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.AppCenter, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Find views.
            AppCenterEnabledSwitch = view.FindViewById(Resource.Id.enabled_app_center) as Switch;
            AppcenterNetworkRequestsAllowedSwitch = view.FindViewById(Resource.Id.appcenter_network_requests_allowed) as Switch;

            LogLevelLabel = view.FindViewById(Resource.Id.log_level) as TextView;
            LogWriteMessageText = view.FindViewById(Resource.Id.write_log_message) as EditText;
            LogWriteTagText = view.FindViewById(Resource.Id.write_log_tag) as EditText;
            LogWriteLevelLabel = view.FindViewById(Resource.Id.write_log_level) as TextView;
            LogWriteButton = view.FindViewById(Resource.Id.write_log) as Button;
            UserIdText = view.FindViewById(Resource.Id.write_user_id) as EditText;
            SaveStorageSizeButton = view.FindViewById(Resource.Id.save_storage_size) as Button;
            StorageSizeText = view.FindViewById(Resource.Id.write_storage_size) as EditText;

            // Subscribe to events.
            AppCenterEnabledSwitch.CheckedChange += UpdateEnabled;
            AppcenterNetworkRequestsAllowedSwitch.CheckedChange += NetworkRequestAllowedChange;
            ((View)LogLevelLabel.Parent).Click += LogLevelClicked;
            ((View)LogWriteLevelLabel.Parent).Click += LogWriteLevelClicked;
            LogWriteButton.Click += WriteLog;
            SaveStorageSizeButton.Click += SaveStorageSize;
            UserIdText.KeyPress += UserIdTextKeyPressedHandler;

            // Set max storage size value.
            var prefs = Context.GetSharedPreferences("AppCenter", FileCreationMode.Private);
            var storageSizeValue = prefs.GetLong(Constants.StorageSizeKey, 0);
            if (storageSizeValue > 0)
            {
                StorageSizeText.Text = storageSizeValue.ToString();
            }
            UpdateState();
        }

        protected override async void UpdateState()
        {
            AppCenterEnabledSwitch.CheckedChange -= UpdateEnabled;
            AppCenterEnabledSwitch.Checked = await AppCenter.IsEnabledAsync();
            AppCenterEnabledSwitch.CheckedChange += UpdateEnabled;
            AppcenterNetworkRequestsAllowedSwitch.CheckedChange -= NetworkRequestAllowedChange;
            AppcenterNetworkRequestsAllowedSwitch.Checked = AppCenter.IsNetworkRequestsAllowed;
            AppcenterNetworkRequestsAllowedSwitch.CheckedChange += NetworkRequestAllowedChange;
            LogLevelLabel.Text = LogLevelNames[AppCenter.LogLevel];
            LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != (int)Result.Ok || data == null)
            {
                return;
            }
            var logLevel = (LogLevel)data.GetIntExtra("log_level", (int)LogLevel.Verbose);
            switch (requestCode)
            {
                case 0:
                    AppCenter.LogLevel = logLevel;
                    LogLevelLabel.Text = LogLevelNames[AppCenter.LogLevel];
                    break;
                case 1:
                    mLogWriteLevel = logLevel;
                    LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
                    break;
            }
        }

        private async void UpdateEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.IsChecked);
            AppCenterEnabledSwitch.Checked = await AppCenter.IsEnabledAsync();
        }

        private void NetworkRequestAllowedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            AppCenter.IsNetworkRequestsAllowed = e.IsChecked;
            var prefs = Context.GetSharedPreferences("AppCenter", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutBoolean(Constants.AllowNetworkRequests, e.IsChecked);
            prefEditor.Commit();
        }

        private void LogLevelClicked(object sender, EventArgs e)
        {
            var intent = new Intent(Activity.ApplicationContext, typeof(LogLevelActivity));
            StartActivityForResult(intent, 0);
        }

        private void LogWriteLevelClicked(object sender, EventArgs e)
        {
            var intent = new Intent(Activity.ApplicationContext, typeof(LogLevelActivity));
            StartActivityForResult(intent, 1);
        }

        private void UserIdTextKeyPressedHandler(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Up)
            {
                var text = string.IsNullOrEmpty(UserIdText.Text) ? null : UserIdText.Text;
                AppCenter.SetUserId(text);
            }
        }

        private void WriteLog(object sender, EventArgs e)
        {
            string message = LogWriteMessageText.Text;
            string tag = LogWriteTagText.Text;
            LogFunctions[mLogWriteLevel](tag, message);
        }

        private void SaveStorageSize(object sender, EventArgs e)
        {
            var inputText = StorageSizeText.Text;
            if (long.TryParse(inputText, out var result))
            {
                AppCenter.SetMaxStorageSizeAsync(result);
                var prefs = Context.GetSharedPreferences("AppCenter", FileCreationMode.Private);
                var prefEditor = prefs.Edit();
                prefEditor.PutLong(Constants.StorageSizeKey, result);
                prefEditor.Commit();
            }
            else
            {
                AppCenterLog.Error(LogTag, "Wrong number value for the max storage size.");
            }
        }
    }
}
