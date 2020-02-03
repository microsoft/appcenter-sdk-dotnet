// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Distribute;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public enum UpdateTrackTime
    {
        Now,
        BeforeNextStart
    };

    public class TrackUpdateUtils
    {
        public const UpdateTrackTime DefaultTimeUpdateTrack = UpdateTrackTime.Now;

        public const UpdateTrack DefaultUpdateTrackType = UpdateTrack.Public;

        public static UpdateTrackTime GetPersistedUpdateTrackTime()
        {
            if (Application.Current.Properties.TryGetValue(Constants.UpdateTrackTimeKey, out object persistedObject))
            {
                string persistedString = (string)persistedObject;
                if (Enum.TryParse<UpdateTrackTime>(persistedString, out var persistedEnum))
                {
                    return persistedEnum;
                }
            }
            return DefaultTimeUpdateTrack;
        }

        public static UpdateTrack GetPersistedUpdateTrack()
        {
            if (Application.Current.Properties.TryGetValue(Constants.UpdateTrackKey, out object persistedObject))
            {
                string persistedString = (string)persistedObject;
                if (Enum.TryParse<UpdateTrack>(persistedString, out var persistedEnum))
                {
                    return persistedEnum;
                }
            }
            return DefaultUpdateTrackType;
        }

        public static async System.Threading.Tasks.Task SetPersistedUpdateTrackAsync(UpdateTrack choice)
        {
            Application.Current.Properties[Constants.UpdateTrackKey] = choice.ToString();
            await Application.Current.SavePropertiesAsync();
        }

        public static async System.Threading.Tasks.Task SetPersistedUpdateTrackTimeAsync(UpdateTrackTime choice)
        {
            Application.Current.Properties[Constants.UpdateTrackTimeKey] = choice.ToString();
            await Application.Current.SavePropertiesAsync();
        }

        public static IEnumerable<string> GetUpdateTrackTimeChoiceStrings()
        {
            foreach (var updateTrackTimeObject in Enum.GetValues(typeof(UpdateTrackTime)))
            {
                yield return updateTrackTimeObject.ToString();
            }
        }

        public static IEnumerable<string> GetUpdateTrackChoiceStrings()
        {
            foreach (var updateTrackObject in Enum.GetValues(typeof(UpdateTrack)))
            {
                yield return updateTrackObject.ToString();
            }
        }

        public static int ToPickerUpdateTrackIndex(UpdateTrack updateTrack)
        {
            return (int)(updateTrack - 1);
        }

        public static UpdateTrack FromPickerUpdateTrackIndex(int index)
        {
            return (UpdateTrack)(index + 1);
        }
    }
}
