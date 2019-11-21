// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;

namespace Microsoft.AppCenter
{
    using tvOSLogLevel = Microsoft.AppCenter.tvOS.Bindings.MSLogLevel;
    using tvOSAppCenter = Microsoft.AppCenter.tvOS.Bindings.MSAppCenter;
    using tvOSWrapperSdk = Microsoft.AppCenter.tvOS.Bindings.MSWrapperSdk;

    public partial class AppCenter
    {
        /* The key identifier for parsing app secrets */
        const string PlatformIdentifier = "tvos";

        internal AppCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var val = tvOSAppCenter.LogLevel();
                switch (val)
                {
                    case tvOSLogLevel.Verbose:
                        return LogLevel.Verbose;
                    case tvOSLogLevel.Debug:
                        return LogLevel.Debug;
                    case tvOSLogLevel.Info:
                        return LogLevel.Info;
                    case tvOSLogLevel.Warning:
                        return LogLevel.Warn;
                    case tvOSLogLevel.Error:
                        return LogLevel.Error;
                    case tvOSLogLevel.Assert:
                        return LogLevel.Assert;
                    case tvOSLogLevel.None:
                        return LogLevel.None;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(val), val, null);
                }
            }
            set
            {
                tvOSLogLevel loglevel;
                switch (value)
                {
                    case LogLevel.Verbose:
                        loglevel = tvOSLogLevel.Verbose;
                        break;
                    case LogLevel.Debug:
                        loglevel = tvOSLogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        loglevel = tvOSLogLevel.Info;
                        break;
                    case LogLevel.Warn:
                        loglevel = tvOSLogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        loglevel = tvOSLogLevel.Error;
                        break;
                    case LogLevel.Assert:
                        loglevel = tvOSLogLevel.Assert;
                        break;
                    case LogLevel.None:
                        loglevel = tvOSLogLevel.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                tvOSAppCenter.SetLogLevel(loglevel);
            }
        }

        static void PlatformSetUserId(string userId)
        {
            tvOSAppCenter.SetUserId(userId);
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            tvOSAppCenter.SetLogUrl(logUrl);
        }

        static bool PlatformConfigured
        {
            get
            {
                return tvOSAppCenter.IsConfigured();
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            SetWrapperSdk();
            tvOSAppCenter.ConfigureWithAppSecret(appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            SetWrapperSdk();
            foreach (var service in GetServices(services))
            {
                tvOSAppCenter.StartService(service);
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
            tvOSAppCenter.Start(parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(tvOSAppCenter.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            tvOSAppCenter.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            Guid? installId = Guid.Parse(tvOSAppCenter.InstallId().AsString());
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
            tvOSWrapperSdk wrapperSdk = new tvOSWrapperSdk(WrapperSdk.Version, WrapperSdk.Name, Constants.Version, null, null, null);
            tvOSAppCenter.SetWrapperSdk(wrapperSdk);
        }

        static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            tvOSAppCenter.SetCustomProperties(customProperties?.TVOSCustomProperties);
        }
    }
}
