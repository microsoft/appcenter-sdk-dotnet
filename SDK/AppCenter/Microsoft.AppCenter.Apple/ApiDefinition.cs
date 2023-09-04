// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;
using ObjCRuntime;
using System;

namespace Microsoft.AppCenter.Apple.Bindings
{
    // typedef (void (^)(BOOL))completionHandler();
    delegate void MSACSetLogLevelCompletionHandlerCallback(bool result);

    // typedef NSString * (^MSACLogMessageProvider)();
    delegate string MSACLogMessageProvider();

    // typedef void (^MSACLogHandler)(MSACLogMessageProvider, MSACLogLevel, const char *, const char *, uint);
    unsafe delegate void MSACLogHandler([CCallback] MSACLogMessageProvider arg0, MSACLogLevel arg1, IntPtr arg2, IntPtr arg3, uint arg4);
    //Note: Objective Sharpie tried to bind the above as:
    //  unsafe delegate void MSACLogHandler(MSACLogMessageProvider arg0, MSACLogLevel arg1, string arg2, sbyte* arg3, sbyte* arg4, uint arg5);
    //But trying to use it as given gave an error.

    // @interface MSACWrapperSdk : NSObject
    [BaseType(typeof(NSObject))]
    interface MSACWrapperSdk
    {
        // @property (readonly, nonatomic) NSString * wrapperSdkVersion;
        [Export("wrapperSdkVersion")]
        string WrapperSdkVersion { get; }

        // @property (readonly, nonatomic) NSString * wrapperSdkName;
        [Export("wrapperSdkName")]
        string WrapperSdkName { get; }

        // @property (readonly, nonatomic) NSString * wrapperRuntimeVersion;
        [Export("wrapperRuntimeVersion")]
        string WrapperRuntimeVersion { get; }

        // @property (readonly, nonatomic) NSString * liveUpdateReleaseLabel;
        [Export("liveUpdateReleaseLabel")]
        string LiveUpdateReleaseLabel { get; }

        // @property (readonly, nonatomic) NSString * liveUpdateDeploymentKey;
        [Export("liveUpdateDeploymentKey")]
        string LiveUpdateDeploymentKey { get; }

        // @property (readonly, nonatomic) NSString * liveUpdatePackageHash;
        [Export("liveUpdatePackageHash")]
        string LiveUpdatePackageHash { get; }

        // -(BOOL)isEqual:(MSACWrapperSdk *)wrapperSdk;
        [Export("isEqual:")]
        bool IsEqual(MSACWrapperSdk wrapperSdk);

        // initWithWrapperSdkVersion:(NSString *)wrapperSdkVersion wrapperSdkName:(NSString *)wrapperSdkName wrapperRuntimeVersion:(NSString*)wrapperRuntimeVersion liveUpdateReleaseLabel:(NSString*)liveUpdateReleaseLabel liveUpdateDeploymentKey:(NSString*)liveUpdateDeploymentKey liveUpdatePackageHash:(NSString*)liveUpdatePackageHash;
        [Export("initWithWrapperSdkVersion:wrapperSdkName:wrapperRuntimeVersion:liveUpdateReleaseLabel:liveUpdateDeploymentKey:liveUpdatePackageHash:")]
        IntPtr Constructor([NullAllowed] string wrapperSdkVersion, [NullAllowed] string wrapperSdkName, [NullAllowed] string wrapperRuntimeVersion, [NullAllowed] string liveUpdateReleaseLabel, [NullAllowed] string liveUpdateDeploymentKey, [NullAllowed] string liveUpdatePackageHash);
    }

    // @interface MSACKeychainUtil : NSObject
    [BaseType(typeof(NSObject))]
    interface MSACKeychainUtil
    {
        // + (BOOL)storeString:(NSString *)string forKey:(NSString *)key;
        [Static]
        [Export("storeString:forKey:")]
        void StoreString(NSString value, NSString key);

        // + (NSString *_Nullable)stringForKey:(NSString *)key statusCode:(OSStatus *_Nullable)statusCode;
        [Static]
        [Export("stringForKey:statusCode:")]
        NSString StringForKey(NSString key, out int errorCode);

        // + (BOOL) clear;
        [Static]
        [Export("clear")]
        bool Clear();
    }

    // @interface MSACDevice : MSACWrapperSdk
    [BaseType(typeof(MSACWrapperSdk))]
    interface MSACDevice
    {
        // @property (readonly, nonatomic) NSString * sdkName;
        [Export("sdkName")]
        string SdkName { get; }

        // @property (readonly, nonatomic) NSString * sdkVersion;
        [Export("sdkVersion")]
        string SdkVersion { get; }

        // @property (readonly, nonatomic) NSString * model;
        [Export("model")]
        string Model { get; }

        // @property (readonly, nonatomic) NSString * oemName;
        [Export("oemName")]
        string OemName { get; }

        // @property (readonly, nonatomic) NSString * osName;
        [Export("osName")]
        string OsName { get; }

