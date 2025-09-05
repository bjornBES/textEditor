
using AvaloniaEdit;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

public enum HighligherType
{
    Textmate,

}

public class Highligher
{
    IRegistryOptions options;
    Registry registry;
    IGrammar grammar;

    public string Language { get; private set; }
    public HighligherType HighligherType { get; private set; }

    public void Initialize(string filePath)
    {
        options = new LocalRegistryOptions();
        registry = new Registry(options);

        string[] fileContentLines = File.ReadAllLines(filePath);
        string ext = Path.GetExtension(filePath);
        grammar = registry.LoadGrammar($"source.{ext}");
        
    }

    public void ApplyHighlighting(TextEditor editor, string filePath)
    {
    }
}

class LocalRegistryOptions : IRegistryOptions
{
    public ICollection<string> GetInjections(string scopeName)
    {
        return null;
    }

    public IRawGrammar GetGrammar(string scopeName)
    {
        return null;
    }

    public IRawTheme GetTheme(string scopeName)
    {
        return null;
    }

    public IRawTheme GetDefaultTheme()
    {
        string themeName = ProjectConfigs.CurrentTheme.Name;
        string themePath = Path.GetFullPath(Path.Combine(ProjectConfigs.ThemesPath, $"{themeName}.json"));

        using (StreamReader reader = new StreamReader(themePath))
        {
            return ThemeReader.ReadThemeSync(reader);
        }
    }
}
