// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// A Build Group contains information on what solutions to build for which platform,
// and how to do so.
using Cake.Common.Tools.MSBuild;

public class BuildGroup
{
    private string _solutionPath;
    private IList<Builder> _builders = new List<Builder>();

    private abstract class Builder
    {
        public string Configuration { get; set; }

        public abstract void Build(string solutionPath);
    }

    private class MSBuilder : Builder
    {
        public string ToolVersion { get; set; }

        public override void Build(string solutionPath)
        {
            Statics.Context.MSBuild(solutionPath, settings => {
                if (ToolVersion != null)
                {
                    Enum.TryParse(ToolVersion, out MSBuildToolVersion msBuildToolVersion);
                    settings.ToolVersion = msBuildToolVersion;
                }
                settings.Configuration = Configuration;
            });
        }
    }

    private class DotNetBuilder : Builder
    {
        public override void Build(string solutionPath)
        {
            var settings = new DotNetCoreBuildSettings
            {
                Configuration = Configuration,
            };
            Statics.Context.DotNetBuild(solutionPath, settings);
        }
    }

    public static IList<BuildGroup> ReadBuildGroups(string platformId)
    {
        XmlReader reader = ConfigFile.CreateReader();
        IList<BuildGroup> groups = new List<BuildGroup>();
        while (reader.Read())
        {
            if (reader.Name == "buildGroup")
            {
                XmlDocument buildGroup = new XmlDocument();
                var node = buildGroup.ReadNode(reader);
                if (node.Attributes.GetNamedItem("platformId").Value != platformId)
                {
                    continue;
                }
                groups.Add(new BuildGroup(node));
            }
        }
        return groups;
    }

    public BuildGroup(XmlNode node)
    {
        _solutionPath = node.Attributes.GetNamedItem("solutionPath").Value;
        for (int i = 0; i < node.ChildNodes.Count; ++i)
        {
            var childNode = node.ChildNodes.Item(i);
            var builder = CreateBuilder(childNode);
            if (builder != null)
            {
                _builders.Add(builder);
            }
        }
    }

    public void ExecuteBuilds()
    {
        if (_solutionPath.EndsWith(".slnf"))
        {
            Statics.Context.DotNetRestore(_solutionPath);
        }
        else
        {
            Statics.Context.NuGetRestore(_solutionPath);
        }
        foreach (var builders in _builders)
        {
            builders.Build(_solutionPath);
        }
    }

    private Builder CreateBuilder(XmlNode node)
    {
        if (node.Name == "msBuild")
        {
            var configuration = node.Attributes.GetNamedItem("configuration")?.Value;
            var toolVersion = node.Attributes.GetNamedItem("toolVersion")?.Value;
            return new MSBuilder {
                Configuration = configuration,
                ToolVersion = toolVersion,
            };
        }
        else if (node.Name == "dotNetBuild")
        {
            var configuration = node.Attributes.GetNamedItem("configuration")?.Value;
            return new DotNetBuilder {
                Configuration = configuration,
            };
        }
        return null;
    }
}
