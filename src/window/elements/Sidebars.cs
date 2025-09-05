

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Layout;

public class Sidebars : Grid
{
    public TabControl sideBar;

    public Sidebars()
    {
        // Default layout
        sideBar = new TabControl
        {
            Background = new SolidColorBrush(Color.Parse("#252526")),
            IsVisible = false,
            Margin = new Thickness(0),
        };
        Margin = new Thickness(0);

        Children.Add(sideBar);
    }

    public void AddItem(SidebarElement control)
    {
        sideBar.Items.Add(control);
        sideBar.IsVisible = true;
        IsVisible = true;
    }

    public void Attach(Dock dock)
    {
        if (dock == Dock.Left || dock == Dock.Right)
        {
            sideBar.HorizontalAlignment = HorizontalAlignment.Stretch;
            sideBar.VerticalAlignment = VerticalAlignment.Stretch;
            sideBar.MinWidth = 150;
        }
        else if (dock == Dock.Bottom)
        {
            sideBar.HorizontalAlignment = HorizontalAlignment.Stretch;
            sideBar.VerticalAlignment = VerticalAlignment.Stretch;
            sideBar.MinHeight = 100;
        }
    }
}
