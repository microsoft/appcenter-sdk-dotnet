using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjCRuntime;

namespace Microsoft.AppCenter
{
    using macLogLevel = Microsoft.AppCenter.macOS.Bindings.MSLogLevel;
    using macAppCenter = Microsoft.AppCenter.macOS.Bindings.MSAppCenter;
    using macWrapperSdk = Microsoft.AppCenter.macOS.Bindings.MSWrapperSdk;

    public partial class AppCenter
    {
        /* The key identifier for parsing app secrets */
        const string PlatformIdentifier = "mac";

        internal AppCenter()
        {
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                var val = macAppCenter.LogLevel();
                switch (val)
                {
                    case macLogLevel.Verbose:
                        return LogLevel.Verbose;
                    case macLogLevel.Debug:
                        return LogLevel.Debug;
                    case macLogLevel.Info:
                        return LogLevel.Info;
                    case macLogLevel.Warning:
                        return LogLevel.Warn;
                    case macLogLevel.Error:
                        return LogLevel.Error;
                    case macLogLevel.Assert:
                        return LogLevel.Assert;
                    case macLogLevel.None:
                        return LogLevel.None;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(val), val, null);
                }
            }
            set
            {
                macLogLevel loglevel;
                switch (value)
                {
                    case LogLevel.Verbose:
                        loglevel = macLogLevel.Verbose;
                        break;
                    case LogLevel.Debug:
                        loglevel = macLogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        loglevel = macLogLevel.Info;
                        break;
                    case LogLevel.Warn:
                        loglevel = macLogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        loglevel = macLogLevel.Error;
                        break;
                    case LogLevel.Assert:
                        loglevel = macLogLevel.Assert;
                        break;
                    case LogLevel.None:
                        loglevel = macLogLevel.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                macAppCenter.SetLogLevel(loglevel);
            }
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            macAppCenter.SetLogUrl(logUrl);
        }

        static bool PlatformConfigured
        {
            get
            {
                return macAppCenter.IsConfigured();
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            SetWrapperSdk();
            macAppCenter.ConfigureWithAppSecret(appSecret);
        }

        static void PlatformStart(params Type[] services)
        {
            SetWrapperSdk();
            foreach (var service in GetServices(services))
            {
                macAppCenter.StartService(service);
            }
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            SetWrapperSdk();
            string parsedSecret;
            try
            {
                parsedSecret = GetSecretForPlatform(appSecret, PlatformIdentifier);
            }
            catch (AppCenterException ex)
            {
                AppCenterLog.Assert(AppCenterLog.LogTag, ex.Message);
                return;
            }
            macAppCenter.Start(parsedSecret, GetServices(services));
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(macAppCenter.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            macAppCenter.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            Guid? installId = Guid.Parse(macAppCenter.InstallId().AsString());
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
            macWrapperSdk wrapperSdk = new macWrapperSdk(WrapperSdk.Version, WrapperSdk.Name, Constants.Version, null, null, null);
            macAppCenter.SetWrapperSdk(wrapperSdk);
        }

        static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            macAppCenter.SetCustomProperties(customProperties?.MacCustomProperties);
        }
    }
}
