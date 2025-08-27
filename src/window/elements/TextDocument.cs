
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using Tmds.DBus.Protocol;

public class TextDocument : TextEditor
{
    public string FilePath;
    public string LangageId;
    public bool IsOpen;
    public TextDocument(string filePath, string langageId)
    {
        IsOpen = true;
        FilePath = filePath;
        LangageId = langageId;

        Background = new SolidColorBrush(Color.Parse("#333333"));
        IsVisible = true;

        Name = Path.GetFullPath(filePath);
        ShowLineNumbers = true;
        FontFamily = new FontFamily("Consolas");
        FontSize = 14;
        Background = new SolidColorBrush(Color.Parse("#1e1e1e"));
        Foreground = Brushes.White;
        IsVisible = true;
        Text = "";
        Padding = new Thickness(10);
        SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(langageId);
    }

    public void LoadFile()
    {
        if (!File.Exists(FilePath))
        {
            File.Create(FilePath).Close();
            return;
        }
        Text = File.ReadAllText(FilePath);
    }

    public void SwitchLangageId(string langageId)
    {
        LangageId = langageId;
    }

}