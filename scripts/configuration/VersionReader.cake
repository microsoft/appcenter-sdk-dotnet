public class VersionReader
{
    private const string SdkVersionTag = "sdkVersion";
    private const string IosVersionTag = "iosVersion";
    private const string AndroidVersionTag = "androidVersion";

    public static string SdkVersion { get; private set; }
    public static string IosVersion { get; private set; }
    public static string AndroidVersion { get; private set; }

    public static void ReadVersions()
    {
        XmlReader reader = ConfigFile.CreateReader();
        string sdkVersion = null;
        string iosVersion = null;
        string androidVersion = null;
        while (reader.Read())
        {
            ReadVersion(SdkVersionTag, reader, ref sdkVersion);
            ReadVersion(IosVersionTag, reader, ref iosVersion);
            ReadVersion(AndroidVersionTag, reader, ref androidVersion);
        }
        SdkVersion = sdkVersion;
        IosVersion = iosVersion;
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

    public static void WriteIosVersion(string value)
    {
        Statics.Context.Information($"Replacing build config versions: {IosVersionTag} from {IosVersion} to {value}.");
        Statics.Context.ReplaceTextInFiles(ConfigFile.Path, $"<{IosVersionTag}>{IosVersion}</{IosVersionTag}>", $"<{IosVersionTag}>{value}</{IosVersionTag}>");
    }

    public static void WriteAndroidVersion(string value)
    {
        Statics.Context.Information($"Replacing build config versions: {AndroidVersionTag} from {AndroidVersion} to {value}.");
        Statics.Context.ReplaceTextInFiles(ConfigFile.Path, $"<{AndroidVersionTag}>{AndroidVersion}</{AndroidVersionTag}>", $"<{AndroidVersionTag}>{value}</{AndroidVersionTag}>");
    }
}