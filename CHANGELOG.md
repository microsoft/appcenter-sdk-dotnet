# App Center SDK for .NET Change Log

## Version 5.0.7 (Under development)

___
## Version 5.0.6

### App Center

* **[Fix]** Fix strong name signing.

#### Windows

* **[Fix]** Remove commit hash from `sdkVersion` log field for UWP and Windows desktop SDK.

#### Android

* **[Improvement]** Update native SDK to version [5.0.5](https://github.com/microsoft/appcenter-sdk-android/releases/tag/5.0.5)
___

## Version 5.0.5

### App Center

* **[Fix]** Fix Mac Catalyst by adding it to the main nuspec package

___

## Version 5.0.4

### App Center

* **[Improvement]** Update DotNet target version to net8

#### Windows

* **[Improvement]** Improve UWP and WinUI app type checking.
* **[Improvement]** Update SQLitePCLRaw.bundle_green to version 2.1.7

#### Apple

* **[Improvement]** Support Mac Catalyst
* **[Improvement]** Update PrivacyInfo files to include all relevant information.
* **[Improvement]** Update PLCrashReporter to version 1.11.2.

#### Android

* **[Fix]** Add RECEIVER_EXPORTED flag for install receiver.
* **[Fix]** Add FLAG_ALLOW_UNSAFE_IMPLICIT_INTENT flag for broadcast pending intent.

___

## Version 5.0.3

### App Center

* **[Internal]** Add `dataResidencyRegion` option.

#### Windows

* **[Improvement]** Update SQLitePCLRaw.bundle_green to version 2.1.5
* **[Improvement]** Improved desktop app type checking to work with app life cycle.
___

## Version 5.0.2

### App Center

* **[Fix]** Update `Newtonsoft.Json` dependency to version `13.0.2`. The update fixes [GHSA-5crp-9r3c-p9vr](https://github.com/advisories/GHSA-5crp-9r3c-p9vr).
* **[Fix]** Use `DispatcherUnhandledException` instead of `AppDomain.CurrentDomain.UnhandledException` for WPF applications for postcrash processing. It fixes #1727.
* **[Improvement]** Remove SmartLink=false, as it may break the SDK integration in some cases.
* **[Improvement]** Remove flag RegexOptions.Compiled, as it is proved to be slow on net6 and above.

#### Windows

* **[Fix]** Fix WindowsHelper's OSVersion check for Windows 7 sp1

___

## Version 5.0.1

### App Center

* **[Fix]** Fix strong name validation.

___

## Version 5.0.0

### App Center

#### Apple

* **[Fix]** Fix crash on net6.0-macos application because of an incorrect native binary linking.
* **[Feature]** Add Xcode 14 support. Xcode 11 and Xcode 12 are out of support now. Bump minumum supported iOS version to iOS 11.

#### Android

* **[Fix]** Fix ignoring maximum storage size limit in case logs contain large payloads.

#### Windows

* **[Fix]** Fix crash on MAUI windows unpackaged application due to picking a wrong lifecycle helper.

### App Center Crashes

#### Apple

* **[Improvement]** Update PLCrashReporter to 1.11.0.

### App Center Distribute

#### Android

* **[Feature]** Add requesting notifications permission for Android 13 (notifications are used to inform about downloading/installing status if an application is in background)

___

## Version 5.0.0-preview.1

### App Center

* **[Feature]** Add .NET 6 support.
* **[Breaking change]** .NET Core `3.0` target for WPF and WinForms applications is [out of support](https://devblogs.microsoft.com/dotnet/net-core-3-0-end-of-life/), it's replaced with `3.1` (LTS).
* **[Breaking change]** .NET Framework `4.6.1` target is [out of support](https://devblogs.microsoft.com/dotnet/net-framework-4-5-2-4-6-4-6-1-will-reach-end-of-support-on-april-26-2022/), it's replaced with `4.6.2`
* **[Breaking change]** .NET `5.0` for windows is [out of support](https://devblogs.microsoft.com/dotnet/dotnet-5-end-of-support-update/), it's replaced with .NET `6.0`.
* **[Breaking change]** Updated .NET Standard "fallback" target from `1.0` to `2.0`.
* **[Breaking change]** Updated required version of `Xamarin.Android` target framework to `10.0`. The version is specified in the `<TargetFrameworkVersion>` element in the project file.

___

## Version 4.5.3

### App Center

#### Android

* **[Fix]** Fix crash when storage is encrypted during direct boot. Please note that settings and pending logs database are not shared between regular and device-protected storage.

#### Windows

* **[Fix]** Fix crash when retrieving device info on early Windows versions.
* **[Fix]** Update vulnerable version of `SQLitePCLRaw.bundle_green` dependency.
* **[Fix]** Fix version reporting for non-WinUI applications when target framework is `net5.0-windows10.0.17763.0` or higher.
* **[Fix]** Fix `4.5.1` regression of auto session tracking.
* **[Fix]** Fix duplicate inner exception reporting in `AggregateException`.

### App Center Crashes

#### iOS/macOS

* **[Improvement]** Update PLCrashReporter to 1.10.2.
* **[Fix]** Fix `4.3.0` regression where native exceptions crashes an application even if wrapped into `try`/`catch`.

### App Center Distribute

#### Android

* **[Improvement]** Remove optional `SYSTEM_ALERT_WINDOW` permission that was required to automatically restart the app after installing the update.
* **[Improvement]** Add fallback to the old [ACTION_INSTALL_PACKAGE](https://developer.android.com/reference/android/content/Intent#ACTION_INSTALL_PACKAGE) installation method if the update cannot be installed by `PackageInstaller` API (e.g. when MIUI optimizations block installation).
* **[Fix]** Fix possible crash on resume event before initialization.
* **[Fix]** Fix clicks on the download completion notification.
* **[Fix]** Fix ANR on installing large packages.
* **[Fix]** Fix cancellation handling of confirmation dialog for mandatory updates.
* **[Fix]** Fix strict mode issues.

___

## Version 4.5.1

### App Center

#### Windows

* **[Feature]** Add support target framework `net5.0-windows10.0.17763.0` or higher for non-WinUI applications.
* **[Fix]** Fix a deadlock that occurs during the services initialization on Windows.

#### macOS 

* **[Fix]** Fix "Malformed Framework" error when submitting to App Store.

### App Center Crashes

#### UWP

* **[Fix]** Fix crash when use `Crashes.TrackError` API where exception has frames with empty native images.

___

## Version 4.5.0

### App Center

* **[Breaking change]** Remove `AppCenter.SetCustomProperties` API.
* **[Fix]** Fix check on using `HttpClient` API instead of `ServicePointManager` in applications with target framework version `4.7.2` and higher.

#### Windows

* **[Fix]** Fix App Center SDK compatibility with project targets .NET Framework and .NET Core.
* **[Fix]** Fix crash during getting device information when WMI service is disabled.
* **[Fix]** Fix a deadlock after the application start caused by improper `StatefulMutex` usage on the Windows Server OS.

#### Android/iOS

* **[Feature]** Add `AppCenter.setCountryCode(string)` API to set the country code manually.

#### iOS

* **[Fix]** Fix throw an exception when checking to authenticate MAC value during decryption.

### App Center Analytics

* **[Feature]** Increase the interval between sending logs from 3 to 6 seconds for the backend load optimization.
* **[Feature]** Add `Analytics.EnableManualSessionTracker` and `Analytics.StartSession` APIs for tracking session manually.

#### macOS

* **[Feature]** Add support for Xamarin.Mac.

### App Center Crashes

#### iOS

* **[Fix]** Fix sending `Crashes.trackError` logs after allowing network requests after the launch app.

#### macOS

* **[Feature]** Add support for Xamarin.Mac.

### App Center Distribute

#### iOS

* **[Fix]** Cancel authorization process if application is not active, otherwise `ASWebAuthenticationSession` will fail opening browswer and update flow will end up being in a broken state. This only affects updating from a private distribution group.

#### Android

* **[Feature]** Remove the download manager task if the download doesn't start within 10 seconds.
* **[Feature]** Replace installing a new release using the deprecated intent action [ACTION_INSTALL_PACKAGE](https://developer.android.com/reference/android/content/Intent#ACTION_INSTALL_PACKAGE) with the `PackageInstaller` API.
* **[Feature]** Add sumcheck on the downloaded file before starting the install process.
* **[Fix]** Fix a crash after discarding the installation if the download of a new release was interrupted in the previous application start and resumed in the current one.

___

## Version 4.4.0

### App Center

#### Windows

* **[Feature]** Add support for platform `WinUI in Desktop`. The target version of `WinUI` apps should be `net5.0-windows10.0.17763.0` or higher. **Known issue**: on WinUI apps, the amount of sessions may be lower than on UWP apps due to specifics of its lifecycle.
* **[Fix]** Update `Newtonsoft.Json` dependency to version `13.0.1`.
* **[Fix]** Fix sending pending logs after the first application start.
* **[Fix]** Using `ServicePointManager` for setup TLS configuration breaks the ability to use any other TLS protocols except TLS1.2 in the clients' applications. We replaced using `ServicePointManager` to `HttpClient` API for applications with target framework version `4.7.1` or higher. For applications with the target framework version lower than `4.7.1` TLS connection is still configured via `ServicePointManager`.

#### iOS

* **[Feature]** Improved `AES` token encryption algorithm using `Encrypt-then-MAC` data authentication approach.

#### Android

* **[Feature]** Improved `AES` token encryption algorithm using `Encrypt-then-MAC` data authentication approach.

### App Center Crashes

#### iOS

* **[Improvement]** Update PLCrashReporter to 1.10.

### App Center Distribute

#### Android

* **[Fix]** Fix a rare deadlock case when a new version starts downloading and at the same moment the download status is checked.
* **[Fix]** Fix passing pending intent flag for a completed download notification on Android lower then 23 API.

___

## Version 4.3.0

### AppCenter

* **[Feature]** Add a `AppCenter.IsNetworkRequestsAllowed` API to block any network requests without disabling the SDK.

#### Windows

* **[Fix]** Fix infinite loop when old logs cannot be purged by a new one with a different channel name in a case when the storage is full.

### App Center Crashes

#### WPF/WinForms

* **[Fix]** Add obfuscation for username value in the stack trace.

#### iOS

* **[Fix]** Merge the device information from the crash report with the SDK's device information in order to fix some time sensitive cases where the reported application information was incorrect.
* **[Improvement]** Update PLCrashReporter to 1.9.0.

### App Center Distribute

#### Android

* **[Fix]** Fix crash during downloading a new release when minifyEnabled settings is true.
* **[Fix]** Add a missing tag android:exported to the manifest required for Android 12.

### App Center Distribute Play

#### Android

* **[Fix]** Fix replacement of Distribute dependencies for publishing in Google Play.

___

## Version 4.2.0

### AppCenter

* **[Feature]** In the App Center SDK was added support of strong-named signing assemblies for compatibility with applications that use strong-named signing. Read more about [strong-named assemblies](https://docs.microsoft.com/en-us/dotnet/standard/assembly/strong-named).
* **[Fix]** Fix SDK not sending events when application is packed to self-contained single-file executable.

#### iOS

* **[Improvement]** Use ASWebAuthenticationSession for authentication on iOS 12 or later.

### App Center Crashes

#### Android

* **[Fix]** Fix formatting of stack trace in the `ErrorReport`.

### App Center Distribute

#### Android

* **[Fix]** Fix `NullPointerException` occurring when settings dialog was intended to be shown, but there is no foreground activity at that moment.
* **[Fix]** Fix a crash when download manager application was disabled.
* **[Fix]** Fix showing the title in the push notification while downloading a new release.

#### iOS

* **[Fix]** Fix `kMSACUpdateTokenRequestIdKey` never gets removed.

___

## Version 4.1.0

### AppCenter

* **[Feature]** Add a `SetMaxStorageSizeAsync` API which allows setting a maximum size limit on the local SQLite storage. The default value is 10MiB.

#### iOS

* **[Fix]** Fix a crash when SQLite returns zero for `page_size`.

### App Center Distribute

#### iOS

* **[Feature]** Add `WillExitApp` callback to distribute listener.

#### Android

* **[Fix]** Fix a crash when the app is trying to open the system settings screen from the background.
* **[Fix]** Fix browser opening when using a private distribution group on Android 11.

#### iOS/Android

 * **[Feature]** Add `NoReleaseAvailable` callback to distribute listener.

### App Center Crashes

#### Android

* **[Fix]** Fix removing throwable files after rewriting error logs due to small database size.

___

## Version 4.0.0

### App Center

#### Android

* **[Breaking change]** Bumping the minimum Android SDK version to 21 API level (Android 5.0), because old Android versions do not support root certificate authority used by App Center and would not get CA certificates updates anymore.

#### iOS

* **[Fix]** Fix `NSInvalidArgumentException` when using non-string object as a key in `NSUserDefaults`.
* **[Fix]** Fix `NSDateFormatter` initialization in a concurrent environment.

### App Center Crashes

#### iOS

* **[Improvement]** Update PLCrashReporter to 1.8.0.

### App Center Push

App Center Push has been removed from the SDK and will be [retired on December 31st, 2020](https://devblogs.microsoft.com/appcenter/migrating-off-app-center-push/). 
As an alternative to App Center Push, we recommend you migrate to [Azure Notification Hubs](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-push-notification-overview) by following the [Push Migration Guide](https://docs.microsoft.com/en-us/appcenter/migration/push/).

___

## Version 3.4.3

### App Center Analytics

#### iOS

* **[Fix]** Fix naming conflict with iOS 14 private Apple framework.

___

## Version 3.4.2

### App Center Crashes

#### Xamarin

* **[Fix]** Fix sending attachments with a `null` text value.

### App Center Distribute

#### iOS

* **[Fix]** Fix manually checking for updates before `applicationDidBecomeActive` event.

___

## Version 3.4.1

### App Center

#### WPF/WinForms

* **[Fix]** Fix exception handling when the app tries to access a locked database.
* **[Fix]** Fix getting device information when the application runs in partial trust mode.

### App Center Analytics

#### iOS

* **[Fix]** Fix processing logs (e.g., events) emitted from the `applicationWillTerminate` application delegate method.

### App Center Crashes

#### iOS

* **[Improvement]** Update PLCrashReporter to 1.7.2.

___

## Version 3.4.0

### App Center

#### Android

* **[Fix]** Fix an `IncorrectContextUseViolation` warning when calculating screen size on Android 11.
* **[Fix]** All SQL commands used in SDK are presented as raw strings to avoid any possible static analyzer's SQL injection false alarms.

#### WPF/WinForms

* **[Fix]** Fix exception handling if App Center is disabled during sending logs.

### App Center Distribute

#### Android

* **[Fix]** Fix Distribute can't get updates for Realme devices which use Realme UI.

#### iOS

* **[Fix]** Obfuscate app secret value that appears as URI part in verbose logs when getting release updates info.

### App Center Distribute Play

App Center Distribute Play is a package with stubbed APIs for Distribute module to avoid Google Play flagging the application for malicious behavior. It must be used only for build variants which are going to be published on Google Play. See the [public documentation](https://docs.microsoft.com/en-us/appcenter/sdk/distribute/xamarin#remove-in-app-updates-for-google-play-builds) for more details about this change.

___

## Version 3.3.0

This version has a breaking change on iOS - it drops Xcode 10 support, Xcode 11 is a minimal supported version now.

### App Center

#### Android

* **[Fix]** Fix possible delays in UI thread when queueing a large number of events.

#### iOS

* **[Fix]** Fix crash when local binary data (where unsent logs or unprocessed crashed are stored) is corrupted.
* **[Fix]** When carrier name is retrieved incorrectly by iOS, show `nil` as expected instead of "carrier" string.

### App Center Crashes

#### iOS

* **[Improvement]** Update PLCrashReporter to 1.7.1.
* **[Fix]** Fix reporting stacktraces on iOS simulator.
* **[Fix]** Add attachments verification in `TrackError` function to handle explicitly passed `null` array to variable arguments parameter.

___

## Version 3.2.2

### App Center

#### Android

* **[Fix]** Fix reporting Xamarin.Android version.

#### iOS

* **[Improvement]** Use namespaced `NSUserDefaults` keys with the **MSAppCenter** prefix for all the keys set by the SDK. Fixed a few keys missing namespace.

### App Center Crashes

#### iOS

* **[Improvement]** Update PLCrashReporter to 1.6.0.

___

## Version 3.2.1

### App Center

#### WPF/WinForms

* **[Fix]** Fix possible deadlock on start if the configuration file was corrupted.
* **[Fix]** Fix app build version reporting, the assembly file version is used for this field now.

#### UWP/WPF/WinForms

* **[Fix]** Fix retry sending logs after timeout exception.

### App Center Crashes

* **[Fix]** Remove the multiple attachments warning as that is now supported by the portal.

#### Android

* **[Fix]** Change minidump filter to use file extension instead of name.
* **[Fix]** Fix removing minidump files when the sending crash report was discarded.

#### iOS

* **[Improvement]** Update PLCrashReporter to 1.5.1.

### App Center Distribute

#### Android

* **[Feature]** Automatically check for update when application switches from background to foreground (unless automatic checks are disabled).
* **[Fix]** Fix checking for updates after disabling the Distribute module while downloading the release.

___

## Version 3.1.0

### App Center

#### iOS

* **[Fix]** Fix reference to `Security` framework which can be missing in release builds with Xamarin.iOS 13.10 or newer.

### App Center Distribute

* **[Feature]** Add a `DisableAutomaticCheckForUpdate` API that needs to be called before SDK start in order to turn off automatic check for update. 
* **[Feature]** Add a `CheckForUpdate` API to manually check for update.

#### Android

* **[Fix]** Fix not checking updates on public track if it was ignored on error while initializing in-app update for private track.

___

## Version 3.0.0

### App Center Auth

App Center Auth is [retired](https://aka.ms/MBaaS-retirement-blog-post) and has been removed from the SDK.

### App Center Data

App Center Data is [retired](https://aka.ms/MBaaS-retirement-blog-post) and has been removed from the SDK.

### App Center

* **[Fix]** Allow whitespace characters between secrets like in versions before `2.6.0`.

#### iOS

* **[Fix]** Optimization of release objects from memory during the execution of a large number of operations.
* **[Fix]** Disable module debugging for release mode in the SDK to fix dSYM warnings.
* **[Fix]** Fix SDK crash at application launch on iOS 12.0 (`CTTelephonyNetworkInfo.serviceSubscriberCellularProviders` issue).
* **[Fix]** The SDK was considering 201-299 status code as HTTP errors and is now fixed to accept all 2XX codes as successful.
* **[Improvement]** Replaced sqlite query concatenation with more secure bindings.

#### Android

* **[Fix]** Fix infinite recursion when handling encryption errors.

#### UWP

* **[Feature]** Support ARM64 architecture.
* **[Breaking change]** The minimum supported Window 10 version is now `10.0.16299.0`.

#### WPF

* **[Fix]** Fix a deadlock when starting AppCenter that was preventing touch events from being fired.

#### WPF/WinForms

* **[Breaking change]** The minimum supported version of .NET Framework is now `4.6.1`.
* **[Fix]** Fix application crash when the `AppCenter` configuration file is corrupted.

#### UWP/WPF/WinForms

* **[Dependency changes]** Update `SQLitePCLRaw.bundle_green` dependency to version `2.0.2` and remove dependency from `sqlite-net-pcl`.

### App Center Distribute

* **[Feature]** Add `UpdateTrack` property to be able to explicitly set either `Private` or `Public` update track. By default, a public distribution group is used. **Breaking change**: To allow users to access releases of private groups you now need to migrate your application to call `Distribute.UpdateTrack = UpdateTrack.Private` before the SDK start. Please read the documentation for more details.
* **[Behavior change]** The public distribution is simplified to provide only one public group. If you have existing public groups defined for your application your users will receive the latest version of all public groups.

#### iOS

* **[Fix]** Fix a crash when `SFAuthenticationSession` accesses the controller which is in the process of being released.
* **[Fix]** Fix sign-in when switching to third-party apps while activating updates.

#### Android

* **[Fix]** Avoid opening browser to check for sign-in information after receiving an SSL error while checking for app updates (which often happens when using a public WIFI).
* **[Fix]** When in-app update permissions become invalid and need to open browser again, updates are no longer postponed after sign-in (if user previously selected the action to postpone for a day).
* **[Fix]** Fix a possible deadlock when activity resumes during background operation for some `Distribute` public APIs like `Distribute.isEnabled()`.

### App Center Crashes

#### iOS

* **[Improvement]** Add information logs when signals are removed/restored during initialization.
* **[Improvement]** Update PLCrashReporter to 1.4.0.
* **[Feature]** Add `Crashes.UseMonoRuntimeSignalMethods` boolean property to call before `AppCenter.Start`, if set to `false` it forces the [old signal behavior](https://www.mono-project.com/docs/advanced/signals/#incomplete-solution) even if the runtime Mono version is 4.8 or higher. The default value is `true`, which means using the [current signal behavior](https://www.mono-project.com/docs/advanced/signals/#complete-solution) if the Mono version is 4.8 or higher (and falling back to the old one on older Mono versions). This is introduced in case the current signal behavior causes unexpected segmentation fault crashes.

#### Android

* **[Fix]** Fix incorrect app version when an NDK crash is sent after updating the app.
* **[Behavior change]** Change the path to the minidump directory to use a subfolder in which the current contextual data (device information, etc.) is saved along with the .dmp file.
___

## Version 2.6.4

### App Center

#### WPF/WinForms

* **[Fix]** Use explicit dependencies for .NET Framework assemblies to avoid runtime errors in some applications.

#### UWP/WPF/WinForms

* **[Fix]** Fix sending remaining pending logs after sending 3 concurrent HTTP requests.
* **[Fix]** The SDK was considering 201-299 status code as HTTP errors and is now fixed to accept all 2XX codes as successful.

#### Android

* **[Fix]** Fix `MissingMethodException` of `String.Split()` with some build configurations (a bug introduced in version `2.5.0`).

#### iOS

* **[Fix]** Improve log messages for errors when it failed to read/write auth token history.

### App Center Crashes

#### Android

* **[Fix]** Validate error attachment size to avoid server error or out of memory issues (using the documented limit which is 7MB).

#### iOS

* **[Fix]** Fix sending crashes if an application is launched in background.
* **[Fix]** Validate error attachment size to avoid server error or out of memory issues (using the documented limit which is 7MB).
* **[Fix]** Fix an issue where crash might contain incorrect data if two consecutive crashes occurred in a previous version of the application.

### App Center Auth

#### iOS

* **[Fix]** Fix build warnings when adding App Center Auth framework in project.

### App Center Distribute

#### iOS

* **[Fix]** Fix missing alert dialogs in apps that use iOS 13's new UIScene API (multiple scenes are not yet supported).
* **[Fix]** Fix an issue where users would sometimes be prompted multiple times to sign in with App Center.

___

## Version 2.6.2

### App Center

#### WPF/WinForms

* **[Fix]** Fix an error on running same application twice and enabling App Center.

#### UWP/WPF/WinForms

* **[Fix]** Fix an issue where internal SDK exceptions would sometimes be "unobserved" and would cause undefined behavior.
* **[Fix]** The SDK now detects the SQLite database is corrupted and deletes it and creates a new one to start over.

### App Center Crashes

#### UWP/WPF/WinForms

* **[Fix]** Validate error attachment size to avoid server error or out of memory issues (using the documented limit which is 7MB).

#### UWP

* **[Fix]** Fix NuGet package dependency.

### AppCenterDistribute

#### iOS

* **[Fix]** Fix native link error when using Distribute module without Auth and building for real devices.

___

## Version 2.6.1

### App Center

#### iOS

* **[Fix]** Fix warnings in Xcode 11 when SDK is installed via CocoaPods.

#### Xamarin

* **[Fix]** Updated Android support packages versions to `28.0.0.3` for Auth and Push modules.

### App Center Crashes

* **[Feature]** Support sending attachments in handled errors.

### App Center Distribute

#### Android

* **[Fix]** Fix an in-app update caching issue, where the same version was installed constantly after the 1st successful update (or also if the download was canceled).

___

## Version 2.5.0

### App Center

#### Windows

* **[Fix]** Update `Newtonsoft.Json` dependency to version `12.0.2`.

#### .NET Core 3

* **[Fix]** Fix using stable versions of NuGet dependencies for .NET Core 3.0.

### App Center Crashes

#### UWP

* **[Feature]** App Center now supports crashes for sideloaded UWP applications.
* **[Feature]** APIs in the Crashes module are now implemented for UWP: handled errors, crash attachments, crash callbacks, getting crash information about last session, and enabling/disabling the module. Detecting low memory warning is not supported.

#### UWP/WPF/WinForms

* **[Feature]** Allow users to set userId that applies to crashes and errors.

#### Android

* **[Breaking change]** Remove insecure implementation of the raw `AndroidErrorDetails.Throwable` property (now always returns `null` and marked as obsolete), and provide `string StackTrace` property as an alternative on Xamarin.Android.

### App Center Push

#### UWP

* **[Feature]** Allow developers to push notifications to a specific userId.

### App Center Distribute

#### Android

* **[Fix]** Downloading in-app update APK file has been failing on Android 4.x since TLS 1.2 has been enforced early September. The file is now downloaded using HTTPS direct connection when running on Android 4 instead of relying on system's download manager.
* **[Fix]** Fix a crash and improve logging when downloading an update fails on Android 5+.
* **[Breaking change]** If your minSdkVersion is lower than 19, Android requires the WRITE_EXTERNAL_STORAGE permission to store new downloaded updates. Please refer to the updated documentation site for detailed instructions. This is related to the download fix.

### App Center Data

* **[Fix]** Reduced retries on Data-related operations to fail fast and avoid the perception of calls stopped responding.

___

## Version 2.4.0-preview

### App Center

#### WPF/WinForms

* **[Fix]** Fix application version being reported for ClickOnce deployments.

### App Center Auth

#### iOS

* **[Fix]** Redirect URIs are now hidden in logs.
* **[Fix]** Fix interactive sign in on iOS 13. Temporary fix, will be revisited in the future.
* **[Feature]** Updated the Microsoft Authentication Library dependency to v0.7.0.

### App Center Analytics

#### iOS

* **[Fix]** Fix crash involving SDK's `ms_viewWillAppear` method.

### App Center Push

#### Android

* **[Breaking change]** Update Google Play Services' and Firebase's versions to v71 in Push module which forces applications to use Android 9.0 Mono frameworks in build options. This has no impact on minimum Android SDK version which remains API level 16.

### App Center Crashes

* **[Breaking change]** Remove insecure implementation of the raw `ErrorReport.Exception` property (now always returns `null` and marked as obsolete), and provide `string StackTrace` property as an alternative on Xamarin, UWP, WPF and WinForms.

#### WinForms

* **[Fix]** Don't prevent WinForms applications from crashing. If unhandled exceptions are handled by the application, they must now be reported using `Crashes.TrackError` to be displayed on AppCenter, see the [public documentation](https://docs.microsoft.com/en-us/appcenter/sdk/crashes/wpf-winforms) for more details about this change.

## Version 2.3.0-preview

This preview version adds support for .NET Core 3.0 WPF and WinForms applications for the Analytics and Crashes modules.

This preview version removes Crashes support for UWP. The App Center backend is not yet ready to process UWP crashes using the system introduced in this version. If your app relies on UWP crashes, do not update to this preview version.

### App Center

#### WPF/WinForms

* **[Fix]** Fix configuration file location to be in a user-specific directory.

### App Center Analytics

#### Windows

* **[Fix]** Fix reporting the incorrect device models when a placeholder was used in some cases.

#### WPF/WinForms

* **[Feature]** Add support for .NET Core 3.0 applications.
* **[Fix]** Optimize minimized window detection.

### App Center Crashes

#### Android and iOS

* **[Feature]** Catch "low memory warning" and provide the API to check if it has happened in last session: `Crashes.HasReceivedMemoryWarningInLastSession()`.

#### WPF/WinForms

* **[Feature]** Add support for .NET Core 3.0 applications.
* **[Fix]** Wait to finish processing files before deleting files when calling `SetEnabledAsync(false)` during processing.

### App Center Distribute

#### iOS

* **[Fix]** Obfuscate app secret value that appears as URI part in verbose logs for in-app updates.

### App Center Push

#### Android

* **[Fix]** Fix confusing information log about the availability of the Firebase SDK.
* **[Fix]** Fix sending the push installation log after delayed start.

### App Center Auth

#### iOS

* **[Feature]** App Center Auth logging now includes MSAL logs.

#### Android

* **[Feature]** App Center Auth logging now includes MSAL logs.
* **[Fix]** Redirect URIs are now hidden in logs.

___

## Version 2.2.1-preview

This preview version adds support for WPF and WinForms applications for the Analytics and Crashes modules.

This preview version removes Crashes support for UWP. The App Center backend is not yet ready to process UWP crashes using the system introduced in this version. If your app relies on UWP crashes, do not update to this preview version.

### App Center

* **[Breaking Change]** This version is no longer compatible with PCL. Migrating to .NET standard is now required for portable libraries using the AppCenter SDK.

#### Android

* **[Fix]** Remove unsecure UUID fallback for UUID generation failures that cannot occur in reality.

#### WPF/WinForms

* **[Feature]** APIs from the `AppCenter` class can now be consumed in WPF and WinForms applications (except `AppCenter.SetUserId`).

### App Center Analytics

#### UWP

* **[Fix]** Fix validation of `TrackEvent` that was sending the event without properties instead of removing the invalid ones.

#### WPF/WinForms

* **[Feature]** Add support for WPF and WinForms.

### App Center Crashes

#### WPF/WinForms

* **[Feature]** Add support for WPF and WinForms. User identification is not currently supported.

#### UWP

* **[Breaking Change]** This version introduces a breaking change where the SDK no longers register with Windows error reporting. UWP developers using the Crashes module must keep using the latest stable release (2.1.1).

#### iOS

* **[Fix]** Fix possible deadlock in `Crashes.TrackError`.

#### Android

* **[Fix]** The in-memory cache of error reports is now cleared when disabling Crashes.

### App Center Push

#### Xamarin

* **[Fix]** Updated Android support packages to 28.0.0.1 version in Push module.

### App Center Data

#### Xamarin

* **[Feature]** Add support for offline list of documents.
* **[Feature]** Change the default time-to-live (TTL) from 1 day to infinite (never expire).
* **[Feature]** Add `ReadOptions` parameter to the `list` API.
* **[Feature]** Deserialization errors are now exposed through the document `Error` property (and leaving `DeserializedValue` null) instead of throwing an exception.
* **[Feature]** Serialize `null` document values.

#### Android

* **[Fix]** Allow null for `ReadOptions` and `WriteOptions` parameters.

### App Center Distribute

#### iOS

* **[Fix]** Fix crash when an application was minimized while trying to reinstall after setup failure.

___

## Version 2.1.1

### App Center Distribute

#### iOS

* **[Fix]** Fix a crash (regression from version 2.1.0) when checking for in-app updates.

___

## Version 2.1.0

### App Center

#### iOS

* **[Fix]** Improve encryption security.

### App Center Crashes

#### iOS

* **[Fix]** Fix a crash when tracking an exception without a message.

### App Center Distribute

#### Android

* **[Feature]** Add `Distribute.SetEnabledForDebuggableBuild(bool)` method to allow in-app updates in debuggable builds.
* **[Fix]** Fix duplicate in-app update dialog when restarting (or switching) activity quickly after clicking download. Also fixes a crash when choosing "Ask me in a day" in the duplicate dialog.
* **[Fix]** Fix a crash that could occur when downloading the update with a customized dialog and then calling `Distribute.NotifyUserConfirmation(UpdateAction.Postpone)` right after calling `Distribute.NotifyUserConfirmation(UpdateAction.Update)`.

### App Center Auth

* **[Feature]** Expose the ID Token and Access Token (as raw JWT format) in the `UserInformation` object returned from the sign-in method.
* **[Breaking change]** The `UserInformation` class has been moved from the `Microsoft.AppCenter` namespace to `Microsoft.AppCenter.Auth` namespace.

#### Android

* **[Fix]** Fix missing proguard rules so that the app does not have to specify them.
* **[Fix]** Fix crash on silently refreshing token if initialization of MSAL fails.

#### iOS

* **[Fix]** Fix changing signing status may cause logs (e.g., events) to be delayed.
* **[Fix]** Validate custom URL scheme before starting Auth and log an error message when it is invalid.

### App Center Data

* **[Fix]** Fix an issue where invalid characters in the document ID are accepted at creation time but causing errors while trying to read or delete the document. The characters are `#`, `\`, `/`, `?`, and all whitespaces.

#### iOS

* **[Fix]** Fix document serialization/deserialization to handle non string types inside an object.

___

## Version 2.0.0

Version 2.0.0 of the App Center SDK includes two new modules: Auth and Data.

These new modules are only supported in Xamarin (Android and iOS).

This version has a **breaking change** for Xamarin.iOS, it only supports Xcode 10.0.0+ and Mono 5.12+.

### App Center Auth

App Center Auth is a cloud-based identity management service that enables you to authenticate users and manage their identities. You can also leverage user identities in other App Center services.

### App Center Data

The App Center Data service provides functionality enabling developers to persist app data in the cloud in both online and offline scenarios. This enables you to store and manage both user-specific data as well as data shared between users and across platforms.

### App Center Crashes

* **[Fix]** Fix intellisense for APIs.

#### Xamarin

* **[Feature]** After calling `Auth.SignInAsync()`, the next crashes are associated with an `accountId` corresponding to the signed in user. This is a different field than the `userId` set by `AppCenter.SetUserId(string)`. Calling `Auth.SignOut()` stops the `accountId` association for the next crashes.

##### iOS

* **[Fix]** Print an error and return immediately when calling `Crashes.NotifyUserConfirmation(UserConfirmation)` with confirmation handlers not implemented.

### App Center Distribute

* **[Fix]** Fix intellisense for APIs.

#### iOS

* **[Fix]** Starting the application with "Guided Access" enabled blocks the update flow since in-app update is not possible in this mode.

#### Android

* **[Fix]** Fix in-app updates not working on new Samsung devices.

### App Center Push

* **[Fix]** Fix intellisense for APIs.

#### Xamarin

* **[Feature]** After calling `Auth.SignInAsync()`, the push installation is associated to the signed in user with an `accountId` and can be pushed by using the `accountId` audience. This is a different field than the `userId` set by `AppCenter.SetUserId(string)`. The push installation is also updated on calling `Auth.SignOut()` to stop the association.
* **[Fix]** Fix updating push installation when setting or unsetting the user identifier by calling `AppCenter.setUserId`.

___

## Version 1.14.0

### AppCenter

#### iOS

* **[Fix]** Fix a crash in case decrypting a value failed.

#### Android

* **[Fix]** Fix network connection state tracking issue, which prevented sending data in some restricted networks.
* **[Fix]** Fix possible deadlock on changing network connection state.

### AppCenterPush

#### iOS

* **[Fix]** Fix crash on invoking an optional push callback when it isn't implemented in the push delegate.

### AppCenterDistribute

#### Android

* **[Fix]** Fix in-app updates not working on devices using Xiaomi MIUI from versions 10 and above.

___

## Version 1.13.2

### AppCenter

* **[Improvement]** Add missing XML documentation in the NuGet packages.

#### Android

* **[Fix]** The SDK normally disables storing and sending logs when SQLite is failing instead of crashing the application. New SQLite APIs were introduced in version 1.9.0 and the new API exceptions were not caught, this is now fixed.

#### iOS

* **[Fix]** Fix a possible deadlock if the SDK is started from a background thread.
* **[Fix]** Fix a crash if database query failed.

### AppCenterDistribute

#### Android

* **[Fix]** Fix exception if we receive deep link intent with setup failure before onStart.
* **[Fix]** Fix checking updates for applications installed on corporate-owned single-use devices.

#### iOS

* **[Fix]** Fix a race condition crash on upgrading the application to newer version.

### AppCenterCrashes

#### iOS

* **[Fix]** Fix a crash on iOS when triggering null reference exception and starting Crashes in background. The conditon might still occur very rarely if trigger a null reference exception in another thread during the short time where the [SDK configures native crash reporter](https://www.mono-project.com/docs/advanced/signals/). It is thus recommended to initialize AppCenter Crashes as early as possible (which is also recommended to capture early crashes).

___

## Version 1.13.0

### AppCenter

#### Android

* **[Feature]** AppCenter SDK now supports the User ID string, with a maximum of 256 characters, that applies to crashes, handled errors, and push logs. Settable via `AppCenter.SetUserId()`.

* **[Fix]** Fix TLS 1.2 configuration for some specific devices running API level <21. The bug did not affect all devices running older API levels, only some models/brands, and prevented any data from being sent.

#### iOS

* **[Feature]** AppCenter SDK now supports the User ID string, with a maximum of 256 characters, that applies to crashes, handled errors, and push logs. Settable via `AppCenter.SetUserId()`.
* **[Fix]** Fix issue where the SDK source could not be built in a directory that contains escaped characters (applies to all modules).

### AppCenterAnalytics

#### UWP

* **[Fix]** Extend the current session instead of starting a new session when sending events from the background.

#### Android

* **[Fix]** Extend the current session instead of starting a new session when sending events from the background. Sessions are also no longer started in background by sending an event or a log from another service such as push, as a consequence the push registration information will be missing from crash events information.

### AppCenterDistribute

#### Android

* **[Fix]** Fix issue with forcing Chrome to open links when other browsers are the default.
___

## Version 1.12.0

### AppCenter

#### UWP

* **[Fix]** Update vulnerable `Microsoft.NETCore.UniversalWindowsPlatform` dependency from version `5.2.2` to `5.2.6`.

#### Android

* **[Fix]** Do not delete old logs when trying to add a log larger than the maximum storage capacity.
* **[Fix]** Fix disabling logging of network state changes according to AppCenter.getLogLevel.
* **[Fix]** Fix logs duplication on unstable network.

#### iOS

* **[Fix]** Do not delete old logs when trying to add a log larger than the maximum storage capacity.
* **[Fix]** Fix reporting carrier information using new iOS 12 APIs when running on iOS 12+.
* **[Fix]** Fix a memory leak issue during executing SQL queries.

### AppCenterCrashes

#### UWP

* **[Fix]** Downgrade the platforms verification error to a warning.

#### Android

* **[Fix]** Fix a bug where crash data file could leak when the database is full.

### AppCenterPush

#### Android

* **[Fix]** Fix `PushNotificationReceived` event for pushes received in foreground after re-enabling the push service.

___

## Version 1.11.0

### AppCenter

#### iOS

* **[Fix]** Fix an issue where concurrent modification of custom properties was not thread safe.
* **[Fix]** Fix validating and discarding Not a Number (NaN) and infinite double values for custom properties.
* **[Fix]** Use standard SQL syntax to avoid affecting users with custom SQLite libraries.
* **[Fix]** Get database page size dynamically to support custom values.

### AppCenterCrashes

#### Android

* **[Fix]** Preventing stack overflow crash while reading a huge throwable file.

### AppCenterPush

#### iOS

* **[Fix]** Fix `PushNotificationReceived` callback not triggered on notification tapped or received in foreground when a `UNUserNotificationCenterDelegate` is set. If you have implemented this delegate please remove any call to the `Push.DidReceiveRemoteNotification` method as it's now handled by the new [User Notification Center Delegate Forwarder](https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-ios).

___

## Version 1.10.0

### AppCenter

#### Android

* **[Security]** To enforce TLS 1.2 on all HTTPS connections the SDK makes, we are dropping support for API level 15 (which supports only TLS 1.0), the minimum SDK version thus becomes 16. Previous versions of the SDK were already using TLS 1.2 on API level 16+.
* **[Bug fix]** Fix validating and discarding `NaN` and infinite double values when calling `setCustomProperties`.

#### iOS

* **[Fix]** Add missing network request error logging.

#### UWP

* **[Fix]** Fix a crash when system or user locale cannot be read by falling back to using app locale.
* **[Fix]** Fix compatibility with SQLite 1.5.231.

### App Center Crashes

#### iOS

* **[Fix]** Fix the list of binary images in crash reports for arm64e-based devices.

### App Center Distribute

#### iOS

* **[Fix]** Fix translation of closing a dialog in Portuguese.

___

## Version 1.9.0

### App Center

#### iOS

* **[Fix]** Fix a potential deadlock that can freeze the application launch causing the iOS watchdog to kill the application.

### App Center Crashes

#### Android

* **[Fix]** Fix a bug where some initialize operations were executed twice.
* **[Fix]** Fix a bug where device information could be null when reading the error report client side.

#### iOS

* **[Fix]** The above deadlock was mostly impacting the Crashes module.

### App Center Distribute

#### Android

* **[Fix]** Fix a crash that could happen when starting the application.

### App Center Push

* **[Fix]** Fix a crash that could happen when using Push on devices behind a firewall or unstable network.

___

## Version 1.8.0

### App Center

#### UWP

* **[Fix]** Fix occasional crash when checking network connectivity.

### App Center Distribute

#### iOS

* **[Fix]** Fix a bug where browser would open randomly when in-app updates were already enabled.

### App Center Push

#### Android

The Firebase messaging SDK is now a dependency of the App Center Push SDK to be able to support Android P and also prevent features to break after April 2019 based on [this announcement](https://firebase.googleblog.com/2018/04/time-to-upgrade-from-gcm-to-fcm.html).

You need to follow [some migration steps](https://docs.microsoft.com/en-us/appcenter/sdk/push/migration/xamarin-android) after updating the SDK to actually use Firebase instead of the manual registration mechanism that we are providing. The non Firebase mechanism still works after updating the SDK but you will see a deprecation message, but this will not work on Android P devices until you migrate.

After updating the app to use Firebase, you will also no longer see duplicate notifications when uninstalling and reinstalling the app on the same device and user.
___

## Version 1.7.0

### AppCenter

#### Android

* **[Improvement]** Enable TLS 1.2 on API levels where it's supported but not enabled by default (API level 16-19, this became a default starting API level 20). Please note we still support Android API level 15 and it uses TLS 1.0.
* **[Improvement]** Gzip is used over HTTPS when request size is larger than 1.4KB.
* **[Fix]** Fix a crash when disabling a module at the same time logs are sent.
* **[Fix]** Fix pretty print JSON in Android P when verbose logging is enabled.

### AppCenterCrashes

#### UWP

* **[Fix #543]** Fix warning "The referenced component 'Microsoft.AppCenter.Crashes.dll' could not be found". You might need to clean solution and package cache after updating nuget to resolve the warning for good.

### AppCenterPush

#### UWP

* **[Fix]** Prevent application from crashing when push setup fails due to unexpected errors.

___

## Version 1.6.1

This version contains bug fixes.

### AppCenter

#### Android

* **[Fix]** Fix a crash when network state changes at same time as SDK initializing.
* **[Fix #664]** Fix crashes when trying to detect we run on instrumented test environment.
* **[Fix #649]** Fix a deadlock when setting wrapper SDK information or setting log url while other channel operations performed such as when Crashes is starting.

#### iOS

* **[Fix #670]** [Previous release](https://github.com/microsoft/appcenter-sdk-dotnet/releases/tag/1.6.0) was unintentionally requiring Mono 5.8 on iOS applications. This build issue is now fixed.

#### UWP

* **[Fix #613]** When marking an unhandled exception as handled, the SDK was shutting down and no further log could be sent. This is now fixed.

### AppCenterCrashes

#### Xamarin

* **[Fix]** Fix a crash when calling `Crashes.TrackError` exception parameter with null, now prints an error instead.
* **[Fix]** Fix a SDK crash while saving original crash if SDK cannot serialize the exception for client side inspection (with an exception that is not SerializableException).
* **[Fix]** When looking at `(await Crashes.GetLastSessionCrashReportAsync()).Exception` or when using the sending callbacks, the Exception object is now null instead of containing a serialization exception (on failure to get original exception object after restart). This is only for client side inspection, the crash stack trace is correctly reported to the server in all scenarios.

#### Android

* **[Fix]** Fix reporting crash when process name cannot be determined.

#### iOS

* **[Fix]** Fix an issue in breadcrumbs feature when events are being tracked on the main thread just before a crash.
* **[Fix]** Fix an issue with cached logs for breadcrumbs feature which are sometimes not sent during app start.

### AppCenterPush

#### Android

* **[Fix]** Fix notification text being truncated when large and now supports multi-line.

___

## Version 1.6.0

This version contains improvements and bug fixes.

### Android, iOS and UWP

#### Analytics

* **[Improvement]** Analytics now allows a maximum of 20 properties by event, each property key and value length can be up to 125 characters long.

### Android

#### Crashes

* **[Fix]** Fix capturing exceptions thrown directly from a .NET Thread.

#### Push

* **[Feature]** Configure default notification icon and color using meta-data.
* **[Fix]** Fixes the google.ttl field being considered custom data.
* **[Fix]** Fixes push notification not displayed if Google Play Services too old on the device (#630).
* **[Fix]** Don't crash the application when invalid notification color is pushed.

### iOS

#### Push

* **[Fix]** Fix parsing custom data for push to avoid crash in the application.

### UWP

#### Analytics

* **[Fix]** Fix session identifier when renewing session after background or after re-enabling SDK.
* **[Fix]** Avoid using tasks in background that can stopped responding, use timers instead. Related to #517.
* **[Fix]** Fix storage exception catching so that they don't crash the app.
* **[Fix]** Fix some stability issues in the https sending logic.

___

## Version 1.5.0

This version contains new features.

### Android and iOS

#### AppCenterCrashes

* **[Feature #140]** Add support for handled exceptions with `Crashes.TrackError` API.

#### AppCenterDistribute

* **[Feature]** Add Session statistics for distribution group.
___

## Version 1.4.0

This version contains a new feature.

### Android and iOS

#### AppCenterDistribute

* **[Feature]** Use tester app to enable in-app updates if it's installed.
* **[Feature]** Add reporting of downloads for in-app update.
* **[Improvement]** Add distribution group to all logs that are sent.
___

## Version 1.3.0

This version has a **breaking change** as the SDK now requires iOS 9 or later. It also contains bug fixes and improvements.

### iOS

#### AppCenter

* **[Improvement]** Successful configuration of the SDK creates a success message in the console with log level INFO instead of ASSERT. Errors during configuration will still show up in the console with the log level ASSERT.

#### AppCenterCrashes

* **[Fix]** Fix an issue where crashes were not reported reliably in some cases when used in Xamarin apps or when apps would take a long time to launch.

### Android

#### AppCenter

* **[Fix]** Fix events association with crashes.
* **[Fix]** Fix network state detection.
* **[Fix]** Don't retry sending logs on HTTP error 429.
* **[Fix]** Some logs were not sent or discarded correctly on AppCenter enabled/disabled state changes.

#### AppCenterCrashes

* **[Improvement]** Increase attachment file size limit from 1.5MB to 7MB.

#### AppCenterPush

* **[Fix]** Fix a crash on Android 8.0 (exact version, this does not happen in 8.1) where having an adaptive icon (like launcher icon) would cause crash of the entire system U.I. on push. On Android 8.0 we replace the adaptive icon by a placeholder icon (1 black pixel) to avoid the crash, starting Android 8.1 the adaptive icon is displayed without fallback.
___

## Version 1.2.0

This version has a potentially **breaking change** (in push behavior) with bug fixes and improvements.

### UWP

#### AppCenter

* **[Fix]** The network loss detection code was missing some scenarios.
* **[Fix]** Disabling the SDK could send extra logs and could also discard logs supposed to be sent when re-enabled.

### iOS

#### AppCenter

* **[Fix]** Fix an issue that enables internal services even if App Center was disabled in previous sessions.
* **[Fix]** Fix an issue not to delete pending logs after maximum retries.

#### AppCenterCrashes

* **[Improvement]** Improve session tracking to get appropriate session information for crashes if an application also uses Analytics.

#### AppCenterPush

* **[Fix]** Fix "Missing Push Notification Entitlement" warning message after uploading an application to TestFlight and publishing to App Store.
* **[Improvement] (Breaking Change)** In previous versions, it was required to add code to `DidReceiveRemoteNotification` callback in your application delegate if you or 3rd party libraries already implemented this callback. This is no longer necessary.
  This is a **breaking change** for some use cases because it required modifications in your code. Not changing your implementation might cause push notifications to be received twice.
  * If you don't see any implementation of `DidReceiveRemoteNotification` callback in your application delegate, you don't need to do anything, there is no breaking change for you.
  * If you want to keep automatic forwarding disabled, you also don't need to do anything.
  * If your application delegate contains implementation of `DidReceiveRemoteNotification`, you need to remove the following code from your implementation of the callback. This is typically the case when you or your 3rd party libraries implement the callback.

    ```csharp
    public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, System.Action<UIBackgroundFetchResult> completionHandler)
    {
        var result = Push.DidReceiveRemoteNotification(userInfo);
        if (result)
        {
            completionHandler?.Invoke(UIBackgroundFetchResult.NewData);
        }
        else
        {
            completionHandler?.Invoke(UIBackgroundFetchResult.NoData);
        }
    }
    ```

___

## Version 1.1.0

### AppCenter

#### iOS

* **[Fix]** Fix a locale issue that doesn't properly report system locale if an application doesn't support current language.
* **[Improvement]** Change log level to make HTTP failures more visible, and add more logs.

#### UWP

* **[Fix #390]** The SDK is now compatible with sqlite-net-pcl 1.5.\* (currently in beta). It is still compatible with 1.3.\*. Note that it is still not compatible with versions in the 1.4.\* range due to a known issue.
* **[Fix]** Fix a locale issue that doesn't properly report system locale if an application doesn't support current language.
* **[Improvement]** HTTP warning and error logs now include more details: status code and full response body are now printed.

### AppCenterCrashes

#### Android

* **[Fix]** Fix a crash when sending an attachment larger than 1.4MB. The SDK is still unable to send large attachments (for Android) in this release but now it does not crash anymore. An error log is printed instead.
* **[Improvement]** Java exceptions caught from .NET runtime are now reported with .NET stack trace frames on the server. However the client side exception object returned by the SDK APIs will still be null and you have to look at `AndroidDetails` property to get the Java Throwable that caused this type of crash. (Java.Lang.Throwable is **not serializable** to a System.Exception object).

#### UWP

* **[Fix #544]** Remove message that crashes are not supported on UWP. Please note that none of the client side APIs besides `AppCenter.Start` are supported in UWP as crash reporting is provided by a system level component in Windows. Errors when registering the system level crash reporting are now also logged.

### AppCenterDistribute

#### Android and iOS

* **[Improvement]** Updated translations.
* **[Improvement]** Users with app versions that still use Mobile Center can directly upgrade to versions that use this version of App Center, without the need to reinstall.

### AppCenterPush

#### Android

* **[Improvement]** The Firebase SDK dependency is now optional. If Firebase SDK is not available at runtime, the push registers and generate notifications using only App Center SDK. The Firebase application and servers are still used whether the Firebase SDK is installed into the application or not.
  * The SDK is still compatible with Firebase packages. But if you don't use Firebase besides App Center, you can now remove these packages and refer to the [updated getting started instructions](https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-android) to migrate the set up. Migrating the push set up in the application remains optional.

#### UWP

* **[Fix #518]** Fix a crash that could occur when starting Push.
___

## Version 1.0.1

This release fixes the NuGet packages that were released for [1.0.0](https://github.com/microsoft/appcenter-sdk-dotnet/releases/tag/1.0.0).

Please read the [1.0.0 release notes](https://github.com/microsoft/appcenter-sdk-dotnet/releases/tag/1.0.0) if you are not yet aware of this version.

Fixes from 1.0.1:

* Fix support for PCL that was broken in 1.0.0.
* Fix nuget licence acceptance.

___

## Version 1.0.0

### General Availability (GA) Announcement.

This version contains **breaking changes** due to the renaming from Mobile Center to App Center. In the unlikely event there was data on the device not sent prior to the update, that data will be discarded.

Please follow [the migration guide](https://docs.microsoft.com/en-us/appcenter/sdk/sdk-migration/xamarin) to update from an earlier version of Mobile Center SDK.

### AppCenter

#### iOS

* **[Fix]** Don't send startService log while SDK is disabled.

### AppCenterDistribute

#### iOS

* **[Fix]** Fix a bug where unrecoverable HTTP error wouldn't popup the reinstall app dialog after an app restart.
* **[Improvement]** Adding missing translations.
* **[Known bug]** Checking for last updates will fail if the app was updating from a Mobile Center app. A pop up will show next time the app is restarted to ask for reinstallation.

#### Android

* **[Fix]** The view release notes button was not correctly hidden when no release notes were available.
* **[Fix]** Added missing translations for failed to enable in-app update dialog title and buttons. The main message, however, is not localized yet as it's extracted from a REST API text response.
* **[Known issue]** When updating an application that uses Mobile Center SDK using the in-app update dialog to an application version that uses AppCenter SDK version, the browser will be re-opened and will fail. User will need to re-install the application from the App Center portal.

___

## Version 1.17.1

This release include 2 bug fixes.

### MobileCenterAnalytics

* **[Fix]** Prevents a possible crash on UWP startup #484

### MobileCenterPush

* **[Improvement]** Updates dependencies on Xamarin Firebase Push to avoid most package conflicts when updating Mobile Center Push on Android.
* **[Fix]** Fixes a crash when receiving a silent push in background with no message on iOS.

### Native SDK Updates

* Fixes from [iOS SDK version 0.14.1](https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.14.1).

___

## Version 1.17.0

This version contains bug fixes, a new API, and improvements.

### MobileCenter

* **[Improvement]** The NuGet packages now support .NET standard without adding extra project configuration, and the nugets remain compatible with PCL projects.
* **[Feature]** Added a `MobileCenter.SdkVersion` property to get the current version of Mobile Center SDK programmatically.

### MobileCenterPush

* **[Breaking change]** To support Android 8.0 applications that use `targetSdkVersion="26"` we updated the Firebase SDK dependency which forces applications to use Android 8.0 Mono frameworks.
    * You need to have Xamarin tools up to date, select Android 8.0 as a target framework and then make sure that in `package.config` the attribute for every package looks like this: `targetFramework="monoandroid80"` and update as necessary. It's highly recommended to close Studio and clean every obj/bin folder manually in case of any build error after the update.
    * If you get a conflict with `Xamarin.GooglePlayServices.Basement` package, you need to update this one to **prerelease** version `60.1142.0-beta1` (ore more recent) and retry updating the Mobile Center Push package.
* **[Fix]** Fixed: prior to this release, calling `Push.SetEnabledAsync(true)` during startup could prevent UWP push registration. Please note that any call to `SetEnabledAsync(true)` for any module is useless if you have not called it with `false` before, modules are enabled by default once included in the `Start` call.

### Native SDK updates

Embed changes from the underlying native SDK releases:

* https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.13.0
* https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.14.0

___

## Version 0.16.0

This release includes bug fixes for iOS from the native SDK: https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.13.0.

Please note that this release does not yet provide the .NET equivalent of the sdkVersion method introduced by the iOS change.

___

## Version 0.15.2

This release includes bug fixes for iOS from the native SDK: https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.3.

This release thus fixes the Mobile Center build incompatibility introduced in version [0.15.1](https://github.com/Microsoft/mobile-center-sdk-dotnet/releases/tag/0.15.1).

___

## Version 0.15.1

This release includes bug fixes for iOS from the native SDK: https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.2

**Known issue** This release causes build error in Mobile Center or in environments where XCode version is lower than 9. As a consequence, this release was unlisted from nuget.org.

___

## Version 0.15.0

When you update to this release, there will be potential data loss if an application installed with previous versions of MobileCenter SDK on devices that has pending logs which are not sent to server yet at the time of the application is being updated.

* New feature: in app updates can now be received from public distribution groups.
* Fix a bug in UWP SDK where an `InvalidOperationException` could sometimes be thrown at launch time.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.12.0
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.1
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.12.0

___

## Version 0.14.2

* Fix a bug in UWP where some crashes, especially those caused by `AggregateException`, would not be sent properly.
* Fix a bug in UWP where Analytics would always emit a warning on startup.
* Reword some logs that used to look like errors when starting Mobile Center in multiple places to appear less harmful. (Issue #396).
* Improve the reliability of device screen size retrieval in UWP (used in device properties sent with logs).
* Fix a bug where getting the screen size in UWP could, in some cases, cause a crash. (Issue #398).
* Change the behavior of Analytics events so that now, when event fields exceed their capacity, they are truncated instead of dropped altogether. Limits:
  * Event name maximum length: 256 characters.
  * Maximum number of event properties: 5.
  * Event property key maximum length: 64 characters.
  * Event property value maximum length: 64 characters.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.2
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.2
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.1

___

## Version 0.14.0

### Async APIs and Android strict mode

This release focuses on fixing Android strict mode issues (including Android 8 ones).

Since strict mode checks if you spend time reading storage on U.I. thread we had to make the following APIs asynchronous and is thus a breaking change on PCL and all targets:

* `{AnyClass}.Enabled` is now split into `GetEnabledAsync()` and `SetEnabledAsync(bool)`
* `MobileCenter.InstallId` is now `MobileCenter.GetInstallIdAsync()`
* `Crashes.HasCrashedInLastSession` is now `Crashes.HasCrashedInLastSessionAsync()`

### Other changes

#### Android

* The Android crash reporting is now compatible with debugger attached (you need to continue execution after exception), Hockey App for Xamarin (only if initialized like getting started suggests) and `XA_BROKEN_EXCEPTION_TRANSITIONS=true`. This change is for **Android only**.
* This release also includes changes from underlying Android SDK of the following versions:
  * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.0
  * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.11.1

#### iOS

* Fix null argument exceptions when passing null arguments to most APIs, an error is logged instead now.
* This release also includes changes from underlying iOS SDK:
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.11.0
* Xamarin SDK calls `MSCrashes.disableMachExceptionHandler()` automatically as it's not suitable in Xamarin environment.

#### UWP

* Fix various race conditions.
* Fix screen size reporting.
* Fix a bug where U.I. could be blocked for a long time when initializing push.
* Fix null reference exception when calling `SetCustomProperties` before `Start`, now logs an error.

___

## Version 0.13.1

Roll back version [0.13.0](https://github.com/Microsoft/mobile-center-sdk-dotnet/releases/tag/0.13.0) .NET standard change for portable libraries due to reported issues with it.

Also, version 0.13.0 was adding unnecessary packages to Android and iOS projects, and they can now be safely uninstalled. You will find a list of these unnecessary packages in the [packages.config](https://github.com/Microsoft/mobile-center-sdk-dotnet/releases/download/0.13.1/packages.config) file attached to this release.

If your portable project is using .NET standard, you can still use our nugets by adding this to your **project.json** file:

```
  "frameworks": {
    "netstandard1.{DotNETStandardVersion}": {
        "imports": "portable-net45+win8+wpa81"
    }
}
```

After the dependencies section.

Example:

```
{
  "dependencies": {
    "Microsoft.Azure.Mobile.Analytics": "0.13.1",
    "Microsoft.Azure.Mobile.Crashes": "0.13.1",
    "Microsoft.Azure.Mobile.Distribute": "0.13.1",
    "Microsoft.Azure.Mobile.Push": "0.13.1",
    "NETStandard.Library": "1.6.1"
  },
  "frameworks": {
    "netstandard1.0": {
        "imports": "portable-net45+win8+wpa81"
    }
  }
}
```

___

## Version 0.13.0

* Add `MobileCenter.SetCustomProperties` method to enable audience segmentation with push notifications.
* Fix a bug in UWP where logs would be mishandled on a network outage.
* Move initialization of `HttpClient` class to a background thread in UWP.
* Fix bug in UWP SDK where starting `MobileCenter` from another place than `OnLaunched` sometimes crashed. Also removed the error log, now you can init the SDK in constructor too if you prefer.
* Fix various race conditions in UWP.
* Fix a bug where 2 sessions could be reported at once when resuming from background (UWP and Android).
* Add a new Android specific method `Push.CheckLaunchedFromNotification` to use in `OnNewIntent` if `launchMode` of the activity is not standard to fix push listener when clicking on background push and recycling the activity.
* Switch from PCL to .NET Standard for the portable projects.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.10.0
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.10.1
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.10.0

___

## Version 0.12.0

* Add capability to attach data to crash reports (via the `Crashes.GetErrorAttachments` callback) on Xamarin.
* Fix a crash on old Xamarin.Android versions where mono version could not be read (`System.IndexOutOfRangeException` when calling `Start`).
* Fix push event on UWP.
* Fix `Push.Enabled` property behavior on UWP.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.9.0
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.9.0

___

## Version 0.11.1

* Add Push service to Xamarin.Android and Xamarin.iOS to enable sending push notifications to your apps from the Mobile Center portal.
* This release also includes changes from underlying Android and iOS SDKs of the following versions:
  * Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.8.0
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.8.1
  * iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.8.0
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.8.1

___

## Version 0.10.0

* Add Push service to UWP to enable sending push notifications to your apps from the Mobile Center portal.

___

## Version 0.9.2

* Fix `ReleaseAvailable` callback in iOS to be able to customize the update dialog on iOS too.
* Fix showing default update dialog after resetting `ReleaseAvailable` callback to `null`.

___

## Version 0.9.1

This version contains bug fixes, improvements and new features.

### MobileCenter

* Add method to manually set the country code in UWP apps: `MobileCenter.SetCountryCode(string)`.
* Fix a bug where installing the Mobile Center NuGet packages can cause Mobile Center types not to appear in Intellisense.
* Use another sqlite package dependency in UWP.
* Add xamarin runtime version in logs sent to backend.

### Analytics

Events have some validation and you will see the following in logs:

* An error if the event name is null, empty or longer than 256 characters (event is not sent in that case).
* A warning for invalid event properties (the event is sent but without invalid properties):
  * More than 5 properties per event (in that case we send only 5 of them and log warnings).
  * Property key null, empty or longer than 64 characters.
  * Property value null or longer than 64 characters.

### Crashes

* Improve reporting and rendering of some Java `ClassNotFoundException` caught from .NET side.

### Distribute

* New distribute callback to provide an ability of in-app update customization (on Android only for now).
* New default update dialog with release notes view.

### Android

Include changes from https://github.com/Microsoft/mobile-center-sdk-android/releases/0.7.0

### iOS

Include changes from https://github.com/Microsoft/mobile-center-sdk-ios/releases/0.7.0

___

## Version 0.8.1

This release introduces the following features:

* Distribute module for Xamarin.iOS and Xamarin.Android.
* Analytics module support for UWP platform.

Those 2 features works also when using PCL and Xamarin Forms.

This release also includes changes from underlying Android and iOS SDKs of the following versions:

* Android
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.6.0
    * https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.6.1
* iOS
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.6.0
    * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.6.1

___

## Version 0.7.0

- **[Feature]** Can now initialize the SDK with multiple app secrets for different platforms (Android vs iOS) in the same `Start` call. No need to split the initialization between `Start` and `Configure` anymore. Just use `MobileCenter.Start("android={androidAppSecret};ios={iosAppSecret}", typeof(Analytics), typeof(Crashes));` for example.
- **[Bug fix]** Fix a crash happening on iOS devices before iOS 10 when reading `MobileCenter.InstallId` property.
- **[Misc]** Renamed `SetServerUrl` to `SetLogUrl`.
- **[iOS]** Include underlying iOS SDK changes from the following releases:
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.5.1
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.5.0
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.4.2
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.4.1
  * https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.4.0

___

## Version 0.6.0

**[Breaking changes]**
- Remove Crashes APIs related to attachments as it's not supported by backend yet.
- `Crashes.LastSessionCrashReport` property becomes `Crashes.GetLastSessionCrashReportAsync()` async method.

[Bug fixes]
- Include underlying iOS SDK changes from the following releases:
  - https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.7
  - https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.6
  - https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.5
- Include underlying Android SDK changes from the following releases:
  - https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.5.0
  - https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.4.0

___

## Version 0.5.0

- Add `MobileCenter.IsConfigured` boolean property to check for SDK already configured.
- Fix issue where sdk could cause ios apps to crash while trying to handle SIGFPE signal.
- Fix a crash when starting analytics when using "Link All" in iOS.
- Fix a crash when calling `MobileCenter.InstallId` before `MobileCenter.Configure` in Android, now returns `null` like in the underlying SDK in that case.
- Include underlying [iOS SDK v0.3.4](https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.4).
- Include underlying [Android SDK v0.3.3](https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.3.3).

___

## Version 0.4.1

- Fix crash that would occur when disabling Crashes service in iOS.
- Fix crash when trying to include a null ErrorAttachment in an ErrorReport.

___

## Version 0.4.0

- Log a debug message instead of crashing in UWP projects.
- Add raw stack trace information for backend processing.
- Include underlying [iOS SDK v0.3.3](https://github.com/Microsoft/mobile-center-sdk-ios/releases/tag/0.3.3).
- Include underlying [Android SDK v0.3.2](https://github.com/Microsoft/mobile-center-sdk-android/releases/tag/0.3.2).

___

## Version 0.3.0

- Fix linker issues in iOS, you can also remove safely the `MobileCenterFrameworks` folder from iOS projects.
- Fix issue where sdk could cause ios apps to crash while trying to handle signals.
- Fix a crash when starting MobileCenter on Android when "link all assemblies" is used.
- Add advanced crash features that were already available since version 0.2.0 of Android and iOS SDKs:
  - Last session crash report.
  - Crash callbacks.

___

## Version 0.2.1

Rename Android bindings so that they don't get used by mistake instead of the PCL.

___

## Version 0.2.0

- Rename `Initialize` to `Configure`.
- Embed Android and iOS SDKs version 0.3.0.

___

## Version 0.1.0

- First release of Xamarin SDK.
- Using version 0.2.0 of underlying Android and iOS SDKs.
- Last session error report details and crash callbacks are not yet available.
