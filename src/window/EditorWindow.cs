using System.Text.Json;
using System.Threading.Tasks;
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
using ExtensionLibGlobal;
using NativeFileDialogSharp;
using Tmds.DBus.Protocol;

public class EditorWindow : Window
{
    // the main editor space
    Editor TextEditor;
    ExplorerElement ExplorerElement;
    QuickInputWindow quickInputWindow;
    NotificationManager NotificationManager;
    TerminalElement TerminalElement;
    TopMenu TopMenu;
    PopupElement PopupElement;

    Sidebars rightSideBar;
    Sidebars leftSideBar;
    Sidebars bottomSideBar;

    public EditorWindow()
    {
        BackupWorkspaces backupWorkspaces = new BackupWorkspaces()
        {
            Folders =
            [
                "file:///home/bjornbes/Desktop/FDriveDump/projects/project",
                "file:///home/bjornbes/Desktop/FDriveDump/projects/textEditor",
            ]
        };
        /*
                GlobalStorage globalStorage = new GlobalStorage()
                {
                    BackupWorkspaces = backupWorkspaces,
                    Theme = "normalDark",
                    ThemeBackground = "#252526"
                };

                string json = JsonSerializer.Serialize(globalStorage, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText("./test.json", json);
        */

        InitializeComponent();
        Commands.AddCommand(
            "Open Folder Dialog",
            "file.openFolder.dialog",
            OpenSelectFolderDialog,
            false
        );
        Commands.AddCommand(
            "Open Save file Dialog",
            "file.save.file.dialog",
            OpenSaveFileDialog,
            false
        );
        Commands.AddCommand("Open File", "file.open", OpenFile, false);
        Commands.AddCommand("Open New File", "file.open.new", OpenNewFile, true);
        Commands.AddCommand(
            "Change theme from list",
            "theme.change.list",
            ChangeThemeFromList,
            true
        );
        Commands.AddCommand("Change theme from path", "theme.change", ChangeThemeFromPath, false);
        Commands.AddCommand("Update explorer", "update.file.tree", ExplorerElement.UpdateExplorer, false);

        Commands.AddCommand("Save file", "file.save", TextEditor.SaveCurrent, true);
        Commands.AddCommand("Save file as", "file.save.as", TextEditor.SaveAsCurrent, true);

        Commands.AddCommand("Show notification", "show.notification", DisplayNotification_Cmd, false);

        ProjectConfigs.InitializeConfig();
        Extensions.StartExtensions();

        List<string> paths = getRecentOpenPaths(out GlobalStorage globalStorage);
        if (paths.Count > 0)
        {
            ExplorerElement.SetPath(paths.First().Replace("file://", ""));
        }
        if (!string.IsNullOrEmpty(globalStorage.Theme))
        {
            string themePath = Path.Combine(ProjectConfigs.ThemesPath, globalStorage.Theme + ".json");
            if (File.Exists(themePath))
            {
                ChangeThemeFromPath(themePath);
            }
        }

        PopupElement = new PopupElement();
        PopupElement.Hide();

        quickInputWindow = new QuickInputWindow();
        quickInputWindow.Hide();

        KeyDown += MainWindow_KeyDown;
        Closing += (_, e) => { OnClosingFunc(e); };

        UpdateTheme();
    }

