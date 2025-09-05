
using System.Text.Json.Serialization;

public class ExtensionManifest
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }
    [JsonPropertyName("description")]
#nullable enable
    public string? Description { get; set; }
#nullable disable
    [JsonPropertyName("version")]
    public string Version { get; set; }
    [JsonPropertyName("order")]
    public int? Order { get; set; }
    [JsonPropertyName("main")]
    public string Main { get; set; }

    [JsonPropertyName("contributes")]
    public Contributes Contributes { get; set; }
}

#nullable enable
public class Contributes
{
    [JsonPropertyName("grammar")]
    public ExtGrammar? Grammar { get; set; }
    [JsonPropertyName("language")]
    public ExtLanguage? Language { get; set; }
    [JsonPropertyName("theme")]
    public ExtTheme? Theme { get; set; }
}
#nullable disable

public class ExtLanguage
{
    [JsonPropertyName("languageId")]
    public string LanguageId { get; set; }
    [JsonPropertyName("extensions")]
    public List<string> Extensions { get; set; }
}

public class ExtGrammar
{
}

public class ExtTheme
{
}