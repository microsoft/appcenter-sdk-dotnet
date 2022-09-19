// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace MAUI.Demo;

public static class EventFilterHolder
{
    public interface IImplementation
    {
        Type BindingType { get; }

        Task<bool> IsEnabledAsync();

        Task SetEnabledAsync(bool enabled);
    }

    public static IImplementation Implementation { get; set; }
}
