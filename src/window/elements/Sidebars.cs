

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

public class Sidebars : Grid
{
    public TabControl sideBar;
    Border dragger;
    public Sidebars()
    {
        ColumnDefinitions = new ColumnDefinitions("Auto,5");
        IsVisible = false;

        sideBar = new TabControl()
        {
            Background = new SolidColorBrush(Color.Parse("#252526")),
            Width = 250,
            MinWidth = 150,
            MaxWidth = 500,
            IsVisible = false,
        };
    }

    public void AddItem(Control control)
    {
        sideBar.Items.Add(control);
        sideBar.IsVisible = true;
        dragger.IsVisible = true;
        IsVisible = true;
    }
    

    public void MakeDraggable(DockPanel dockPanel, Dock dock)
    {
        // Drag handle
        dragger = new Border
        {
            Width = 5,
            Background = new SolidColorBrush(Color.Parse("#3c3c3c")),
            Cursor = new Cursor(StandardCursorType.SizeWestEast),
            IsVisible = false,
        };

        bool isDragging = false;
        Point lastPoint = new Point();

        dragger.PointerPressed += (s, e) =>
        {
            isDragging = true;
            lastPoint = e.GetPosition(dockPanel);
            e.Pointer.Capture(dragger);
        };

        dragger.PointerReleased += (s, e) =>
        {
            isDragging = false;
            e.Pointer.Capture(null);
        };

        dragger.PointerMoved += (s, e) =>
        {
            if (isDragging)
            {
                var point = e.GetPosition(dockPanel);
                double delta = point.X - lastPoint.X;
                double newWidth = sideBar.Width + delta;

                if (newWidth >= sideBar.MinWidth && newWidth <= sideBar.MaxWidth)
                {
                    sideBar.Width = newWidth;
                    lastPoint = point;
                }
            }
        };


        if (dock == Dock.Left)
        {
            Children.Add(sideBar);
            Grid.SetColumn(sideBar, 0);

            Children.Add(dragger);
            Grid.SetColumn(dragger, 1);
        }
        else if (dock == Dock.Right)
        {
            Children.Add(sideBar);
            Grid.SetColumn(sideBar, 1);

            Children.Add(dragger);
            Grid.SetColumn(dragger, 0);
        }
    }
}