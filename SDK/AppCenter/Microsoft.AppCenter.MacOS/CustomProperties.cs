using System;
using Foundation;
using Microsoft.AppCenter.MacOS.Bindings;

namespace Microsoft.AppCenter
{
    public partial class CustomProperties
    {
        static readonly DateTime _epoch = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal MSACCustomProperties MacOSCustomProperties { get; } = new MSACCustomProperties();

        CustomProperties PlatformSet(string key, string value)
        {
            MacOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, DateTime value)
        {
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate((value.ToUniversalTime() - _epoch).TotalSeconds);
            MacOSCustomProperties.Set(nsDate, key);
            return this;
        }

        CustomProperties PlatformSet(string key, int value)
        {
            MacOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, long value)
        {
            MacOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, float value)
        {
            MacOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, double value)
        {
            MacOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, decimal value)
        {
            MacOSCustomProperties.Set((double)value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, bool value)
        {
            MacOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformClear(string key)
        {
            MacOSCustomProperties.Clear(key);
            return this;
        }
    }
}