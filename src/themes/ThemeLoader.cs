using Avalonia;
using Avalonia.Media;

public static class ThemeLoader
{
    public static SolidColorBrush ToBrush(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return null;
        if (!hex.StartsWith("#")) hex = "#" + hex;
        return SolidColorBrush.Parse(hex);
    }
    public static void ApplyTheme(ThemeFile theme)
    {
        // Flatten JSON colors into resources
        AddElementColors(theme.Colors, "");
    }

    private static void AddElementColors(Dictionary<string, ElementColor> colors, string prefix)
    {
        foreach (var kvp in colors)
        {
            string key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";
            var el = kvp.Value;

            if (el.background != null)
                Application.Current.Resources[$"{key}.Background"] = ToBrush(el.background);

            if (el.foreground != null)
                Application.Current.Resources[$"{key}.Foreground"] = ToBrush(el.foreground);

            if (el.subElements != null)
                AddElementColors(el.subElements, key);
        }
    }
}
