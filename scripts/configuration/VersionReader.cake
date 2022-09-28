// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

public class VersionReader
{
    private const string SdkVersionTag = "sdkVersion";
    private const string AppleVersionTag = "appleVersion";
    private const string AndroidVersionTag = "androidVersion";

    public static string SdkVersion { get; private set; }
    public static string AppleVersion { get; private set; }
    public static string AndroidVersion { get; private set; }

    public static void ReadVersions()
    {
        XmlReader reader = ConfigFile.CreateReader();
        string sdkVersion = null;
        string appleVersion = null;
        string androidVersion = null;
        while (reader.Read())
        {
            ReadVersion(SdkVersionTag, reader, ref sdkVersion);
            ReadVersion(AppleVersionTag, reader, ref appleVersion);
            ReadVersion(AndroidVersionTag, reader, ref androidVersion);
        }
        SdkVersion = sdkVersion;
        AppleVersion = appleVersion;
        AndroidVersion = androidVersion;
        reader.Close();
    }

    private static void ReadVersion(string versionName, XmlReader reader, ref string versionVar)
    {
        if (reader.Name == versionName)
        {
            var versionNode = new XmlDocument();
            var node = versionNode.ReadNode(reader);
            versionVar = node.FirstChild.Value;
            Statics.Context.Information($"Found {versionName} = {versionVar}");
        }
    }

    public static void WriteAppleVersion(string value)
    {
        Statics.Context.Information($"Replacing build config versions: {AppleVersionTag} from {AppleVersion} to {value}.");
        Statics.Context.ReplaceTextInFiles(ConfigFile.Path, $"<{AppleVersionTag}>{AppleVersion}</{AppleVersionTag}>", $"<{AppleVersionTag}>{value}</{AppleVersionTag}>");
    }

    public static void WriteAndroidVersion(string value)
    {
        Statics.Context.Information($"Replacing build config versions: {AndroidVersionTag} from {AndroidVersion} to {value}.");
        Statics.Context.ReplaceTextInFiles(ConfigFile.Path, $"<{AndroidVersionTag}>{AndroidVersion}</{AndroidVersionTag}>", $"<{AndroidVersionTag}>{value}</{AndroidVersionTag}>");
    }
}