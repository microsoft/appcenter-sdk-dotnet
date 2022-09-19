// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace MAUI.Demo;

public enum StartType
{
    AppCenter,
    OneCollector,
    Both
};

public class StartTypeUtils
{
    public const StartType DefaultStartType = StartType.AppCenter;
    private const string StartTypeSettingKey = "startType";

    public static StartType GetPersistedStartType()
    {
        var startType = Preferences.Get(StartTypeSettingKey, null);
        if (Enum.TryParse<StartType>(startType, out var persistedStartTypeEnum))
        {
            return persistedStartTypeEnum;
        }
        return DefaultStartType;
    }

    public static void SetPersistedStartType(StartType choice)
    {
        Preferences.Set(StartTypeSettingKey, choice.ToString());
    }

    public static IEnumerable<string> GetStartTypeChoiceStrings()
    {
        foreach (var startTypeObject in Enum.GetValues(typeof(StartType)))
        {
            yield return startTypeObject.ToString();
        }
    }
}
