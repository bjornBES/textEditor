using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using NativeFileDialogSharp;

public class EditorWindow : Window
{
    // the main editor space
    Editor TextEditor;
    ExplorerElement ExplorerElement;
    QuickInputWindow quickInputWindow;

    Sidebars rightSideBar;
    Sidebars leftSideBar;
    Sidebars bottomSideBar;

    public EditorWindow()
    {
        ExtensionManifest extension = new ExtensionManifest();
        extension.Name = "textext";
        extension.DisplayName = "Text Ext";
        extension.Version = "0";
        extension.Contributes = new Contributes();
        extension.Contributes.Language = new ExtLanguage();
        extension.Contributes.Language.LanguageId = "Test";
        extension.Contributes.Language.Extensions = [ ".test" ];

        string json = JsonSerializer.Serialize(extension, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText("./test.json", json);

        Commands.AddCommand(
            "Open Folder Dialog",
            "file.openFolder.dialog",
            OpenSelectFolderDialog,
            false
        );
        Commands.AddCommand("Open File", "file.open", OpenFile, false);
        Commands.AddCommand(
            "Change theme from list",
            "theme.change.list",
            ChangeThemeFromList,
            true
        );
        Commands.AddCommand("Change theme from path", "theme.change", ChangeThemeFromPath, false);
        InitializeComponent();

        ProjectConfigs.InitializeConfig();
        Extensions.StartExtensions();

        quickInputWindow = new QuickInputWindow();
        quickInputWindow.Hide();

        ThemeFile themeFile = new ThemeFile()
        {
            name = "normalDark",
            type = "dark",
            colors = new Dictionary<string, ElementColor>()
            {
                {
                    "editor",
                    new ElementColor() { background = "333333" }
                },
                {
                    "sideBar",
                    new ElementColor()
                    {
                        background = "252526",
                        subElements = new Dictionary<string, ElementColor>()
                        {
                            {
                                "Explorer",
                                new ElementColor() { background = "00FF00" }
                            },
                        },
                    }
                },
            },
            semanticHighlighting = false,
            tokenColors = null,
        };
        // ProjectConfigs.SaveTheme(themeFile);

        KeyDown += MainWindow_KeyDown;
    }

    void InitializeComponent()
    {
        Height = 600;
        Width = 800;
        MinHeight = 400;
        MinWidth = 600;

        Content = CreateLayout();
    }

    private DockPanel CreateLayout()
    {
        var dockPanel = new DockPanel();

        // ===== Status bar =====
        var statusBar = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#007acc")),
            Height = 24,
            Child = new TextBlock
            {
                Text = "Status: Ready",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0),
                Foreground = Brushes.White,
            },
        };
        DockPanel.SetDock(statusBar, Dock.Bottom);
        dockPanel.Children.Add(statusBar);

        // ===== Top Tab bar =====
        var tabBar = new StackPanel
        {
            Height = 35,
            Orientation = Orientation.Horizontal,
            Background = new SolidColorBrush(Color.Parse("#333333")),
        };
        tabBar.Children.Add(
            new TextBlock
            {
                Text = " file1.cs ",
                Margin = new Thickness(10, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.White,
            }
        );
        DockPanel.SetDock(tabBar, Dock.Top);
        dockPanel.Children.Add(tabBar);

        // ===== Sidebar =====
        leftSideBar = new Sidebars();

        leftSideBar.MakeDraggable(dockPanel, Dock.Left);
        ExplorerElement = new ExplorerElement();
        ExplorerElement.OpenFolderAction += OpenWorkspace;
        leftSideBar.AddItem(ExplorerElement);

        DockPanel.SetDock(leftSideBar, Dock.Left);
        dockPanel.Children.Add(leftSideBar);
        rightSideBar = new Sidebars();
        rightSideBar.MakeDraggable(dockPanel, Dock.Right);

        DockPanel.SetDock(rightSideBar, Dock.Right);
        dockPanel.Children.Add(rightSideBar);

        bottomSideBar = new Sidebars();
        bottomSideBar.MakeDraggable(dockPanel, Dock.Bottom);

        TextBox clientBox = new TextBox()
        {
            Text = "",
            IsVisible = true,
        };

        TextBox messageBox = new TextBox()
        {
            Text = "",
            IsVisible = true,
        };
        Button button = new Button();
        button.Content = "Send";
        button.Click += (s, e) =>
        {
            Extensions.WriteToClient(clientBox.Text, messageBox.Text);
        };
        StackPanel stackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
        };
        stackPanel.Children.Add(clientBox);
        stackPanel.Children.Add(messageBox);
        stackPanel.Children.Add(button);
        bottomSideBar.AddItem(stackPanel);

