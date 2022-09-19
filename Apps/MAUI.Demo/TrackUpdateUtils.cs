// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Distribute;

namespace MAUI.Demo;

public class TrackUpdateUtils
{
    public const UpdateTrack DefaultUpdateTrackType = UpdateTrack.Public;

    public static UpdateTrack? GetPersistedUpdateTrack()
    {
        string persistedString = Preferences.Get(Constants.UpdateTrackKey, null);
        if (Enum.TryParse<UpdateTrack>(persistedString, out var persistedEnum))
        {
            return persistedEnum;
        }
        return null;
    }

    public static void SetPersistedUpdateTrack(UpdateTrack choice)
    {
        Preferences.Set(Constants.UpdateTrackKey, choice.ToString());
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
