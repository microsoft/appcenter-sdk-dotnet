// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Xml;
using System.Collections.Generic;

public static class ConfigFile
{
    public const string Name = "ac-build-config.xml";
    public const string Path = "scripts/configuration/" + Name;

    public static XmlReader CreateReader()
    {
        return XmlReader.Create(Path);
    }
}

#load "AppCenterModule.cake"
#load "AssemblyGroup.cake"
#load "BuildGroup.cake"
#load "VersionReader.cake"
