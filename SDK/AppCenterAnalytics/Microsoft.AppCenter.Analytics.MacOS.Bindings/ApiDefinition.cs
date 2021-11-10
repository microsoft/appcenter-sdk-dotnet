using Foundation;

namespace Microsoft.AppCenter.Analytics.MacOS.Bindings
{
    // @interface MSACAnalytics : MSACService
    [BaseType(typeof(NSObject))]
    interface MSACAnalytics
    {
        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // +(void)pause;
        [Static]
        [Export("pause")]
        void Pause();

        // +(void)resume;
        [Static]
        [Export("resume")]
        void Resume();

        // +(void)trackEvent:(NSString *)eventName;
        [Static]
        [Export("trackEvent:")]
        void TrackEvent([NullAllowed] string eventName);

        // +(void)trackEvent:(NSString *)eventName withProperties:(NSDictionary *)properties;
        [Static]
        [Export("trackEvent:withProperties:")]
        void TrackEvent([NullAllowed] string eventName, [NullAllowed] NSDictionary properties);

        // +(void)setDelegate:(id<MSACAnalyticsDelegate> _Nullable)delegate;
        [Static]
        [Export("setDelegate:")]
        void SetDelegate([NullAllowed] MSACAnalyticsDelegate analyticsDelegate);

        // + (void)resetSharedInstance
        [Static]
        [Export("resetSharedInstance")]
        void ResetSharedInstance();

        // + (void)enableManualSessionTracker
        [Static]
        [Export("enableManualSessionTracker:")]
        void EnableManualSessionTracker();

        // + (void)startSession
        [Static]
        [Export("startSession")]
        void StartSession();
    }

    // @protocol MSACAnalyticsDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSACAnalyticsDelegate
    {
        //@optional - (void)analytics:(MSACAnalytics*)analytics willSendEventLog:(MSACEventLog*)eventLog;
        [Export("analytics:willSendEventLog:")]
        void WillSendEventLog(MSACAnalytics analytics, MSACEventLog eventLog);

        //@optional - (void)analytics:(MSACAnalytics*)analytics didSucceedSendingEventLog:(MSACEventLog*)eventLog;
        [Export("analytics:didSucceedSendingEventLog:")]
        void DidSucceedSendingEventLog(MSACAnalytics analytics, MSACEventLog eventLog);

        //@optional - (void)analytics:(MSACAnalytics*)analytics didFailSendingEventLog:(MSACEventLog*)eventLog withError:(NSError*)error;
        [Export("analytics:didFailSendingEventLog:withError:")]
        void DidFailSendingEventLog(MSACAnalytics analytics, MSACEventLog eventLog, NSError error);
    }

    // @interface MSACLogWithProperties : MSACAbstractLog
    [BaseType(typeof(NSObject))]
    interface MSACLogWithProperties
    {
        //@property(nonatomic) NSDictionary<NSString*, NSString*>* properties;
        [Export("properties")]
        NSDictionary<NSString, NSString> Properties { get; set; }
    }

    //@interface MSACEventLog : MSACLogWithProperties
    [BaseType(typeof(MSACLogWithProperties))]
    interface MSACEventLog : MSACLogWithProperties
    {
        //@property(nonatomic) NSString *eventId;
        [Export("eventId")]
        string EventId { get; set; }

        //@property(nonatomic) NSString *name;
        [Export("name")]
        string Name { get; set; }
    }

    //@interface MSACPageLog : MSACLogWithProperties
    [BaseType(typeof(MSACLogWithProperties))]
    interface MSACPageLog : MSACLogWithProperties
    {
        //@property(nonatomic) NSString *name;
        [Export("name")]
        string Name { get; set; }
    }
}