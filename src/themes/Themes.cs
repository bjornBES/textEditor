
using System.Text.Json.Serialization;

public class Themes
{
    public string controlInt;
    public ColorType background;
    public ColorType foreground;
}

#nullable enable
public class ElementColor
{
    public string? foreground { get; set; }
    public string? background { get; set; }
    public Dictionary<string, ElementColor>? subElements { get; set; }
}
#nullable disable
public class ThemeFile
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("colors")]
    public Dictionary<string, ElementColor> Colors { get; set; }
    [JsonPropertyName("semanticHighlighting")]
    public bool? SemanticHighlighting { get; set; }

    // TODO make a type here for tokenColors
    public object[] tokenColors { get; set; }
}
