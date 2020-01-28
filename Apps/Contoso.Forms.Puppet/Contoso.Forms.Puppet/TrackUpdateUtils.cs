// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Distribute;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public enum TimeUpdateTrack
    {
        Now,
        AfterRestart
    };

    public class TrackUpdateUtils
    {
        public const TimeUpdateTrack DefaultTimeUpdateTrack = TimeUpdateTrack.Now;
        public const UpdateTrack DefaultUpdateTrackType = UpdateTrack.Public;

        public static TimeUpdateTrack GetPersistedTimeUpdateTrack()
        {
            if (Application.Current.Properties.TryGetValue(Constants.WhenUpdateKey, out object persistedObject))
            {
                string persistedString = (string)persistedObject;
                if (Enum.TryParse<TimeUpdateTrack>(persistedString, out var persistedEnum))
                {
                    return persistedEnum;
                }
            }
            return DefaultTimeUpdateTrack;
        }

        public static UpdateTrack GetPersistedUpdateTrackType()
        {
            if (Application.Current.Properties.TryGetValue(Constants.TrackUpdateKey, out object persistedObject))
            {
                string persistedString = (string)persistedObject;
                if (Enum.TryParse<UpdateTrack>(persistedString, out var persistedEnum))
                {
                    return persistedEnum;
                }
            }
            return DefaultUpdateTrackType;
        }

        public static async System.Threading.Tasks.Task SetPersistedUpdateTrackTypeAsync(UpdateTrack choice)
        {
            Application.Current.Properties[Constants.TrackUpdateKey] = choice.ToString();
            await Application.Current.SavePropertiesAsync();
        }

        public static async System.Threading.Tasks.Task SetPersistedTimeUpdateTrackAsync(TimeUpdateTrack choice)
        {
            Application.Current.Properties[Constants.WhenUpdateKey] = choice.ToString();
            await Application.Current.SavePropertiesAsync();
        }

        public static IEnumerable<string> GetTimeUpdateTrackChoiceStrings()
        {
            foreach (var timeUpdateTrackObject in Enum.GetValues(typeof(TimeUpdateTrack)))
            {
                yield return timeUpdateTrackObject.ToString();
            }
        }

        public static IEnumerable<string> GetUpdateTrackChoiceStrings()
        {
            foreach (var updateTypeTypeObject in Enum.GetValues(typeof(UpdateTrack)))
            {
                yield return updateTypeTypeObject.ToString();
            }
        }

        public static int ToPicketUpdateTrackIndex(UpdateTrack updateTrack)
        {
            return (int)(updateTrack - 1);
        }

        public static UpdateTrack FromPickerUpdateTrackIndex(int index)
        {
            return (UpdateTrack)(index + 1);
        }
    }
}
