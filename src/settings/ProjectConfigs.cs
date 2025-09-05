
global using static Program;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

public static class ProjectConfigs
{
    public static ProjectConfigData ProjectConfig;
    private static Dictionary<string, object> JsonCache = new Dictionary<string, object>();

    public static string WorkspacePath;
    public static string ThemesPath;
    public static string ExtensionsPath;
    public static string SnippetsPath;
    public static string WorkspaceStoragePath;
    public static string GlobalStoragePath;

    public static string EditorPreferenceFilePath;
    public static string RecentOpenPathsFilePath;
    /// <summary>
    /// project-specific settings
    /// </summary>
    public static string ProjectConfigFilePath;
    /// <summary>
    /// global settings
    /// </summary>
    public static string GlobalSettingsFilePath;
    public static string KeybindingsFilePath;
    public static string GlobalStorageFilePath;

    public static ThemeFile CurrentTheme;


    public static void InitializeConfig()
    {
        ProjectConfig = new ProjectConfigData();

        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName);
        DirectoryExists(appDataPath);
        string userProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "." + ProjectName);
        DirectoryExists(userProfilePath);

        string appDataUserPath = Path.Combine(appDataPath, "User");

        ThemesPath = Path.Combine(userProfilePath, "themes");
        DirectoryExists(ThemesPath);
        ExtensionsPath = Path.Combine(userProfilePath, "extensions");
        DirectoryExists(ExtensionsPath);
        SnippetsPath = Path.Combine(appDataPath, "snippets");
        DirectoryExists(SnippetsPath);
        WorkspaceStoragePath = Path.Combine(appDataUserPath, "workspaceStorage");
        DirectoryExists(WorkspaceStoragePath);
        GlobalStoragePath = Path.Combine(appDataUserPath, "globalStorage");
        DirectoryExists(GlobalStoragePath);

        GlobalSettingsFilePath = FileExists(appDataPath, "settings.json");    // settings are in ~/.config/App/settings.json
        EditorPreferenceFilePath = FileExists(appDataPath, "editorPreference.json");
        KeybindingsFilePath = FileExists(appDataPath, "keybindings.json");
        GlobalStorageFilePath = FileExists(GlobalStoragePath, "storage.json");
    }

    static string FileExists(string path1, string path2)
    {
        DirectoryExists(path1);
        string result = Path.Combine(path1, path2);
        if (!File.Exists(result))
        {
            File.Create(result).Close();
        }
        return result;
    }

    static void DirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static T ReadFile<T>(string path)
    {
        if (JsonCache.ContainsKey(path))
        {
            return (T)JsonCache[path];
        }

        if (!File.Exists(path)) return default;

        string json = File.ReadAllText(path);
        if (string.IsNullOrWhiteSpace(json)) return default;
        var obj = JsonSerializer.Deserialize<T>(json);
        JsonCache.Add(path, obj);
        return obj;
    }

    public static void WriteFile<T>(string path, T obj)
    {
        if (JsonCache.ContainsKey(path))
        {
            JsonCache.Remove(path);
        }

        if (!File.Exists(path)) return;

        string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(path, json);
        JsonCache.Add(path, obj);
    }

    /// <summary>
    /// A workspace is the project in this case
    /// </summary>
    /// <param name="path"></param>
    public static void SetWorkspace(string path)
    {
        WorkspacePath = path;
        ProjectConfigFilePath = Path.Combine(WorkspacePath, ".editor");
        DirectoryExists(ProjectConfigFilePath);
        ProjectConfigFilePath = FileExists(ProjectConfigFilePath, "settings.json");
    }

    public static void ReadConfigFile(ref ProjectConfigData config)
    {
        string path = "";



        string json = File.ReadAllText(path);
        config = JsonSerializer.Deserialize<ProjectConfigData>(json);
    }
    public static void SaveTheme(ThemeFile theme)
    {
        string path = Path.Combine(ThemesPath, theme.Name + ".json");
        string json = JsonSerializer.Serialize(theme, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}

public class ProjectConfigData
{
    public WorkspaceSettings WorkspaceSettings { get; set; }
    public EditorSettings EditorSettings { get; set; }

    public Dictionary<string, ExtensionManifest> Extensions { get; set; } = new Dictionary<string, ExtensionManifest>();
}

public class LocalProjectConfigData
{
    public WorkspaceSettings WorkspaceSettings { get; set; }
    public EditorSettings EditorSettings { get; set; }


}

