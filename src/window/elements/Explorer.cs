

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

public class ExplorerElement : StackPanel
{
    Label Header;
    TreeView explorerTreeView;
    StackPanel panel;
    Button OpenFolder;
    public string ProjectPath { get; set; } = "";
    public Action<string> OpenFolderAction { get; set; }
    public ExplorerElement()
    {
        Orientation = Orientation.Vertical;
        Margin = new Thickness(10);
        Focusable = true;

        Header = new Label()
        {
            Content = "EXPLORER",
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Focusable = true,
        };
        Children.Add(Header);

        explorerTreeView = new TreeView()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            SelectionMode = SelectionMode.Multiple,
            Focusable = true,
        };
        explorerTreeView.SelectionChanged += OpenFile;

        panel = new StackPanel()
        {
            Name = "ExplorerStackPanel",
            Orientation = Orientation.Vertical,
        };
        OpenFolder = new Button()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Content = "Open folder"
        };
        OpenFolder.Click += (s, e) =>
        {
            string path = (string)Commands.RunCommand("file.openFolder.dialog");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            SetPath(path);
        };
        panel.Children.Add(new TextBlock() { Text = "You have not opened a folder.", HorizontalAlignment = HorizontalAlignment.Left });
        panel.Children.Add(OpenFolder);

        Children.Add(explorerTreeView);
        Children.Add(panel);

        UpdateExplorer();
    }

    public void OpenFile(object s, SelectionChangedEventArgs e)
    {
        TextBlock item = explorerTreeView.SelectedItem as TextBlock;
        explorerTreeView.SelectedItem = null;
        string path = item.Tag.ToString();
        Commands.RunCommand("file.open", path);
    }

    public void SetPath(string path)
    {
        ProjectPath = path;
        UpdateExplorer();
        OpenFolderAction(path);
    }

    public void UpdateExplorer()
    {
        if (string.IsNullOrEmpty(ProjectPath))
        {
            Header.Content = "EXPLORER: NO FOLDER OPENED";
            panel.IsVisible = true;
            explorerTreeView.IsVisible = false;
            return;
        }
        else
        {
            Header.Content = "EXPLORER";
            panel.IsVisible = false;
            explorerTreeView.IsVisible = true;
        }

        string[] files = Directory.GetFiles(ProjectPath, "**", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string path = ProjectPath.Replace(ProjectPath, "");
            TextBlock text = new TextBlock()
            {
                Text = Path.GetFileName(file),
                Tag = file,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            explorerTreeView.Items.Add(text);
        }
    }
}