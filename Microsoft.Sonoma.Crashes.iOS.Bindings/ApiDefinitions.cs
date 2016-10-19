using System;
using Foundation;
using ObjCRuntime;
using SonomaCrashes;

// @interface SNMErrorReport : NSObject
[BaseType (typeof(NSObject))]
interface SNMErrorReport
{
	// @property (readonly, nonatomic) NSString * incidentIdentifier;
	[Export ("incidentIdentifier")]
	string IncidentIdentifier { get; }

	// @property (readonly, nonatomic) NSString * reporterKey;
	[Export ("reporterKey")]
	string ReporterKey { get; }

	// @property (readonly, nonatomic) NSString * signal;
	[Export ("signal")]
	string Signal { get; }

	// @property (readonly, nonatomic) NSString * exceptionName;
	[Export ("exceptionName")]
	string ExceptionName { get; }

	// @property (readonly, nonatomic) NSString * exceptionReason;
	[Export ("exceptionReason")]
	string ExceptionReason { get; }

	// @property (readonly, nonatomic, strong) NSDate * appStartTime;
	[Export ("appStartTime", ArgumentSemantic.Strong)]
	NSDate AppStartTime { get; }

	// @property (readonly, nonatomic, strong) NSDate * appErrorTime;
	[Export ("appErrorTime", ArgumentSemantic.Strong)]
	NSDate AppErrorTime { get; }

	// @property (readonly, nonatomic) SNMDevice * device;
	[Export ("device")]
	SNMDevice Device { get; }

	// @property (readonly, assign, nonatomic) NSUInteger appProcessIdentifier;
	[Export ("appProcessIdentifier")]
	nuint AppProcessIdentifier { get; }

	// -(BOOL)isAppKill;
	[Export ("isAppKill")]
	[Verify (MethodToProperty)]
	bool IsAppKill { get; }
}

// @protocol SNMFeature <NSObject>
[Protocol, Model]
[BaseType (typeof(NSObject))]
interface SNMFeature
{
	// @required +(void)setEnabled:(BOOL)isEnabled;
	[Static, Abstract]
	[Export ("setEnabled:")]
	void SetEnabled (bool isEnabled);

	// @required +(BOOL)isEnabled;
	[Static, Abstract]
	[Export ("isEnabled")]
	[Verify (MethodToProperty)]
	bool IsEnabled { get; }
}

// @interface SNMFeatureAbstract : NSObject <SNMFeature>
[BaseType (typeof(NSObject))]
interface SNMFeatureAbstract : ISNMFeature
{
}

// typedef void (^SNMUserConfirmationHandler)(NSArray<SNMErrorReport *> * _Nonnull);
delegate void SNMUserConfirmationHandler (SNMErrorReport[] arg0);

// @interface SNMCrashes : SNMFeatureAbstract
[BaseType (typeof(SNMFeatureAbstract))]
interface SNMCrashes
{
	// +(void)generateTestCrash;
	[Static]
	[Export ("generateTestCrash")]
	void GenerateTestCrash ();

	// +(BOOL)hasCrashedInLastSession;
	[Static]
	[Export ("hasCrashedInLastSession")]
	[Verify (MethodToProperty)]
	bool HasCrashedInLastSession { get; }

	// +(SNMErrorReport * _Nullable)lastSessionCrashReport;
	[Static]
	[NullAllowed, Export ("lastSessionCrashReport")]
	[Verify (MethodToProperty)]
	SNMErrorReport LastSessionCrashReport { get; }
}
