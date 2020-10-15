[![GitHub Release](https://img.shields.io/github/release/microsoft/appcenter-sdk-dotnet.svg)](https://github.com/microsoft/appcenter-sdk-dotnet/releases/latest)
[![NuGet](https://img.shields.io/nuget/v/Microsoft.AppCenter.svg)](https://www.nuget.org/packages/Microsoft.AppCenter/)
[![license](https://img.shields.io/badge/license-MIT%20License-yellow.svg)](https://github.com/microsoft/appcenter-sdk-dotnet/blob/master/license.txt)

# Visual Studio App Center SDK for .NET

App Center is your continuous integration, delivery and learning solution for iOS, Android, and Windows apps.
Get faster release cycles, higher-quality apps, and the insights to build what users want.

The App Center SDK uses a modular architecture so you can use any or all of the following services:

1. **App Center Analytics**: App Center Analytics helps you understand user behavior and customer engagement to improve your app. The SDK automatically captures session count, device properties like model, OS version, etc. You can define your own custom events to measure things that matter to you. All the information captured is available in the App Center portal for you to analyze the data.

2. **App Center Crashes**: App Center Crashes will automatically generate a crash log every time your app crashes. The log is first written to the device's storage and when the user starts the app again, the crash report will be sent to App Center. Collecting crashes works for both beta and live apps, i.e. those submitted to the App Store. Crash logs contain valuable information for you to help fix the crash.

3. **App Center Distribute**: App Center Distribute will let your users install a new version of the app when you distribute it via the App Center. With a new version of the app available, the SDK will present an update dialog to the users to either download or postpone the new version. Once they choose to update, the SDK will start to update your application.
    > **Google Play considers the in-app update code as malicious behavior even if it isn’t used at runtime. Please use App Center Distribute Play instead before submitting your app to Google Play. Failure to not remove the in-app update code can lead to noncompliance and removal of the app from Google Play.**
    > See [Remove in-app updates for Google Play builds](https://docs.microsoft.com/en-us/appcenter/sdk/distribute/xamarin#remove-in-app-updates-for-google-play-builds) documentation for details.

4. **App Center Distribute Play**: App Center Distribute Play is stubbing the Distribute package's APIs to avoid Google Play rejecting the application for malicious behavior. It must be used only for build variants which are going to be published on Google Play.

## 1. Get started

It is super easy to use App Center. Have a look at our [get started documentation](https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/xamarin) and onboard your app within minutes. Our [detailed documentation](https://docs.microsoft.com/en-us/appcenter/sdk/) is available as well.

## 2. Contributing

We are looking forward to your contributions via pull requests.

### 2.1 Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

### 2.2 Contributor License

You must sign a [Contributor License Agreement](https://cla.microsoft.com/) before submitting your pull request. To complete the Contributor License Agreement (CLA), you will need to submit a request via the [form](https://cla.microsoft.com/) and then electronically sign the CLA when you receive the email containing the link to the document. You need to sign the CLA only once to cover submission to any Microsoft OSS project. 

## 3. Contact

### 3.1 Support

App Center SDK support is provided directly within the App Center portal. Any time you need help, just log in to [App Center](https://appcenter.ms), then click the blue chat button in the lower-right corner of any page and our dedicated support team will respond to your questions and feedback. For additional information, see the [App Center Help Center](https://intercom.help/appcenter/getting-started/welcome-to-app-center-support).

### 3.2 Twitter

We're on Twitter as [@vsappcenter](https://www.twitter.com/vsappcenter).
