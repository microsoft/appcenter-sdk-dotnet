// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;

namespace Microsoft.AppCenter.Analytics.iOS.Bindings
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
        [Export("enableManualSessionTracker")]
        void EnableManualSessionTracker();

        // + (void)startSession
        [Static]
        [Export("startSession")]
        void StartSession();

        //// +(void)trackPage:(NSString *)pageName;
        //[Static]
        //[Export("trackPage:")]
        //void TrackPage(string pageName);

        //// +(void)trackPage:(NSString *)pageName withProperties:(NSDictionary *)properties;
        //[Static]
        //[Export("trackPage:withProperties:")]
        //void TrackPage(string pageName, NSDictionary properties);

        //// +(void)setAutoPageTrackingEnabled:(BOOL)isEnabled;
        //[Static]
        //[Export("setAutoPageTrackingEnabled:")]
        //void SetAutoPageTrackingEnabled(bool isEnabled);

        //// +(BOOL)isAutoPageTrackingEnabled;
        //[Static]
        //[Export("isAutoPageTrackingEnabled")]
        //bool IsAutoPageTrackingEnabled();
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

        ////@optional - (void)analytics:(MSACAnalytics*)analytics willSendPageLog:(MSACPageLog*)pageLog;
        //[Export("analytics:willSendPageLog:")]
        //void WillSendPageLog(MSACAnalytics analytics, MSACPageLog pageLog);

        ////@optional - (void)analytics:(MSACAnalytics*)analytics didSucceedSendingPageLog:(MSACPageLog*)pageLog;
        //[Export("analytics:didSucceedSendingPageLog:")]
        //void DidSucceedSendingPageLog(MSACAnalytics analytics, MSACPageLog pageLog);

        ////@optional - (void)analytics:(MSACAnalytics*)analytics didFailSendingPageLog:(MSACPageLog*)pageLog withError:(NSError*)error;
        //[Export("analytics:didFailSendingPageLog:withError:")]
        //void DidFailSendingPageLog(MSACAnalytics analytics, MSACPageLog pageLog, NSError error);
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
