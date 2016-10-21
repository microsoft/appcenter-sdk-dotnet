using System;
using Foundation;
using ObjCRuntime;


namespace Microsoft.Sonoma.Crashes.iOS.Bindings
{
	[BaseType(typeof(NSObject))]
	interface SNMDevice
	{
		//need to flesh this out?
	}

	// @interface SNMErrorReport : NSObject
	[BaseType(typeof(NSObject))]
	interface SNMErrorReport
	{
		// @property (readonly, nonatomic) NSString * incidentIdentifier;
		[Export("incidentIdentifier")]
		string IncidentIdentifier { get; }

		// @property (readonly, nonatomic) NSString * reporterKey;
		[Export("reporterKey")]
		string ReporterKey { get; }

		// @property (readonly, nonatomic) NSString * signal;
		[Export("signal")]
		string Signal { get; }

		// @property (readonly, nonatomic) NSString * exceptionName;
		[Export("exceptionName")]
		string ExceptionName { get; }

		// @property (readonly, nonatomic) NSString * exceptionReason;
		[Export("exceptionReason")]
		string ExceptionReason { get; }

		// @property (readonly, nonatomic, strong) NSDate * appStartTime;
		[Export("appStartTime", ArgumentSemantic.Strong)]
		NSDate AppStartTime { get; }

		// @property (readonly, nonatomic, strong) NSDate * appErrorTime;
		[Export("appErrorTime", ArgumentSemantic.Strong)]
		NSDate AppErrorTime { get; }

		// @property (readonly, nonatomic) SNMDevice * device;
		[Export("device")]
		SNMDevice Device { get; }

		// @property (readonly, assign, nonatomic) NSUInteger appProcessIdentifier;
		[Export("appProcessIdentifier")]
		nuint AppProcessIdentifier { get; }

		// -(BOOL)isAppKill;
		[Export("isAppKill")]
		//[Verify(MethodToProperty)]
		bool IsAppKill { get; }
	}

	// typedef void (^SNMUserConfirmationHandler)(NSArray<SNMErrorReport *> * _Nonnull);
	delegate void SNMUserConfirmationHandler(SNMErrorReport[] arg0);

	// @interface SNMCrashes : SNMFeatureAbstract
	[BaseType(typeof(Core.iOS.Bindings.SNMFeatureAbstract))]
	interface SNMCrashes
	{
		// +(void)generateTestCrash;
		[Static]
		[Export("generateTestCrash")]
		void GenerateTestCrash();

		// +(BOOL)hasCrashedInLastSession;
		[Static]
		[Export("hasCrashedInLastSession")]
		//[Verify(MethodToProperty)]
		bool HasCrashedInLastSession { get; }

		// +(SNMErrorReport * _Nullable)lastSessionCrashReport;
		[Static]
		[NullAllowed, Export("lastSessionCrashReport")]
		//[Verify(MethodToProperty)]
		SNMErrorReport LastSessionCrashReport { get; }
	}
}