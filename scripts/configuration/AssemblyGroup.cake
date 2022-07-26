// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// An assembly group contains information about which assemblies to be packaged together
// for each supported platform
public class AssemblyGroup
{
    public string Id { get; set; }
    public string NuspecKey => $"${Id}_dir$";
    public string Folder => $"bin/{Id}";
    public IList<string> AssemblyPaths { get; set; }
    public bool Download { get; set; }

    public static IList<AssemblyGroup> ReadAssemblyGroups()
    {
        XmlReader reader = ConfigFile.CreateReader();
        IList<AssemblyGroup> groups = new List<AssemblyGroup>();
        while (reader.Read())
        {
            if (reader.Name == "group")
            {
                XmlDocument group = new XmlDocument();
                var node = group.ReadNode(reader);
                groups.Add(new AssemblyGroup(node));
            }
        }
        return groups;
    }

    private AssemblyGroup(XmlNode groupNode)
    {
        AssemblyPaths = new List<string>();
        Id = groupNode.Attributes.GetNamedItem("id").Value;
        var buildGroup = groupNode.Attributes.GetNamedItem("buildGroup")?.Value;
        var platformString = Statics.Context.IsRunningOnUnix() ? "mac" : "windows";
        if (buildGroup != null)
        {
            Download = (buildGroup != platformString);
        }
        for (int i = 0; i < groupNode.ChildNodes.Count; ++i)
        {
            var childNode = groupNode.ChildNodes.Item(i);
            if (childNode.Name == "assembly")
            {
                var assemblyName = childNode.Attributes.GetNamedItem("path").Value;
                AssemblyPaths.Add(assemblyName);
                if (System.IO.Path.GetExtension(assemblyName) == ".dll")
                {
                    var docName = System.IO.Path.ChangeExtension(assemblyName, "xml");
                    AssemblyPaths.Add(docName);
                }
            }
        }
    }
}
