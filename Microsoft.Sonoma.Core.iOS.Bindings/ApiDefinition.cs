using Foundation;
using ObjCRuntime;
using System;

namespace Microsoft.Sonoma.Core.iOS.Bindings
{
	//TODO SNMFeature must be reconciled with the same interface found elsewhere
	interface ISNMFeature { }


	// typedef NSString * (^SNMLogMessageProvider)();
	delegate string SNMLogMessageProvider();

	//TODO this needs to be fixed. might require a special implementation.
	//unsafe delegate void SNMLogHandler(SNMLogMessageProvider arg0, SNMLogLevel arg1, sbyte* arg2, sbyte* arg3, uint arg4);

	//TODO this seems to work when replacing sbyte* with IntPtr...
	// typedef void (^SNMLogHandler)(SNMLogMessageProvider, SNMLogLevel, const char *, const char *, uint);
	unsafe delegate void SNMLogHandler(SNMLogMessageProvider arg0, SNMLogLevel arg1, IntPtr arg2, IntPtr arg3, uint arg4);

	// @interface SNMSonoma : NSObject
	[BaseType(typeof(NSObject))]
	interface SNMSonoma
	{
		// +(instancetype)sharedInstance;
		[Static]
		[Export("sharedInstance")]
		SNMSonoma SharedInstance();

		// +(void)start:(NSString *)appSecret;
		[Static]
		[Export("start:")]
		void Start(string appSecret);

		// +(void)start:(NSString *)appSecret withFeatures:(NSArray<Class> *)features;
		[Static]
		[Export("start:withFeatures:")]
		void Start(string appSecret, Class[] features);

		// +(void)startFeature:(Class)feature;
		[Static]
		[Export("startFeature:")]
		void StartFeature(Class feature);

		// +(BOOL)isInitialized;
		[Static]
		[Export("isInitialized")]
		bool IsInitialized();

		// +(void)setServerUrl:(NSString *)serverUrl;
		[Static]
		[Export("setServerUrl:")]
		void SetServerUrl(string serverUrl);

		// +(void)setEnabled:(BOOL)isEnabled;
		[Static]
		[Export("setEnabled:")]
		void SetEnabled(bool isEnabled);

		// +(BOOL)isEnabled;
		[Static]
		[Export("isEnabled")]
		bool IsEnabled();

		// +(SNMLogLevel)logLevel;
		// +(void)setLogLevel:(SNMLogLevel)logLevel;
		[Static]
		[Export("logLevel")]
		SNMLogLevel LogLevel();

		[Static]
		[Export("setLogLevel:")]
		void SetLogLevel(SNMLogLevel logLevel);

		//TODO this needs to be fixed
		//// +(void)setLogHandler:(SNMLogHandler)logHandler;
		[Static]
		[Export("setLogHandler:")]
		void SetLogHandler(SNMLogHandler logHandler);

		// +(NSUUID *)installId;
		[Static]
		[Export("installId")]
		NSUuid InstallId();

		// +(BOOL)isDebuggerAttached;
		[Static]
		[Export("isDebuggerAttached")]
		bool IsDebuggerAttached();

		// +(BOOL)isDebuggerAttached;
		[Static]
		[Export("randomaerpsdcjn")]
		bool random();
	}

	//TODO make it so that snmfeature stuff is only defined in core, and referenced by the other two
	//// @protocol SNMFeature <NSObject>
	//[Protocol, Model]
	//[BaseType(typeof(NSObject))]
	//interface SNMFeature
	//{
	//	// @required +(BOOL)isEnabled;
	//	[Static, Abstract]
	//	[Export("isEnabled")]
	//	bool Enabled { get; set; }
	//}
	//// @interface SNMFeatureAbstract : NSObject <SNMFeature>
	//[BaseType(typeof(SNMFeature))]
	//interface SNMFeatureAbstract : ISNMFeature
	//{
	//	//[Static, Override, Abstract]
	//	//[Export("isEnabled")]
	//	//bool Enabled { get; set; }
	//}
}