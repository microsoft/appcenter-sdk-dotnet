// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;
using Microsoft.AppCenter.Apple.Bindings;

namespace Microsoft.AppCenter
{
    public partial class AppCenter
    {
        /* The key identifier for parsing app secrets */
#if __IOS__
        const string PlatformIdentifier = "ios";
#elif __MACOS__
        const string PlatformIdentifier = "macos";
#endif

        internal AppCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var val = MSACAppCenter.LogLevel();
                switch (val)
                {
                    case MSACLogLevel.Verbose:
                        return LogLevel.Verbose;
                    case MSACLogLevel.Debug:
                        return LogLevel.Debug;
                    case MSACLogLevel.Info:
                        return LogLevel.Info;
                    case MSACLogLevel.Warning:
                        return LogLevel.Warn;
                    case MSACLogLevel.Error:
                        return LogLevel.Error;
                    case MSACLogLevel.Assert:
                        return LogLevel.Assert;
                    case MSACLogLevel.None:
                        return LogLevel.None;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(val), val, null);
                }
            }
            set
            {
                MSACLogLevel loglevel;
                switch (value)
                {
                    case LogLevel.Verbose:
                        loglevel = MSACLogLevel.Verbose;
                        break;
                    case LogLevel.Debug:
                        loglevel = MSACLogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        loglevel = MSACLogLevel.Info;
                        break;
                    case LogLevel.Warn:
                        loglevel = MSACLogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        loglevel = MSACLogLevel.Error;
                        break;
                    case LogLevel.Assert:
                        loglevel = MSACLogLevel.Assert;
                        break;
                    case LogLevel.None:
                        loglevel = MSACLogLevel.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                MSACAppCenter.SetLogLevel(loglevel);
            }
        }

        static bool PlatformIsNetworkRequestsAllowed
        {
            get => MSACAppCenter.IsNetworkRequestsAllowed();
            set => MSACAppCenter.SetNetworkRequestsAllowed(value);
        }

        static void PlatformSetUserId(string userId)
        {
            MSACAppCenter.SetUserId(userId);
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            MSACAppCenter.SetLogUrl(logUrl);
        }

        static void PlatformSetCountryCode(string countryCode)
        {
            MSACAppCenter.SetCountryCode(countryCode);
        }

        static void PlatformSetDataResidencyRegion(string dataResidencyRegion)
        {
            MSACAppCenter.setDataResidencyRegion(dataResidencyRegion);
        }

        static bool PlatformConfigured
        {
            get
            {
                return MSACAppCenter.IsConfigured();
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            SetWrapperSdk();
            MSACAppCenter.ConfigureWithAppSecret(appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            SetWrapperSdk();
            foreach (var service in GetServices(services))
            {
                MSACAppCenter.StartService(service);
            }
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            SetWrapperSdk();
            string parsedSecret;
            try
            {
                parsedSecret = GetSecretAndTargetForPlatform(appSecret, PlatformIdentifier);
            }
            catch (AppCenterException ex)
            {
                AppCenterLog.Assert(AppCenterLog.LogTag, ex.Message);
                return;
            }
            MSACAppCenter.Start(parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSACAppCenter.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSACAppCenter.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            Guid? installId = Guid.Parse(MSACAppCenter.InstallId().AsString());
            return Task.FromResult(installId);
        }

        static Class[] GetServices(IEnumerable<Type> services)
        {
            var classes = new List<Class>();
            foreach (var t in services)
            {
                var bindingType = GetBindingType(t);
                if (bindingType != null)
                {
                    var aClass = GetClassForType(bindingType);
                    if (aClass != null)
                    {
                        classes.Add(aClass);
                    }
                }
            }
            return classes.ToArray();
        }

        static Class GetClassForType(Type type)
        {
            IntPtr classHandle = Class.GetHandle(type);
            if (classHandle != IntPtr.Zero)
            {
                return new Class(classHandle);
            }
            return null;
        }

        static Type GetBindingType(Type type)
        {
            return (Type)type.GetProperty("BindingType")?.GetValue(null, null);
        }

        static void SetWrapperSdk()
        {
            MSACWrapperSdk wrapperSdk = new MSACWrapperSdk(WrapperSdk.Version, WrapperSdk.Name, Constants.Version, null, null, null);
            MSACAppCenter.SetWrapperSdk(wrapperSdk);
        }

        internal static void PlatformUnsetInstance()
        {
            MSACAppCenter.ResetSharedInstance();
        }

        static Task<bool> PlatformSetMaxStorageSizeAsync(long sizeInBytes)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            MSACAppCenter.SetMaxStorageSize(sizeInBytes, (result) => taskCompletionSource.SetResult(result));
            return taskCompletionSource.Task;
        }
    }
}
