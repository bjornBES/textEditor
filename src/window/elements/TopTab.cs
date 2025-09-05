using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia;
using Avalonia.Media;
using Avalonia.Input;
using System;
using System.Collections.Generic;

public class TopMenuItem
{
    public string Name;
    public Action Action;
}

public class TopMenu : StackPanel
{
    private Dictionary<Control, Border> submenuMap = new();
    private Dictionary<Button, Action> actionMap = new();
    private Panel submenuHost;

    public TopMenu()
    {
        ZIndex = 1;
        Orientation = Orientation.Horizontal;
        Background = new SolidColorBrush(Color.Parse("#333333"));
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Top;
        MinHeight = 40;
    }


    public void SetSubmenuHost(Panel host)
    {
        submenuHost = host;
        submenuHost.IsVisible = false; // Start hidden
        submenuHost.Background = Brushes.Transparent; // Transparent overlay


        submenuHost.PointerExited += (_, __) =>
        {
            HideAllSubmenus();
            submenuHost.IsVisible = false;
        };
    }

    public TextBlock AddMenu(string title)
    {
        var trigger = new TextBlock
        {
            Text = title,
            Margin = new Thickness(20, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.White,
            FontSize = 16,
            Cursor = new Cursor(StandardCursorType.Hand),
            Background = Brushes.Transparent
        };


        var submenu = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#333333")),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            IsVisible = false,
            IsHitTestVisible = true,
            Child = new StackPanel { Orientation = Orientation.Vertical }
        };


        submenuMap[trigger] = submenu;


        trigger.PointerPressed += (_, __) => ShowSubmenu(trigger, submenu);
        submenu.PointerExited += (_, __) => CanHideSubmenus(trigger, submenu);

        Children.Add(trigger);

        return trigger;
    }

    public void AddMenu(string title, string[] items, Action[] actions)
    {
        var trigger = new TextBlock
        {
            Text = title,
            Margin = new Thickness(20, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = Brushes.White,
            FontSize = 16,
            Cursor = new Cursor(StandardCursorType.Hand),
            Background = Brushes.Transparent
        };


        var submenu = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#333333")),
            BorderBrush = Brushes.Gray,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            IsVisible = false,
            IsHitTestVisible = true,
            Child = new StackPanel { Orientation = Orientation.Vertical }
        };

        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            var button = new Button
            {
                Content = items[i],
                Margin = new Thickness(0),
                Padding = new Thickness(10, 5),
                Background = new SolidColorBrush(Color.Parse("#333333")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            button.Click += (_, __) =>
            {
                Action action = actionMap[button];
                
                action?.Invoke();
                HideAllSubmenus();
                if (submenuHost != null)
                    submenuHost.IsVisible = false;
            };
            ((StackPanel)submenu.Child).Children.Add(button);
            actionMap.Add(button, actions[i]);
        }


        submenuMap[trigger] = submenu;


        trigger.PointerPressed += (_, __) => ShowSubmenu(trigger, submenu);
        submenu.PointerExited += (_, __) => CanHideSubmenus(trigger, submenu);

        Children.Add(trigger);
    }

    public void AddItem(TextBlock trigger, string item, Action action)
    {
        Border submenu = submenuMap[trigger];
        var button = new Button
        {
            Content = item,
            Margin = new Thickness(0),
            Padding = new Thickness(10, 5),
            Background = new SolidColorBrush(Color.Parse("#333333")),
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        button.Click += (_, __) =>
        {
            Action action = actionMap[button];

            action?.Invoke();
            HideAllSubmenus();
            if (submenuHost != null)
                submenuHost.IsVisible = false;
        };
        ((StackPanel)submenu.Child).Children.Add(button);
        actionMap.Add(button, action);
    }

    void CanHideSubmenus(Control trigger, Border submenu)
    {
        if (!submenu.IsPointerOver)
        {
            HideAllSubmenus();
        }
    }

    private void ShowSubmenu(Control trigger, Border submenu)
    {
        if (submenuHost == null)
            return;

        HideAllSubmenus();
        var point = trigger.TranslatePoint(new Point(0, trigger.Bounds.Height), submenuHost);

        // Clear any old submenu
        submenuHost.Children.Clear();
        submenuHost.Children.Add(submenu);

        var offset = trigger.Bounds.BottomLeft;
        Canvas.SetLeft(submenu, offset.X);
        Canvas.SetTop(submenu, Bounds.Height); // just below the menu bar

        submenu.IsVisible = true;
        submenuHost.IsVisible = true;
    }

    private void HideAllSubmenus()
    {
        if (submenuHost != null)
        {
            submenuHost.Children.Clear();
        }

        foreach (var item in submenuMap.Values)
        {
            item.IsVisible = false;
        }
        submenuHost.IsVisible = false;
    }
}