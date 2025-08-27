

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

public class CommandPalettePanel : Panel
{
    public Action<string> OnCommandSelected;
    public CommandPalettePanel()
    {
        KeyDown += CommandPaletteWindow_KeyDown;
        Width = 600;
        Height = 400;
        Background = new SolidColorBrush(Color.Parse("#1e1e1e"));

        InitializeComponent();
    }

    void CommandPaletteWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    void InitializeComponent()
    {
    }

    public void VisibleChanged()
    {
        List<object> items = new List<object>();
        foreach (var command in Commands.CommandEntries)
        {
            if (command.Types.Length == 0)
            {
                if (command.ShowInCC == false)
                {
                    continue;
                }
                object arg = command.DisplayName;
                items.Add(arg);
                if (items.Count == QuickInputWindow.MaxNumberItemsInList)
                {
                    break;
                }
            }
        }
        Commands.RunCommand("quickpalette.list.update", [items.ToArray()]);
    }
    public void CommandPalettePanel_Tapped(object sender, TappedEventArgs e)
    {
        ExecuteSelectedCommand();
    }
    public void CommandPalettePanel_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ExecuteSelectedCommand();
        }
    }

    public void CommandPalettePanel_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }
        string searchText = Commands.RunCommand("quickpalette.search").ToString();
        if (searchText.Length == 0)
        {
            IsVisible = false;
            return;
        }

        var search = searchText.ToLowerInvariant().Substring(1).Trim() ?? "";
        var filtered = Commands.CommandEntries
            .Where(c => c.DisplayName.ToLowerInvariant().Contains(search))
            .ToList();

        List<object> filteredItems = new List<object>();
        foreach (var command in filtered)
        {
            filteredItems.Add(command.DisplayName);
        }
        Commands.RunCommand("quickpalette.list.update", [filteredItems.ToArray()]);
    }

    void Close()
    {
        Commands.RunCommand("quickpalette.close");
    }


    private void ExecuteSelectedCommand()
    {
        ListBox _commandListBox = (ListBox)Commands.RunCommand("quickpalette.list");
        if (_commandListBox.SelectedIndex >= 0)
        {
            var selectedCommand = _commandListBox.SelectedItem?.ToString();
            OnCommandSelected?.Invoke(selectedCommand);
        }
    }

}