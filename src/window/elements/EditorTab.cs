using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;

public class EditorTab
{
    public string FilePath { get; private set; }
    public TextEditor TextEditor { get; private set; }
    public TabItem TabItem { get; private set; }
    public bool IsDirty { get; private set; }

    private StackPanel tabPanel;
    Button closeButton;
    TextBlock fileText;
    Highligher highligher;
#nullable enable
    public Action<EditorTab>? CloseTab;
#nullable disable

    public EditorTab(string filePath)
    {
        FilePath = filePath;
        highligher = new Highligher();
        highligher.Initialize(filePath);

        makeEmpty(filePath);
    }
    public EditorTab()
    {
        makeEmpty("");
    }

    void makeEmpty(string filePath)
    {
        TextEditor = new TextEditor
        {
            ShowLineNumbers = true,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = (IBrush)Application.Current.Resources["editor.texteditor.Background"],
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

        tabPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
        };

        closeButton = new Button()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Content = "C",
        };
        closeButton.Click += (_, __) => CloseTab?.Invoke(this);

        tabPanel.Children.Add(closeButton);

        string fileName = Path.GetFileName(FilePath);
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "Untitled";
        }
        fileText = new TextBlock()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Text = fileName,
        };

        tabPanel.Children.Add(fileText);

        TabItem = new TabItem
        {
            Header = tabPanel,
            Content = TextEditor
        };
    }

    public void Save()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            string path = (string)Commands.RunCommand("file.save.file.dialog");
            FilePath = path;
        }
        File.WriteAllText(FilePath, TextEditor.Text);
        IsDirty = false;
        UpdateTabHeader();
        Commands.RunCommand("update.file.tree");
    }

    public void SaveAs()
    {
        string path = (string)Commands.RunCommand("file.save.file.dialog");
        if (string.IsNullOrEmpty(path))
        {
            Save();
            return;
        }
        FilePath = path;
        File.WriteAllText(FilePath, TextEditor.Text);
        IsDirty = false;
        UpdateTabHeader();
        Commands.RunCommand("update.file.tree");
    }

    public void DontSave()
    {
        IsDirty = false;
        UpdateTabHeader();
    }

    private void UpdateTabHeader()
    {
        if (Path.Exists(FilePath))
        {
            TabItem.Header = Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
        }
        else
        {
            TabItem.Header = "Untitled" + (IsDirty ? "*" : "");
        }
    }

    public void UpdateTheme()
    {
        TextEditor.Background = (IBrush)Application.Current.Resources["editor.texteditor.Background"];
    }
}