        // @property (readonly, nonatomic) NSString * osVersion;
        [Export("osVersion")]
        string OsVersion { get; }

        // @property (readonly, nonatomic) NSString * osBuild;
        [Export("osBuild")]
        string OsBuild { get; }

        // @property (readonly, nonatomic) NSNumber * osApiLevel;
        [Export("osApiLevel")]
        NSNumber OsApiLevel { get; }

        // @property (readonly, nonatomic) NSString * locale;
        [Export("locale")]
        string Locale { get; }

        // @property (readonly, nonatomic) NSNumber * timeZoneOffset;
        [Export("timeZoneOffset")]
        NSNumber TimeZoneOffset { get; }

        // @property (readonly, nonatomic) NSString * screenSize;
        [Export("screenSize")]
        string ScreenSize { get; }

        // @property (readonly, nonatomic) NSString * appVersion;
        [Export("appVersion")]
        string AppVersion { get; }

        // @property (readonly, nonatomic) NSString * carrierName;
        [Export("carrierName")]
        string CarrierName { get; }

        // @property (readonly, nonatomic) NSString * carrierCountry;
        [Export("carrierCountry")]
        string CarrierCountry { get; }

        // @property (readonly, nonatomic) NSString * appBuild;
        [Export("appBuild")]
        string AppBuild { get; }

        // @property (readonly, nonatomic) NSString * appNamespace;
        [Export("appNamespace")]
        string AppNamespace { get; }

        // -(BOOL)isEqual:(MSACDevice *)device;
        [Export("isEqual:")]
        bool IsEqual(MSACDevice device);
    }

    // @interface MSACAppCenter : NSObject
    [BaseType(typeof(NSObject), Name = "MSACAppCenter")]
    interface MSACAppCenter
    {
        // +(instancetype)sharedInstance;
        [Static]
        [Export("sharedInstance")]
        MSACAppCenter SharedInstance();

        // +(void)resetSharedInstance
        [Static]
        [Export("resetSharedInstance")]
        void ResetSharedInstance();

        // +(void)configureWithAppSecret:(NSString *)appSecret;
        [Static]
        [Export("configureWithAppSecret:")]
        void ConfigureWithAppSecret([NullAllowed] string appSecret);

        // +(void)start:(NSString *)appSecret withServices:(NSArray<Class> *)services;
        [Static]
        [Export("start:withServices:")]
        void Start([NullAllowed] string appSecret, Class[] services);

        // +(void)startService:(Class)service;
        [Static]
        [Export("startService:")]
        void StartService(Class service);

        // +(BOOL)isConfigured;
        [Static]
        [Export("isConfigured")]
        bool IsConfigured();

        // +(void)setLogUrl:(NSString *)setLogUrl;
        [Static]
        [Export("setLogUrl:")]
        void SetLogUrl([NullAllowed] string logUrl);

        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // +(MSACLogLevel)logLevel;
        // +(void)setLogLevel:(MSACLogLevel)logLevel;
        [Static]
        [Export("logLevel")]
        MSACLogLevel LogLevel();

        [Static]
        [Export("setLogLevel:")]
        void SetLogLevel(MSACLogLevel logLevel);

        // + (void)setCountryCode:(NSString *)countryCode
        [Static]
        [Export("setCountryCode:")]
        void SetCountryCode(string countryCode);

        //+ (void)setDataResidencyRegion:(NSString *)dataResidencyRegion
        [Static]
        [Export("setDataResidencyRegion:")]
        void SetDataResidencyRegion([NullAllowed] string dataResidencyRegion);

        // + (void)setNetworkRequestsAllowed:(BOOL)isAllowed;
        [Static]
        [Export("setNetworkRequestsAllowed:")]
        void SetNetworkRequestsAllowed(bool isAllowed);

        // + (BOOL)isNetworkRequestsAllowed;
        [Static]
        [Export("isNetworkRequestsAllowed")]
        bool IsNetworkRequestsAllowed();

        // + (void)setUserId:(NSString *)userId;
        [Static]
        [Export("setUserId:")]
        void SetUserId([NullAllowed] string userId);

        // +(void)setLogHandler:(MSACLogHandler)logHandler;
        [Static]
        [Export("setLogHandler:")]
        void SetLogHandler(MSACLogHandler logHandler);

        // +(void)setWrapperSdk:(MSACWrapperSdk *)wrapperSdk;
        [Static]
        [Export("setWrapperSdk:")]
        void SetWrapperSdk(MSACWrapperSdk wrapperSdk);

        // +(NSUUID *)installId;
        [Static]
        [Export("installId")]
        NSUuid InstallId();

        // +(BOOL)isDebuggerAttached;
        [Static]
        [Export("isDebuggerAttached")]
        bool IsDebuggerAttached();

