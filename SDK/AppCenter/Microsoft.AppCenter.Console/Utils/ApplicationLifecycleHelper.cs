// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Utils
{
	public class ApplicationLifecycleHelper : IApplicationLifecycleHelper
	{
		// Singleton instance of ApplicationLifecycleHelper
		private static IApplicationLifecycleHelper _instance;

		public static IApplicationLifecycleHelper Instance
		{
			get => _instance ??= new ApplicationLifecycleHelper();
		}

		private ApplicationLifecycleHelper()
		{
			Enabled = true;
			AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) => { UnhandledExceptionOccurred?.Invoke(sender, new UnhandledExceptionOccurredEventArgs((Exception) eventArgs.ExceptionObject)); };
		}

		private bool _enabled;

		public bool IsSuspended { get; } = false;
		public event EventHandler ApplicationSuspended;
		public event EventHandler ApplicationResuming;
		public event EventHandler<UnhandledExceptionOccurredEventArgs> UnhandledExceptionOccurred;

		public bool Enabled
		{
			set => _enabled = value;
		}
	}
}