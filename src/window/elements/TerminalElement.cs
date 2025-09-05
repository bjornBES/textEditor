using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Input;
using System;

public class TerminalElement : SidebarElement
{
    private readonly Label Header;
    private readonly TextBox ClientBox;
    private readonly TextBox MessageBox;
    private readonly Button SendButton;

#nullable enable
    public event Action<string, string>? OnSend; // (clientId, message)

    public TerminalElement()
    {
        Orientation = Orientation.Vertical;
        Margin = new Thickness(10);
        Focusable = true;

        Header = new Label()
        {
            Content = "TERMINAL",
            Foreground = Brushes.Lime,
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        Children.Add(Header);

        ClientBox = new TextBox()
        {
            Watermark = "Client ID",
            Text = "",
            Margin = new Thickness(0, 5, 0, 0),
        };
        Children.Add(ClientBox);

        MessageBox = new TextBox()
        {
            Watermark = "Client Message",
            Text = "",
            AcceptsReturn = true,
            Height = 60,
            Margin = new Thickness(0, 5, 0, 0),
        };
        Children.Add(MessageBox);

        SendButton = new Button()
        {
            Content = "Send",
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 10, 0, 0),
        };
        SendButton.Click += (s, e) =>
        {
            if (!string.IsNullOrWhiteSpace(ClientBox.Text) && !string.IsNullOrWhiteSpace(MessageBox.Text))
            {
                OnSend?.Invoke(ClientBox.Text, MessageBox.Text);
                MessageBox.Text = ""; // clear after send
            }
        };
        Children.Add(SendButton);
    }
}
