
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;

public class Editor : Panel
{
    #nullable enable
    public Action? askSaveFiles;
    #nullable disable
    private TabControl tabControl;
    private List<EditorTab> openTabs;

    public Editor()
    {
        Background = (IBrush)Application.Current.Resources["editor.Background"];

        KeyDown += OnKeyDown;

        openTabs = new List<EditorTab>();

        tabControl = new TabControl
        {
            Background = (IBrush)Application.Current.Resources["editor.tabs.Background"],
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
        tab.CloseTab += CloseTab;
        openTabs.Add(tab);

        List<TabItem> tabs = openTabs.Select(t => t.TabItem).ToList();
        tabControl.Items.Clear();
        foreach (var item in tabs)
        {
            tabControl.Items.Add(item);
        }

        tabControl.SelectedItem = tab.TabItem;
    }

    public void OpenNewFile()
    {
        var tab = new EditorTab();
        tab.CloseTab += CloseTab;
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
    public void SaveAsCurrent()
    {
        var current = GetCurrentTab();
        if (current != null)
        {
            current.SaveAs();
        }
    }

    public void CloseAllTabs()
    {
        if (IsNotSaved(out _))
        {
            // TODO fix this to save before closing
            askSaveFiles?.Invoke();
            CloseAllTabs();
        }
        else
        {
            while (openTabs.Count > 0)
            {
                CloseCurrent();
            }
        }
    }

    public void CloseCurrent()
    {
        var current = GetCurrentTab();
        CloseTab(current);
    }

    public void CloseTab(EditorTab tab)
    {
        if (tab != null)
        {
            openTabs.Remove(tab);
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

    public bool IsNotSaved(out EditorTab[] files)
    {
        List<EditorTab> filesList = new List<EditorTab>();
        foreach (var tab in openTabs)
        {
            if (tab.IsDirty)
            {
                filesList.Add(tab);
            }
        }

        files = filesList.ToArray();
        return filesList.Count > 0;
    }

    public void UpdateTheme()
    {
        Background = (IBrush)Application.Current.Resources["editor.Background"];
        tabControl.Background = (IBrush)Application.Current.Resources["editor.tabs.Background"];
        foreach (var tab in openTabs)
        {
            tab.UpdateTheme();
        }
    }
}
