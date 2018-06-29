namespace Microsoft.AppCenter
{
    using macMessageProvider = Microsoft.AppCenter.macOS.Bindings.MSLogMessageProvider;
    using macLogger = Microsoft.AppCenter.macOS.Bindings.MSWrapperLogger;

	public static partial class AppCenterLog
	{
        /// <summary>
        /// The log tag for this SDK. All logs emitted at the SDK level will contain this tag.
        /// </summary>
        public static string LogTag { get; private set; }

        static AppCenterLog()
        {
            LogTag = "AppCenterXamarin";
        }

        public static void Verbose(string tag, string message)
		{
            macMessageProvider msg_provider = () => { return message; };
            macLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.macOS.Bindings.MSLogLevel.Verbose);
		}

		public static void Debug(string tag, string message)
		{
            macMessageProvider msg_provider = () => { return message; };
            macLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.macOS.Bindings.MSLogLevel.Debug);
		}

		public static void Info(string tag, string message)
		{
            macMessageProvider msg_provider = () => { return message; };
            macLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.macOS.Bindings.MSLogLevel.Info);
		}

		public static void Warn(string tag, string message)
		{
            macMessageProvider msg_provider = () => { return message; };
            macLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.macOS.Bindings.MSLogLevel.Warning);
		}

        public static void Error(string tag, string message)
        {
            macMessageProvider msg_provider = () => { return message; };
            macLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.macOS.Bindings.MSLogLevel.Error);
        }

        public static void Assert(string tag, string message)
        {
            macMessageProvider msg_provider = () => { return message; };
            macLogger.MSWrapperLog(msg_provider, tag, Microsoft.AppCenter.macOS.Bindings.MSLogLevel.Assert);
        }
	}
}
