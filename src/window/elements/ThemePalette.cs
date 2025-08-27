

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

public class ThemePalette : Panel
{
    string[] themes;
    public ThemePalette()
    {
        Width = 600;
        Height = 400;
        Background = new SolidColorBrush(Color.Parse("#1e1e1e"));

        InitializeComponent();
    }

    void InitializeComponent()
    {
    }

    public void VisibleChanged()
    {
        string themePath = ProjectConfigs.ThemesPath;
        themes = Directory.GetFiles(themePath, "*.json", SearchOption.AllDirectories);
        for (int i = 0; i < themes.Length; i++)
        {
            themes[i] = Path.GetFileNameWithoutExtension(themes[i]);
        }
        Commands.RunCommand("quickpalette.list.update", [themes]);
    }
    public void ThemePalette_Tapped(object sender, TappedEventArgs e)
    {
        SelecteTheme();
    }
    public void ThemePalette_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SelecteTheme();
        }
    }

    public void ThemePalette_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }
        string searchText = Commands.RunCommand("quickpalette.search").ToString();
        if (searchText.Length == 0)
        {
            return;
        }

        var search = searchText.ToLowerInvariant().Trim() ?? "";
        var filtered = themes
            .Where(c => c.ToLowerInvariant().Contains(search))
            .ToList();

        List<object> filteredItems = [.. filtered];
        Commands.RunCommand("quickpalette.list.update", [filteredItems.ToArray()]);
    }

    void Close()
    {
        Commands.RunCommand("quickpalette.close");
    }


    private void SelecteTheme()
    {
        ListBox _themesListBox = (ListBox)Commands.RunCommand("quickpalette.list");
        if (_themesListBox.SelectedIndex >= 0)
        {
            string selectedTheme = _themesListBox.SelectedItem?.ToString();
            string themePath = ProjectConfigs.ThemesPath;
            string[] paths = Directory.GetFiles(themePath, "*.json", SearchOption.AllDirectories);

            string selectedThemePath = "";
            for (int i = 0; i < paths.Length; i++)
            {
                if (Path.GetFileNameWithoutExtension(paths[i]) == selectedTheme)
                {
                    selectedThemePath = paths[i];
                }
            }

            if (string.IsNullOrEmpty(selectedThemePath))
            {
                return;
            }

            Commands.RunCommand("theme.change", selectedThemePath);
        }
    }

}