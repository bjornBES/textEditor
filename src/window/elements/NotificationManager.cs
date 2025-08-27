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
    private int _selectedIndex = -1;
    private StackPanel _stack;
    private double _targetHeight;

    private readonly Canvas _overlayCanvas;

    // private NotificationManager(string titleText, string messageText, NotificationType type, List<string> options, Window owner)
    public NotificationManager()
    {
        /*
                // this.Width = 300;
                // this.MinHeight = 80;
                // this.MaxHeight = 180;
                // this.CanResize = false;
                // this.WindowStartupLocation = WindowStartupLocation.Manual;
                // this.Background = Brushes.DimGray;
                _stack = new StackPanel
                {
                    Margin = new Thickness(10),
                    Spacing = 5
                };

                Grid grid = new Grid()
                {
                    IsVisible = true,
                };
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                TextBlock typeBox = new TextBlock()
                {
                    Text = GetTypeChar(type) + " ",
                    Foreground = Brushes.White
                };
                Grid.SetColumn(typeBox, 0);
                grid.Children.Add(typeBox);

                TextBlock titleBox = new TextBlock
                {
                    Text = titleText,
                    FontWeight = FontWeight.Bold,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap
                };
                Grid.SetColumn(titleBox, 1);
                grid.Children.Add(titleBox);

                _stack.Children.Add(grid);
                if (!string.IsNullOrEmpty(messageText))
                {
                    TextBlock messageBox = new TextBlock
                    {
                        Text = messageText,
                        Foreground = Brushes.White,
                        TextWrapping = TextWrapping.Wrap
                    };
                    _stack.Children.Add(messageBox);
                }

                var buttonsPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                for (int i = 0; i < options.Count; i++)
                {
                    int index = i; // capture for closure
                    var btn = new Button
                    {
                        Content = options[i],
                        FontSize = 12,
                        Background = Brushes.LightGray
                    };
                    btn.Click += (s, e) =>
                    {
                        _selectedIndex = index;
                        this.Close();
                    };
                    buttonsPanel.Children.Add(btn);
                }

                _stack.Children.Add(buttonsPanel);

                this.Content = _stack;

                this.LayoutUpdated += (_, _) =>
                {
                    _targetHeight = _stack.Bounds.Height + 20; // extra padding
                    AnimateHeight(_targetHeight);
                    this.Position = new PixelPoint(
                        (int)(owner.Bounds.Width - this.Bounds.Width) + owner.Position.X,
                        (int)(owner.Bounds.Height - this.Bounds.Height) + owner.Position.Y
                    );
                };
        */

Orientation = Orientation.Vertical;
        Spacing = 5;
        HorizontalAlignment = HorizontalAlignment.Right;
        VerticalAlignment = VerticalAlignment.Bottom;
        Margin = new Thickness(10);

    }
    /*
    private void AnimateHeight(double newHeight)
    {
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(200),
            Easing = new CubicEaseOut(),
            Children =
            {
                new KeyFrame
                {
                    Setters =
                    {
                        new Setter(Window.HeightProperty, newHeight)
                    },
                    Cue = new Cue(1d)
                }
            }
        };

        animation.RunAsync(this);
    }
    */

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
    /*
        public static int ShowNotification(Window owner, string title, string message, NotificationType type, List<string> options)
        {
            var win = new NotificationManager(title, message, type, options, owner)
            {
                SystemDecorations = SystemDecorations.None,
                Topmost = true,
                ShowInTaskbar = false
            };

            // ShowDialog is synchronous and returns when window closes
            win.ShowDialog(owner);
            return win._selectedIndex;
        }
    */
 /// <summary>
    /// Shows a synchronous notification inside the main window.
    /// </summary>
    /// <param name="title">Notification title</param>
    /// <param name="message">Notification message</param>
    /// <param name="type">Notification type</param>
    /// <param name="buttonActions">Dictionary of button text => action</param>
    public void ShowNotification(string title, string message, NotificationType type, Dictionary<string, Action> buttonActions)
    {
        bool completed = false;

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
                completed = true;
            };

            buttonsPanel.Children.Add(btn);
        }

        stack.Children.Add(buttonsPanel);
        border.Child = stack;

        this.Children.Add(border);
/*
        // Process synchronously
        while (!completed)
        {
            Dispatcher.UIThread.RunJobs();
            System.Threading.Thread.Sleep(10);
        }
*/
    }

}
