using Foundation;

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

// @interface SNMAnalytics : SNMFeatureAbstract
[BaseType (typeof(SNMFeatureAbstract))]
interface SNMAnalytics
{
	// +(void)trackEvent:(NSString *)eventName;
	[Static]
	[Export ("trackEvent:")]
	void TrackEvent (string eventName);

	// +(void)trackEvent:(NSString *)eventName withProperties:(NSDictionary *)properties;
	[Static]
	[Export ("trackEvent:withProperties:")]
	void TrackEvent (string eventName, NSDictionary properties);

	// +(void)trackPage:(NSString *)pageName;
	[Static]
	[Export ("trackPage:")]
	void TrackPage (string pageName);

	// +(void)trackPage:(NSString *)pageName withProperties:(NSDictionary *)properties;
	[Static]
	[Export ("trackPage:withProperties:")]
	void TrackPage (string pageName, NSDictionary properties);

	// +(void)setAutoPageTrackingEnabled:(BOOL)isEnabled;
	[Static]
	[Export ("setAutoPageTrackingEnabled:")]
	void SetAutoPageTrackingEnabled (bool isEnabled);

	// +(BOOL)isAutoPageTrackingEnabled;
	[Static]
	[Export ("isAutoPageTrackingEnabled")]
	[Verify (MethodToProperty)]
	bool IsAutoPageTrackingEnabled { get; }
}
