
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

public class QuickInputWindow : Window
{
    public const string CommandPaletteIdent = ">";
    public static int MaxNumberItemsInList = 12;
    CommandPalettePanel commandPalettePanel;
    ThemePalette themePalettePanel;
    TextBox searchBox;
    ListBox quickInputList;
    StackPanel panel;
    public QuickInputWindow()
    {
        KeyUp += QuickInputWindow_KeyUp;
        KeyDown += QuickInputWindow_KeyDown;

        Width = 600;
        Height = 400;
        Title = "Quick Input";
        Name = "quickInput";
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        SystemDecorations = SystemDecorations.None;
        Background = new SolidColorBrush(Color.Parse("#1e1e1e"));
        TransparencyLevelHint = new List<WindowTransparencyLevel> { WindowTransparencyLevel.Mica, WindowTransparencyLevel.Transparent };
        Commands.AddCommand("Close Quick Input", "quickpalette.close", ClosePalette, true);
        Commands.AddCommand("Get Quick Input String", "quickpalette.search", GetSearchBox, false);
        Commands.AddCommand("Update Quick Input List", "quickpalette.list.update", QuickInputUpdateList, false);
        Commands.AddCommand("Get Quick Input List", "quickpalette.list", GetQuickInputList, false);
        Commands.AddCommand("Get Quick Input List", "quickpalette.search.watermark", ChangeSearchBoxWatermark, false);

        Commands.AddCommand("Open Themes", "themes.open", OpenThemePalette, false);

        searchBox = new TextBox()
        {
            Name = "quickInputTitle",
            Text = "",
            Margin = new Thickness(10),
            FontSize = 16,
            Focusable = true,
        };

        quickInputList = new ListBox()
        {
            Name = "quickInputList",
            Margin = new Thickness(10),
            Height = 300
        };
        quickInputList.Tapped += QuickInputList_Tapped;
        quickInputList.KeyDown += QuickInputList_KeyDown;


        panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
        };
        panel.Children.Add(searchBox);
        panel.Children.Add(quickInputList);


        commandPalettePanel = new CommandPalettePanel()
        {
            IsVisible = false,
        };

        commandPalettePanel.OnCommandSelected = name =>
        {
            var cmd = Commands.CommandEntries.FirstOrDefault(c => c.DisplayName == name);
            if (cmd.Types.Length == 0)
            {
                Commands.RunCommand(cmd.CommandId);
            }
        };

        themePalettePanel = new ThemePalette()
        {
            IsVisible = false,
        };

        Content = panel;
    }
    void QuickInputList_Tapped(object sender, TappedEventArgs e)
    {
        e.Handled = true;
        if (searchBox.Text.StartsWith(CommandPaletteIdent))
        {
            commandPalettePanel.CommandPalettePanel_Tapped(sender, e);
        }
        else if (themePalettePanel.IsVisible)
        {
            themePalettePanel.ThemePalette_Tapped(sender, e);
        }
    }
    void QuickInputList_KeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = true;
        if (e.Key == Key.Escape)
        {
            ClosePalette();
        }
        else
        {
            if (searchBox.Text.StartsWith(CommandPaletteIdent))
            {
                commandPalettePanel.CommandPalettePanel_KeyDown(sender, e);
            }
            else if (themePalettePanel.IsVisible)
            {
                themePalettePanel.ThemePalette_KeyDown(sender, e);
            }
        }
    }
    void QuickInputUpdateList(object[] objects)
    {
        quickInputList.Items.Clear();
        foreach (var obj in objects)
        {
            quickInputList.Items.Add(obj);
            if (quickInputList.Items.Count == MaxNumberItemsInList)
            {
                break;
            }
        }
    }
    void ChangeSearchBoxWatermark(string Text)
    {
        searchBox.Text = "";
        searchBox.Watermark = Text;
    }
    ListBox GetQuickInputList()
    {
        return quickInputList;
    }
    void QuickInputWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            ClosePalette();
        }
    }
    void QuickInputWindow_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            ClosePalette();
        }

        if (searchBox.Text.StartsWith(CommandPaletteIdent))
        {
            commandPalettePanel.IsVisible = true;
            commandPalettePanel.CommandPalettePanel_KeyUp(sender, e);
        }
        else if (themePalettePanel.IsVisible)
        {
            themePalettePanel.IsVisible = true;
            themePalettePanel.ThemePalette_KeyUp(sender, e);
        }
    }

    public string GetSearchBox()
    {
        return searchBox.Text;
    }

    public void OpenCommandPalette()
    {
        searchBox.Text = CommandPaletteIdent;
        searchBox.CaretIndex = searchBox.Text?.Length ?? 0;
        searchBox.Focus();
        Focus();
        commandPalettePanel.IsVisible = true;
        commandPalettePanel.VisibleChanged();
    }
    public void OpenThemePalette()
    {
        searchBox.Text = "";
        searchBox.CaretIndex = 0;
        searchBox.Focus();
        Focus();
        themePalettePanel.IsVisible = true;
        themePalettePanel.VisibleChanged();
    }

    public void ClosePalette()
    {
        commandPalettePanel.IsVisible = false;
        searchBox.Text = "";
        Hide();
    }
}