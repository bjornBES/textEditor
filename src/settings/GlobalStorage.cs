
using System.Text.Json.Serialization;

public class GlobalStorage
{
    [JsonPropertyName("backupWorkspaces")]
    public BackupWorkspaces BackupWorkspaces { get; set; }
    [JsonPropertyName("theme")]
    public string Theme { get; set; }
    [JsonPropertyName("themeBackground")]
    public string ThemeBackground { get; set; }
}

public class BackupWorkspaces
{
    [JsonPropertyName("workspace")]
    public string[] Workspace { get; set; } = Array.Empty<string>();
    [JsonPropertyName("folders")]
    public string[] Folders { get; set; } = Array.Empty<string>();
    [JsonPropertyName("emptyWindows")]
    public string[] EmptyWindows { get; set; } = Array.Empty<string>();
}