        // +(void)setMaxStorageSize:(long)sizeInBytes completionHandler(void (^)(BOOL))completionHandler;
        [Static]
        [Export("setMaxStorageSize:completionHandler:")]
        void SetMaxStorageSize(long sizeInBytes, MSACSetLogLevelCompletionHandlerCallback callback);
    }

    // @protocol MSACService <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSACService
    {
    }

    // @interface MSACServiceAbstract : NSObject <MSACService>
    [BaseType(typeof(MSACService))]
    interface MSACServiceAbstract : MSACService
    {
    }

    // @interface MSACLogger : NSObject
    [BaseType(typeof(NSObject))]
    interface MSACLogger
    {
        // +(void)logMessage:(MSACLogMessageProvider)messageProvider level:(MSACLogLevel)loglevel tag:(NSString *)tag file:(const char *)file function:(const char *)function line:(uint)line;
        [Static]
        [Export("logMessage:level:tag:file:function:line:")]
        unsafe void LogMessage(MSACLogMessageProvider messageProvider, MSACLogLevel loglevel, string tag, IntPtr file, IntPtr function, uint line);
    }

    // @interface MSACWrapperLogger : NSObject
    [BaseType(typeof(NSObject))]
    interface MSACWrapperLogger
    {
        // +(void)MSACWrapperLog:(MSACLogMessageProvider)message tag:(NSString *)tag level:(MSACLogLevel)level;
        [Static]
        [Export("MSACWrapperLog:tag:level:")]
        void MSACWrapperLog(MSACLogMessageProvider message, string tag, MSACLogLevel level);
    }

    // typedef void (^MSACHttpRequestCompletionHandler)(NSData *_Nullable responseBody, NSHTTPURLResponse *_Nullable response, NSError* _Nullable error);
    delegate void MSACHttpRequestCompletionHandler([NullAllowed] NSData responseBody, [NullAllowed] NSHttpUrlResponse response, [NullAllowed] NSError error);

    // @protocol MSACHttpClientProtocol <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSACHttpClientProtocol
    {
        // @property(nonatomic, weak, nullable) id<MSACHttpClientDelegate> delegate;
        // Using a C# property, even when using the Wrap/Weak pattern, we get "setDelegate:]: unrecognized selector sent to instance", work around by using a setter method as we don't need the getter
        [Export("setDelegate:")]
        void SetDelegate([NullAllowed] MSACHttpClientDelegate httpClientDelegate);

        // @required - (void)sendAsync:(NSURL *)url
        //                      method:(NSString*) method
        //                     headers:(nullable NSDictionary<NSString*, NSString*> *)headers
        //                        data:(nullable NSData *)data
        //           completionHandler:(nullable MSACHttpRequestCompletionHandler) completionHandler;
        [Export("sendAsync:method:headers:data:completionHandler:")]
        void SendAsync(NSUrl url, NSString method, [NullAllowed] NSDictionary<NSString, NSString> headers, [NullAllowed] NSData data, [NullAllowed] MSACHttpRequestCompletionHandler completionHandler);

        // @required - (void) sendAsync:(NSURL*) url
        //                       method:(NSString*) method
        //                      headers:(nullable NSDictionary<NSString*, NSString*> *)headers
        //                         data:(nullable NSData *)data
        //               retryIntervals:(NSArray*) retryIntervals
        //           compressionEnabled:(BOOL) compressionEnabled
        //            completionHandler:(nullable MSACHttpRequestCompletionHandler) completionHandler;
        [Export("sendAsync:method:headers:data:retryIntervals:compressionEnabled:completionHandler:")]
        void SendAsync(NSUrl url, NSString method, [NullAllowed] NSDictionary<NSString, NSString> headers, [NullAllowed] NSData data, NSArray retryIntervals, bool compressionEnabled, [NullAllowed] MSACHttpRequestCompletionHandler completionHandler);

        // @required - (void)pause;
        [Export("pause")]
        void Pause();

        // @required - (void)resume;
        [Export("resume")]
        void Resume();

        // @required - (void)setEnabled:(BOOL)isEnabled;
        [Export("setEnabled:")]
        void SetEnabled(bool enabled);
    }

    // @protocol MSACHttpClientDelegate <NSObject>
    [Protocol]
    [BaseType(typeof(NSObject))]
    interface MSACHttpClientDelegate
    {
        // -(void)willSendHTTPRequestToURL:(NSURL *)url withHeaders:(nullable NSDictionary<NSString *, NSString *> *)headers;
        [Export("willSendHTTPRequestToURL:withHeaders:")]
        void WillSendHTTPRequestToURL(NSUrl url, [NullAllowed] NSDictionary<NSString, NSString> headers);
    }

    // @interface MSACDependencyConfiguration : NSObject
    [BaseType(typeof(NSObject))]
    interface MSACDependencyConfiguration
    {
        // @property(class, nonatomic) id<MSACHttpClientProtocol> httpClient;
        [Static]
        [Export("httpClient")]
        MSACHttpClientProtocol HttpClient { get; set; }
    }
}