        DockPanel.SetDock(bottomSideBar, Dock.Bottom);
        dockPanel.Children.Add(bottomSideBar);

        // ===== Main Editor Area =====
        TextEditor = new Editor() { Name = "editor" };

        dockPanel.Children.Add(TextEditor);
        return dockPanel;
    }

    void ChangeThemeFromList()
    {
        Commands.RunCommand("themes.open");
        Commands.RunCommand("quickpalette.search.watermark", "Select color theme");
    }

    void ChangeThemeFromPath(string path)
    {
        string json = File.ReadAllText(path);
        ThemeFile themeFile = JsonSerializer.Deserialize<ThemeFile>(json);
        ChangeTheme(themeFile);
    }

    void ChangeTheme(ThemeFile themeFile)
    {
        Dictionary<string, ElementColor> colors = themeFile.colors;
        parseElements(colors);
        Commands.RunCommand("quickpalette.close");
    }

    void parseElements(Dictionary<string, ElementColor> elements)
    {
        if (elements == null)
        {
            return;
        }

        for (int i = 0; i < elements.Count; i++)
        {
            string fieldName = elements.Keys.ElementAt(i);
            ElementColor color = elements[fieldName];

            if (fieldName == TextEditor.Name)
            {
                if (!string.IsNullOrEmpty(color.background))
                {
                    TextEditor.Background = new SolidColorBrush(
                        Color.Parse("#" + color.background)
                    );
                }
            }
            else if (fieldName == "texteditor")
            {
                if (!string.IsNullOrEmpty(color.background))
                {
                    TextEditor.ChangeDefaultBackgroundColor("#" + color.background);
                }
            }
            else if (fieldName == "sideBar")
            {
                if (!string.IsNullOrEmpty(color.background))
                {
                    rightSideBar.Background = new SolidColorBrush(
                        Color.Parse("#" + color.background)
                    );
                    rightSideBar.sideBar.Background = new SolidColorBrush(
                        Color.Parse("#" + color.background)
                    );
                    leftSideBar.Background = new SolidColorBrush(
                        Color.Parse("#" + color.background)
                    );
                    leftSideBar.sideBar.Background = new SolidColorBrush(
                        Color.Parse("#" + color.background)
                    );
                }
            }
            else if (fieldName == "explorer")
            {
                if (!string.IsNullOrEmpty(color.background))
                {
                    ExplorerElement.Background = new SolidColorBrush(
                        Color.Parse("#" + color.background)
                    );
                }
            }

            if (color.subElements != null)
            {
                parseElements(color.subElements);
            }
        }
    }

    void OpenFile(string file)
    {
        TextEditor.OpenFile(file);
    }

    void OpenWorkspace(string workspacePath)
    {
        ProjectConfigs.SetWorkspace(workspacePath);
    }

    protected override void OnResized(WindowResizedEventArgs e)
    {
        base.OnResized(e);
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.P && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift))
        {
            OpenCommandPalette();
        }
    }

    private void OpenCommandPalette()
    {
        quickInputWindow.OpenCommandPalette();
        quickInputWindow.ShowDialog(this);
        quickInputWindow.Show();
    }

    private void ShowMessage(string msg) { }

    public string OpenSelectFolderDialog()
    {
        var dialog = Dialog.FolderPicker();
        if (dialog.IsOk)
        {
            return dialog.Path;
        }
        return "";
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        Extensions.CloseAllClients();

        quickInputWindow.Close();
    }
}
