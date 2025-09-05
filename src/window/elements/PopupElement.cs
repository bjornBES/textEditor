
using Avalonia.Controls;
using Avalonia.Media;
using ExtensionLibGlobal;
using Avalonia.Layout;
using Avalonia.Input;

public class PopupElement : Window
{
    private TextBlock TitleBox;
    private TextBlock DescriptionBox;
    private Button[] Buttons;
    StackPanel buttonsPanel;
    StackPanel textPanel;
#nullable enable
    public Action<int>? OnResult;

    public PopupElement()
    {
        Title = "Test";
        Width = 600;
        Height = 156;
        Background = "#1e1e1e".GetColoredBrush();

        InitializeComponent();
    }

    void InitializeComponent()
    {
        StackPanel rootPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal
        };

        TitleBox = new TextBlock()
        {
            Text = "",
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = "#FFFFFF".GetColoredBrush(),
            FontSize = 23,
        };

        DescriptionBox = new TextBlock()
        {
            Text = "",
            HorizontalAlignment = HorizontalAlignment.Center,
            Foreground = "#FFFFFF".GetColoredBrush(),
            FontSize = 15,
        };

        textPanel = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        textPanel.Children.Add(TitleBox);
        textPanel.Children.Add(DescriptionBox);


        buttonsPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Center,
            Height = Height - textPanel.Height
        };

        Panel imgPanel = new Panel()
        {
            Background = "#FF0000".GetColoredBrush(),
            Width = Height,
            Height = Height,
        };
        textPanel.Width = Width - imgPanel.Width;

        StackPanel contentsPanel = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Stretch,
            Height = Height,
        };

        contentsPanel.Children.Add(textPanel);
        contentsPanel.Children.Add(buttonsPanel);

        rootPanel.Children.Add(imgPanel);
        rootPanel.Children.Add(contentsPanel);

        Content = rootPanel;
    }
    public void ShowPopup(string title, string message, NotificationType type, Action<int> onResult, params string[] buttonsText)
    {
        OnResult = onResult;
        TitleBox.Text = title;
        DescriptionBox.Text = message;

        Buttons = new Button[buttonsText.Length];
        double buttonWidth = textPanel.Width / 3d;
        for (int i = 0; i < buttonsText.Length; i++)
        {
            Buttons[i] = new Button()
            {
                Content = buttonsText[i],
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = buttonWidth,
                Foreground = "#FFFFFF".GetColoredBrush(),
                FontSize = 15,
            };
            buttonsPanel.Children.Add(Buttons[i]);
            int index = i;
            Buttons[i].Click += (_, __) =>
            {
                OnResult?.Invoke(index);
                Close(index);
            };
        }
    }
}