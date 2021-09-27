namespace Microsoft.AppCenter
{
    using MacOSMessageProvider = Microsoft.AppCenter.MacOS.Bindings.MSACLogMessageProvider;
    using MacOSLogger = Microsoft.AppCenter.MacOS.Bindings.MSACWrapperLogger;
	using MacOSLogLevel = Microsoft.AppCenter.MacOS.Bindings.MSACLogLevel;

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
            MacOSMessageProvider msg_provider = () => { return message; };
            MacOSLogger.MSACWrapperLog(msg_provider, tag, MacOSLogLevel.Verbose);
        }

        public static void Debug(string tag, string message)
        {
            MacOSMessageProvider msg_provider = () => { return message; };
            MacOSLogger.MSACWrapperLog(msg_provider, tag, MacOSLogLevel.Debug);
        }

        public static void Info(string tag, string message)
        {
            MacOSMessageProvider msg_provider = () => { return message; };
            MacOSLogger.MSACWrapperLog(msg_provider, tag, MacOSLogLevel.Info);
        }

        public static void Warn(string tag, string message)
        {
            MacOSMessageProvider msg_provider = () => { return message; };
            MacOSLogger.MSACWrapperLog(msg_provider, tag, MacOSLogLevel.Warning);
        }

        public static void Error(string tag, string message)
        {
            MacOSMessageProvider msg_provider = () => { return message; };
            MacOSLogger.MSACWrapperLog(msg_provider, tag, MacOSLogLevel.Error);
        }

        public static void Assert(string tag, string message)
        {
            MacOSMessageProvider msg_provider = () => { return message; };
            MacOSLogger.MSACWrapperLog(msg_provider, tag, MacOSLogLevel.Assert);
        }
    }
}