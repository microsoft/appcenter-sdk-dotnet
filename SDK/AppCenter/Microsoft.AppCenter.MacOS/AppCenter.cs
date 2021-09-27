using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;

namespace Microsoft.AppCenter
{
    using MacOSLogLevel = Microsoft.AppCenter.MacOS.Bindings.MSACLogLevel;
    using MacOSAppCenter = Microsoft.AppCenter.MacOS.Bindings.MSACAppCenter;
    using MacOSWrapperSdk = Microsoft.AppCenter.MacOS.Bindings.MSACWrapperSdk;

    public partial class AppCenter
    {
        /* The key identifier for parsing app secrets */
        const string PlatformIdentifier = "macos";

        internal AppCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var val = MacOSAppCenter.LogLevel();
                switch (val)
                {
                    case MacOSLogLevel.Verbose:
                        return LogLevel.Verbose;
                    case MacOSLogLevel.Debug:
                        return LogLevel.Debug;
                    case MacOSLogLevel.Info:
                        return LogLevel.Info;
                    case MacOSLogLevel.Warning:
                        return LogLevel.Warn;
                    case MacOSLogLevel.Error:
                        return LogLevel.Error;
                    case MacOSLogLevel.Assert:
                        return LogLevel.Assert;
                    case MacOSLogLevel.None:
                        return LogLevel.None;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(val), val, null);
                }
            }
            set
            {
                MacOSLogLevel loglevel;
                switch (value)
                {
                    case LogLevel.Verbose:
                        loglevel = MacOSLogLevel.Verbose;
                        break;
                    case LogLevel.Debug:
                        loglevel = MacOSLogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        loglevel = MacOSLogLevel.Info;
                        break;
                    case LogLevel.Warn:
                        loglevel = MacOSLogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        loglevel = MacOSLogLevel.Error;
                        break;
                    case LogLevel.Assert:
                        loglevel = MacOSLogLevel.Assert;
                        break;
                    case LogLevel.None:
                        loglevel = MacOSLogLevel.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                MacOSAppCenter.SetLogLevel(loglevel);
            }
        }

        static bool PlatformIsNetworkRequestsAllowed
        {
            get => MacOSAppCenter.IsNetworkRequestsAllowed();
            set => MacOSAppCenter.SetNetworkRequestsAllowed(value);
        }

        static void PlatformSetUserId(string userId)
        {
            MacOSAppCenter.SetUserId(userId);
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            MacOSAppCenter.SetLogUrl(logUrl);
        }

        static bool PlatformConfigured
        {
            get
            {
                return MacOSAppCenter.IsConfigured();
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            SetWrapperSdk();
            MacOSAppCenter.ConfigureWithAppSecret(appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            SetWrapperSdk();
            foreach (var service in GetServices(services))
            {
                MacOSAppCenter.StartService(service);
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
            MacOSAppCenter.Start(parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MacOSAppCenter.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MacOSAppCenter.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            Guid? installId = Guid.Parse(MacOSAppCenter.InstallId().AsString());
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
            MacOSWrapperSdk wrapperSdk = new MacOSWrapperSdk(WrapperSdk.Version, WrapperSdk.Name, Constants.Version, null, null, null);
            MacOSAppCenter.SetWrapperSdk(wrapperSdk);
        }

        static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            MacOSAppCenter.SetCustomProperties(customProperties?.MacOSCustomProperties);
        }

        internal static void PlatformUnsetInstance()
        {
            MacOSAppCenter.ResetSharedInstance();
        }

        static Task<bool> PlatformSetMaxStorageSizeAsync(long sizeInBytes)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            MacOSAppCenter.SetMaxStorageSize(sizeInBytes, (result) => taskCompletionSource.SetResult(result));
            return taskCompletionSource.Task;
        }
    }
}