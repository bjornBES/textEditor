
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
    public string name { get; set; }
    public string type { get; set; }
    public Dictionary<string, ElementColor> colors { get; set; }
    public bool? semanticHighlighting { get; set; }

    // TODO make a type here
    public object[] tokenColors { get; set; }
}
