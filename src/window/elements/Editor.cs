
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;

public class Editor : Panel
{
    private TabControl tabControl;
    private List<EditorTab> openTabs;
    string defaultBackgroundColor = "#1e1e1e";

    public Editor()
    {
        Background = new SolidColorBrush(Color.Parse("#333333"));

        KeyDown += OnKeyDown;

        openTabs = new List<EditorTab>();

        tabControl = new TabControl
        {
            Background = new SolidColorBrush(Color.Parse("#252526")),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };

        Children.Add(tabControl);
    }

    void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.S && e.KeyModifiers == KeyModifiers.Control)
        {
            SaveCurrent();
        }
    }

    public void OpenFile(string filePath)
    {
        // Avoid opening same file twice
        var existing = openTabs.FirstOrDefault(t => t.FilePath == filePath);
        if (existing != null)
        {
            tabControl.SelectedItem = existing.TabItem;
            return;
        }

        var tab = new EditorTab(filePath);
        tab.BackgroundColor = defaultBackgroundColor;
        openTabs.Add(tab);

        List<TabItem> tabs = openTabs.Select(t => t.TabItem).ToList();
        tabControl.Items.Clear();
        foreach (var item in tabs)
        {
            tabControl.Items.Add(item);
        }

        tabControl.SelectedItem = tab.TabItem;
    }

    public void SaveCurrent()
    {
        var current = GetCurrentTab();
        if (current != null)
        {
            current.Save();
        }
    }

    public void CloseCurrent()
    {
        var current = GetCurrentTab();
        if (current != null)
        {
            openTabs.Remove(current);
            List<TabItem> tabs = openTabs.Select(t => t.TabItem).ToList();
            tabControl.Items.Clear();
            foreach (var item in tabs)
            {
                tabControl.Items.Add(item);
            }
        }
    }

    private EditorTab GetCurrentTab()
    {
        var tabItem = tabControl.SelectedItem as TabItem;
        return openTabs.FirstOrDefault(t => t.TabItem == tabItem);
    }

    public void ChangeDefaultBackgroundColor(string color)
    {
        defaultBackgroundColor = color;
        foreach (TextEditor editor in tabControl.Items)
        {
            editor.Background = new SolidColorBrush(Color.Parse(color));
        }
    }
}