    void InitializeComponent()
    {
        Height = 600;
        Width = 800;
        MinHeight = 400;
        MinWidth = 600;


        var rootGrid = new Grid
        {
            // Background = new SolidColorBrush(Color.Parse("#333333")) // window background
            Background = Brushes.White, // window background
        };
        rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // top menu height
        rootGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star))); // rest of window


        // TopMenu
        TopMenu = new TopMenu { MinHeight = 40 };
        Grid.SetRow(TopMenu, 0);
        rootGrid.Children.Add(TopMenu);

        // Main layout grid (editor, sidebars)
        DockPanel mainDock = new DockPanel { Background = Brushes.Transparent };
        Grid.SetRow(mainDock, 1);
        rootGrid.Children.Add(mainDock);

        var mainGrid = CreateLayout();
        mainDock.Children.Add(mainGrid);

        var submenuOverlay = new Canvas
        {
            VerticalAlignment = VerticalAlignment.Top,
            Background = Brushes.Transparent,
            IsVisible = false // only visible when a submenu is open
        };


        // Overlay spans both rows
        Grid.SetRowSpan(submenuOverlay, 2);
        rootGrid.Children.Add(submenuOverlay);


        // Pass overlay to TopMenu so it can host dropdown submenus
        TopMenu.SetSubmenuHost(submenuOverlay);

        BuildMenus();

        Content = rootGrid;
    }

    private Grid CreateLayout()
    {
        // ===== Root Grid =====
        var rootGrid = new Grid
        {
            Background = Brushes.White,
            ZIndex = 0,
        };

        // Columns: Left, Splitter, Editor, Splitter, Right
        rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });               // Left sidebar
        rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });                 // Splitter
        rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Editor
        rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });                 // Splitter
        rootGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(250) });               // Right sidebar

        // Rows: Main, Splitter, Bottom, Status
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });   // Main/editor
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5) });                      // Splitter
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(150) });                    // Bottom sidebar
        rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });                     // Status bar

        // ===== Left Sidebar =====
        leftSideBar = new Sidebars();
        leftSideBar.Attach(Dock.Left);

        ExplorerElement = new ExplorerElement();
        ExplorerElement.ElementName = "Explorer";
        ExplorerElement.OpenFolderAction += OpenWorkspace;
        leftSideBar.AddItem(ExplorerElement);

        Grid.SetRow(leftSideBar, 0);
        Grid.SetColumn(leftSideBar, 0);
        rootGrid.Children.Add(leftSideBar);

        // Left Splitter
        var leftSplitter = new GridSplitter
        {
            Width = 5,
            Background = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            ResizeDirection = GridResizeDirection.Columns
        };
        Grid.SetRow(leftSplitter, 0);
        Grid.SetColumn(leftSplitter, 1);
        rootGrid.Children.Add(leftSplitter);

        // ===== Right Sidebar =====
        rightSideBar = new Sidebars();
        rightSideBar.Attach(Dock.Right);

        Grid.SetRow(rightSideBar, 0);
        Grid.SetColumn(rightSideBar, 4);
        rootGrid.Children.Add(rightSideBar);

        // Right Splitter
        var rightSplitter = new GridSplitter
        {
            Width = 5,
            Background = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            ResizeDirection = GridResizeDirection.Columns
        };
        Grid.SetRow(rightSplitter, 0);
        Grid.SetColumn(rightSplitter, 3);
        rootGrid.Children.Add(rightSplitter);

        // ===== Bottom Sidebar =====
        bottomSideBar = new Sidebars();
        bottomSideBar.Attach(Dock.Bottom);

        TerminalElement = new TerminalElement();
        TerminalElement.ElementName = "Terminal";
        TerminalElement.OnSend += (clientId, message) =>
        {
            Extensions.WritePackageToClient(clientId, ExtensionLibGlobal.PackageTypes.Message, message);
        };
        bottomSideBar.AddItem(TerminalElement);

        Grid.SetRow(bottomSideBar, 2);
        Grid.SetColumn(bottomSideBar, 0);
        Grid.SetColumnSpan(bottomSideBar, 5);
        rootGrid.Children.Add(bottomSideBar);

        // Bottom Splitter
        var bottomSplitter = new GridSplitter
        {
            Height = 5,
            Background = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            ResizeDirection = GridResizeDirection.Rows
        };
        Grid.SetRow(bottomSplitter, 1);
        Grid.SetColumnSpan(bottomSplitter, 5);
        rootGrid.Children.Add(bottomSplitter);

        // ===== Main Editor =====
        TextEditor = new Editor
        {
            Name = "editor",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            askSaveFiles = async () =>
            {
                await askSaveFiles();
            },
            Background = (IBrush)Application.Current.Resources["editor.Background"]
        };
        Grid.SetRow(TextEditor, 0);
        Grid.SetColumn(TextEditor, 2);
        rootGrid.Children.Add(TextEditor);

        // ===== Status Bar =====
        var statusBar = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#007acc")),
            Child = new TextBlock
            {
                Text = "Status: Ready",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0),
                Foreground = Brushes.White,
            },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
        };
        Grid.SetRow(statusBar, 3);
        Grid.SetColumnSpan(statusBar, 5);
        rootGrid.Children.Add(statusBar);

        // ===== Notification Overlay =====
        var overlayGrid = new Grid(); // top-level container
        overlayGrid.Children.Add(rootGrid); // base layer

        NotificationManager = new NotificationManager
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(10),
            ZIndex = 100, // make sure it's on top
        };

        overlayGrid.Children.Add(NotificationManager); // floating overlay layer

        return overlayGrid;
    }

    void BuildMenus()
    {
        TextBlock trigger;
        trigger = TopMenu.AddMenu("File");
        TopMenu.AddItem(trigger, "New", () => TextEditor.OpenNewFile());
        TopMenu.AddItem(trigger, "Open", () => OpenFileFromPath());
        TopMenu.AddItem(trigger, "Open Folder", () => OpenFolderFromPath());
        TopMenu.AddItem(trigger, "Save", () => Commands.RunCommand("file.save"));
        TopMenu.AddItem(trigger, "Save As...", () => Commands.RunCommand("file.save.as"));
        TopMenu.AddItem(trigger, "Close tab", () => TextEditor.CloseCurrent());
        TopMenu.AddItem(trigger, "Close all tabs", () => TextEditor.CloseAllTabs());
        TopMenu.AddItem(trigger, "Exit", () => Close());

        TopMenu.AddMenu("Edit", new[] { "Undo", "Redo" },
                        new Action[] { () => Console.WriteLine("Undo"), () => Console.WriteLine("Redo") });

        trigger = TopMenu.AddMenu("View");

        TopMenu.AddMenu("Help", new[] { "About", "Docs" },
                        new Action[] { () => Console.WriteLine("About"), () => Console.WriteLine("Docs") });

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
        GlobalStorage globalStorage = ProjectConfigs.ReadFile<GlobalStorage>(ProjectConfigs.GlobalStorageFilePath);
        globalStorage.Theme = themeFile.Name;
        ProjectConfigs.WriteFile(ProjectConfigs.GlobalStorageFilePath, globalStorage);

        ProjectConfigs.CurrentTheme = themeFile;
        ThemeLoader.ApplyTheme(themeFile);
        Commands.RunCommand("quickpalette.close");
        UpdateTheme();
    }

    void UpdateTheme()
    {
        TextEditor.UpdateTheme();
        // ExplorerElement.UpdateTheme();
        // TerminalElement.UpdateTheme();
        // TopMenu.UpdateTheme();
        // leftSideBar.UpdateTheme();
        // rightSideBar.UpdateTheme();
        // bottomSideBar.UpdateTheme();
    }

    void OpenFile(string file)
    {
        TextEditor.OpenFile(file);
    }
    void OpenFileFromPath()
    {
        string file = OpenSelectFileDialog();
        if (string.IsNullOrEmpty(file))
        {
            return;
        }
        TextEditor.OpenFile(file);
    }
    void OpenFolderFromPath()
    {
        string path = OpenSelectFolderDialog();
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        ExplorerElement.SetPath(path);
    }
    void OpenNewFile()
    {
        TextEditor.OpenNewFile();
    }

    void OpenWorkspace(string workspacePath)
    {
        ProjectConfigs.SetWorkspace(workspacePath);
    }

    void DisplayNotification(string title, string message, NotificationType type, Dictionary<string, Action> buttonActions)
    {
        NotificationManager.ShowNotification(title, message, type, buttonActions);
    }
    void DisplayNotification_Cmd(string title, string message, int type)
    {
        Dictionary<string, Action> buttonActions = new Dictionary<string, Action>()
        {
            { "Yes", () => { } },
            { "No", () => { } },
        };
        NotificationManager.QueueNotification(title, message, (NotificationType)type, buttonActions);
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
        if (e.Key == Key.T && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift))
        {
            string[] list = { "close", "ok", "why" };
            Action<int> action = new Action<int>((result) =>
            {
                Console.WriteLine(list[result]);
            });
            OpenPopupWindow("Test pop up", "This here is a test", NotificationType.Info, action, list);
        }
        if (e.Key == Key.M)
        {
            Dictionary<string, Action> actions = new Dictionary<string, Action>()
            {
                { "close", () => { Console.WriteLine("close"); } },
                { "ok", () => { Console.WriteLine("ok"); } },
                { "why", () => { Console.WriteLine("why"); } },
            };
            DisplayNotification("Test pop up", "This here is a test", NotificationType.Info, actions);
        }
    }
    private void OpenCommandPalette()
    {
        quickInputWindow.OpenCommandPalette();
        quickInputWindow.ShowDialog(this);
    }
    private async Task<int> OpenPopupWindowAsync(string title, string message, NotificationType type, params string[] actions)
    {
        if (PopupElement == null || !PopupElement.IsLoaded)
        {
            PopupElement = new PopupElement();
        }
        int result = -1;

        PopupElement.ShowPopup(title, message, type, (res) => result = res, actions);
        await PopupElement.ShowDialog<int>(this);

        while (result == -1) { }

        return result;
    }
    private void OpenPopupWindow(string title, string message, NotificationType type, Action<int> onResult, params string[] actions)
    {
        PopupElement.ShowPopup(title, message, type, onResult, actions);
        PopupElement.Show(this);
    }

    private void ShowMessage(string msg) { }

    private List<string> getRecentOpenPaths(out GlobalStorage globalStorage)
    {
        globalStorage = ProjectConfigs.ReadFile<GlobalStorage>(ProjectConfigs.GlobalStorageFilePath);
        BackupWorkspaces backupWorkspaces = globalStorage.BackupWorkspaces;
        return backupWorkspaces.Folders.ToList();
    }

    public string OpenSelectFolderDialog()
    {
        var dialog = Dialog.FolderPicker();
        if (dialog.IsOk)
        {
            List<string> paths = getRecentOpenPaths(out GlobalStorage globalStorage);

            if (!paths.Contains(dialog.Path))
            {
                paths.Add(dialog.Path);
            }

            globalStorage.BackupWorkspaces.Folders = paths.ToArray();
            ProjectConfigs.WriteFile(ProjectConfigs.GlobalStorageFilePath, globalStorage);
            return dialog.Path;
        }
        return "";
    }
    public string OpenSelectFileDialog()
    {
        string defPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (ProjectConfigs.WorkspacePath != "")
        {
            defPath = ProjectConfigs.WorkspacePath;
        }
        var dialog = Dialog.FileOpen(defaultPath: defPath);
        if (dialog.IsOk)
        {
            return dialog.Path;
        }
        return "";
    }
    private string OpenSaveFileDialog()
    {
        string defPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (ProjectConfigs.WorkspacePath != "")
        {
            defPath = ProjectConfigs.WorkspacePath;
        }
        var dialog = Dialog.FileSave(defaultPath: defPath);
        if (dialog.IsOk)
        {
            return dialog.Path;
        }
        return "";
    }

    bool done = true;
    void OnClosingFunc(WindowClosingEventArgs e)
    {
        if (TextEditor.IsNotSaved(out _))
        {
            e.Cancel = true;
            done = false;
            _ = askSaveFiles();
            Thread thread = new Thread(new ThreadStart(waitForClose));
            thread.Start();
        }
    }
    void waitForClose()
    {
        while (!done)
        {

        }
        Close();
    }
    async Task askSaveFiles()
    {
        if (TextEditor.IsNotSaved(out EditorTab[] editortabs))
        {
            foreach (var tab in editortabs)
            {
                string name = Path.GetFileName(tab.FilePath);
                int result = await OpenPopupWindowAsync($"Do you want to save the changes you made to {name}?", "Your changes will be lost if you don't save them.", NotificationType.Warning, "Don't Save", "Cancel", "Save");
                if (result == 0)
                {
                    tab.DontSave();
                }
                if (result == 1)
                {
                    return;
                }
                if (result == 2)
                {
                    tab.Save();
                }
            }
            done = true;
        }
    }
    protected override void OnClosed(EventArgs e)
    {
        {
            base.OnClosed(e);

            Extensions.CloseAllClients();

            quickInputWindow.Close();
        }
    }
}
