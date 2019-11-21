// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Foundation;
using Microsoft.AppCenter.tvOS.Bindings;

namespace Microsoft.AppCenter
{
    public partial class CustomProperties
    {
        static readonly DateTime _epoch = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal MSCustomProperties TVOSCustomProperties { get; } = new MSCustomProperties();

        CustomProperties PlatformSet(string key, string value)
        {
            TVOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, DateTime value)
        {
            var nsDate = NSDate.FromTimeIntervalSinceReferenceDate((value.ToUniversalTime() - _epoch).TotalSeconds);
            TVOSCustomProperties.Set(nsDate, key);
            return this;
        }

        CustomProperties PlatformSet(string key, int value)
        {
            TVOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, long value)
        {
            TVOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, float value)
        {
            TVOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, double value)
        {
            TVOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, decimal value)
        {
            TVOSCustomProperties.Set((double)value, key);
            return this;
        }

        CustomProperties PlatformSet(string key, bool value)
        {
            TVOSCustomProperties.Set(value, key);
            return this;
        }

        CustomProperties PlatformClear(string key)
        {
            TVOSCustomProperties.Clear(key);
            return this;
        }
    }
}
