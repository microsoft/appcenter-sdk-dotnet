using System;
using Foundation;
using Microsoft.AppCenter.macOS.Bindings;

namespace Microsoft.AppCenter
{
    public partial class CustomProperties
    {
        static readonly DateTime _epoch = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal MSCustomProperties MacCustomProperties { get; } = new MSCustomProperties();

        CustomProperties PlatformSet(string key, string value)
        {
            MacCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, DateTime value)
        {
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate((value.ToUniversalTime() - _epoch).TotalSeconds);
            MacCustomProperties.Set(nsDate, key);
            return this;
        }

        CustomProperties PlatformSet(string key, int value)
        {
            MacCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, long value)
        {
            MacCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, float value)
        {
            MacCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, double value)
        {
            MacCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, decimal value)
        {
            MacCustomProperties.Set((double)value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, bool value)
        {
            MacCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformClear(string key)
        {
            MacCustomProperties.Clear(key);
            return this;
        }
    }
}
