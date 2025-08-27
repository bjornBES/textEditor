using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaEdit;

public class EditorTab
{
    public string FilePath { get; private set; }
    public TextEditor TextEditor { get; private set; }
    public TabItem TabItem { get; private set; }
    public bool IsDirty { get; private set; }
    public string BackgroundColor { get; set; }

    public EditorTab(string filePath)
    {
        FilePath = filePath;
        TextEditor = new TextEditor
        {
            ShowLineNumbers = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        if (File.Exists(filePath))
        {
            TextEditor.Text = File.ReadAllText(filePath);
        }

        TextEditor.TextChanged += (s, e) =>
        {
            if (!IsDirty)
            {
                IsDirty = true;
                UpdateTabHeader();
            }
        };

        TabItem = new TabItem
        {
            Header = Path.GetFileName(filePath),
            Content = TextEditor
        };
    }

    public void Save()
    {
        File.WriteAllText(FilePath, TextEditor.Text);
        IsDirty = false;
        UpdateTabHeader();
    }

    private void UpdateTabHeader()
    {
        TabItem.Header = Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
    }

    public void ChangeBackgroundColor(string newBackgroundColor)
    {
        BackgroundColor = newBackgroundColor;
        TextEditor.Background = new SolidColorBrush(Color.Parse(newBackgroundColor));
    }
}