using Avalonia;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.Styling;
public class App : Application
{
    public static StyleInclude AvaloniaEditFluent = new StyleInclude(new Uri("avares://AvaloniaEdit/"))
        {
            Source = new Uri("avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml")
        };
    public override void Initialize()
    {
        Styles.Add(new FluentTheme() { });
        Styles.Add(AvaloniaEditFluent);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new EditorWindow();
            /*
            desktop.MainWindow = new Window
            {
                Width = 800,
                Height = 600,
                Content = new TextEditor
                {
                    Text = "// AvaloniaEdit is working!",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 14,
                    ShowLineNumbers = true,
                    SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#"),
                    Background = Brushes.Black,
                    Foreground = Brushes.White,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                }
            };
            */
        }
        base.OnFrameworkInitializationCompleted();
    }
}