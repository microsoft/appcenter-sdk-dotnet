// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Foundation;

namespace Microsoft.AppCenter.Distribute.iOS.Bindings
{
    // @interface MSACDistribute : MSACService
    [BaseType(typeof(NSObject))]
    interface MSACDistribute
    {
        // +(void)setEnabled:(BOOL)isEnabled;
        [Static]
        [Export("setEnabled:")]
        void SetEnabled(bool isEnabled);

        // +(BOOL)isEnabled;
        [Static]
        [Export("isEnabled")]
        bool IsEnabled();

        // + (void)setApiUrl:(NSString *)apiUrl;
        [Static]
        [Export("setApiUrl:")]
        void SetApiUrl(string apiUrl);

        // + (void)setInstallUrl:(NSString *)installUrl;
        [Static]
        [Export("setInstallUrl:")]
        void SetInstallUrl(string installUrl);

        // + (void)openURL:(NSURL *)url;
        [Static]
        [Export("openURL:")]
        void OpenUrl(NSUrl url);

        // + (void)setDelegate:(id<MSACDistributeDelegate>)delegate;
        [Static]
        [Export("setDelegate:")]
        void SetDelegate(MSACDistributeDelegate distributeDelegate);

        // + (void)notifyUpdateAction:(MSACUpdateAction)action;
        [Static]
        [Export("notifyUpdateAction:")]
        void NotifyUpdateAction(MSACUpdateAction action);

        // + (void)setUpdateTrack:(MSACUpdateTrack)updateTrack;
        [Static]
        [Export("setUpdateTrack:")]
        void SetUpdateTrack(MSACUpdateTrack updateTrack);

        // + (MSACUpdateTrack)updateTrack;
        [Static]
        [Export("updateTrack")]
        MSACUpdateTrack GetUpdateTrack();

        // +(void)resetSharedInstance;
        [Static]
        [Export("resetSharedInstance")]
        void ResetSharedInstance();

        // + (void)checkForUpdate;
        [Static]
        [Export("checkForUpdate")]
        void CheckForUpdate();

        // + (void)disableAutomaticCheckForUpdate;
        [Static]
        [Export("disableAutomaticCheckForUpdate")]
        void DisableAutomaticCheckForUpdate();
    }

    // @protocol MSACDistributeDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface MSACDistributeDelegate
    {
        // @optional - (BOOL)distribute:(MSACDistribute *)distribute releaseAvailableWithDetails:(MSACReleaseDetails *)details;
        [Export("distribute:releaseAvailableWithDetails:")]
        bool OnReleaseAvailable(MSACDistribute distribute, MSACReleaseDetails details);

        // - (void)distributeWillExitApp:(MSACDistribute *)distribute;
        [Export("distributeWillExitApp:")]
        void WillExitApp(MSACDistribute distribute);
    }

    // @interface MSACReleaseDetails : NSObject
    [BaseType(typeof(NSObject))]
    interface MSACReleaseDetails
    {
        // @property(nonatomic, copy) NSNumber *id;
        [Export("id")]
        int Id { get; }

        // @property(nonatomic, copy) NSString *version;
        [Export("version")]
        string Version { get; }

        // @property(nonatomic, copy) NSString *shortVersion;
        [Export("shortVersion")]
        string ShortVersion { get; }

        // @property(nonatomic, copy) NSString *releaseNotes;
        [Export("releaseNotes")]
        string ReleaseNotes { get; }

        // @property(nonatomic) NSURL* releaseNotesUrl;
        [Export("releaseNotesUrl")]
        NSUrl ReleaseNotesUrl { get; }

        // @property(nonatomic, getter=isMandatoryUpdate) BOOL mandatoryUpdate;
        [Export("isMandatoryUpdate")]
        bool MandatoryUpdate { get; }
    }
}
