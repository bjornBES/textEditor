using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using ExtensionLibGlobal;
public class NotificationManager : StackPanel
{

    public NotificationManager()
    {
        Orientation = Orientation.Vertical;
        Spacing = 5;
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom;
        Margin = new Thickness(10);
    }

    private string GetTypeChar(NotificationType type)
    {
        return type switch
        {
            NotificationType.Info => "I",
            NotificationType.Success => "S",
            NotificationType.Warning => "W",
            NotificationType.Error => "E",
            _ => ""
        };
    }

    public void QueueNotification(string title, string message, NotificationType type, Dictionary<string, Action> buttonActions)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ShowNotification(title, message, type, buttonActions);
            Commands.RunCommand("quickpalette.close");
        });
    }
    /// <summary>
    /// Shows a synchronous notification inside the main window.
    /// </summary>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="type">Notification type</param>
    /// <param name="buttonActions">Dictionary of button text => action</param>
    public void ShowNotification(string title, string message, NotificationType type, Dictionary<string, Action> buttonActions)
    {
        var border = new Border
        {
            Background = Brushes.DimGray,
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(10),
            Width = 300
        };

        var stack = new StackPanel { Spacing = 5 };

        // Title + type
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        var typeBlock = new TextBlock
        {
            Text = GetTypeChar(type),
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold
        };
        Grid.SetColumn(typeBlock, 0);
        grid.Children.Add(typeBlock);

        var titleBlock = new TextBlock
        {
            Text = title,
            Foreground = Brushes.White,
            FontWeight = FontWeight.Bold,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };
        Grid.SetColumn(titleBlock, 1);
        grid.Children.Add(titleBlock);

        stack.Children.Add(grid);

        if (!string.IsNullOrEmpty(message))
        {
            stack.Children.Add(new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });
        }

        var buttonsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5,
            Margin = new Thickness(0, 5, 0, 0)
        };

        foreach (var kvp in buttonActions)
        {
            string buttonText = kvp.Key;
            Action action = kvp.Value;

            var btn = new Button
            {
                Content = buttonText,
                FontSize = 12,
                Background = Brushes.LightGray
            };

            btn.Click += (s, e) =>
            {
                action?.Invoke();
                this.Children.Remove(border);
            };

            buttonsPanel.Children.Add(btn);
        }

        stack.Children.Add(buttonsPanel);
        border.Child = stack;

        this.Children.Add(border);
    }
}
