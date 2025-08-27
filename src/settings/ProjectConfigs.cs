
global using static Program;
using System.Text.Json;

public static class ProjectConfigs
{
    public static ProjectConfigData ProjectConfig;

    public static string ProjectConfigPath;
    public static string WorkspacePath;
    public static string ThemesPath;
    public static string GlobalConfigPath;
    public static string ExtensionsPath;
    public static string EditorPreferencePath;  

    public static void InitializeConfig()
    {
        ProjectConfig = new ProjectConfigData();

        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName);
        DirectoryExists(appDataPath);
        ThemesPath = Path.Combine(appDataPath, "themes");
        DirectoryExists(ThemesPath);
        ExtensionsPath = Path.Combine(appDataPath, "extensions");
        DirectoryExists(ExtensionsPath);
        GlobalConfigPath = Path.Combine(appDataPath, "settings.json");
        EditorPreferencePath = Path.Combine(appDataPath, "editorPreference.json");
        
    }

    static void DirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// A workspace is the project in this case
    /// </summary>
    /// <param name="path"></param>
    public static void SetWorkspace(string path)
    {
        WorkspacePath = path;
        ProjectConfigPath = Path.Combine(WorkspacePath, ".editor");
        DirectoryExists(ProjectConfigPath);
        ProjectConfigPath = Path.Combine(ProjectConfigPath, "settings.json");
    }

    public static void ReadConfigFile(ref ProjectConfigData config, bool GetUsers)
    {
        string path = "";

        if (GetUsers)
        {
            path = ProjectConfigPath;
        }
        else
        {
            path = GlobalConfigPath;
        }

        string json = File.ReadAllText(path);
        config = JsonSerializer.Deserialize<ProjectConfigData>(json);
    }
    public static void SaveTheme(ThemeFile theme)
    {
        string path = Path.Combine(ThemesPath, theme.name + ".json");
        string json = JsonSerializer.Serialize(theme, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}

public class ProjectConfigData
{
    public WorkspaceSettings WorkspaceSettings { get; set; }
    public EditorSettings EditorSettings{ get; set; }

    public Dictionary<string, ExtensionManifest> Extensions { get; set; } = new Dictionary<string, ExtensionManifest>();
}

