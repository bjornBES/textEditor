

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
public class ExplorerNode
{
    public string Header { get; set; }
    public string Tag { get; set; }
    public int Level { get; set; } = 0;
    public bool IsExpanded { get; set; } = false;
    public List<ExplorerNode> Children { get; set; } = new List<ExplorerNode>();
    public bool IsDirectory { get; set; }
}
public class ExplorerElement : SidebarElement
{
    Label Header;
    ScrollViewer scroll;
    StackPanel treeRoot;
    StackPanel panel;
    Button OpenFolder;
    public string ProjectPath { get; set; } = "";
    public Action<string> OpenFolderAction { get; set; }
    const int ExplorerFontSize = 14;
    private int IndentStep = 16;
    public ExplorerElement()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        Orientation = Orientation.Vertical;
        Margin = new Thickness(0, 10, 10, 10);
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

        scroll = new ScrollViewer { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        treeRoot = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Stretch };
        scroll.Content = treeRoot;

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

        Children.Add(scroll);
        Children.Add(panel);



        UpdateExplorer();
    }

    public void SetPath(string path)
    {
        ProjectPath = path;
        UpdateExplorer();
        FillTree();
        OpenFolderAction(path);
    }

    public void UpdateExplorer()
    {
        if (string.IsNullOrEmpty(ProjectPath))
        {
            Header.Content = "EXPLORER: NO FOLDER OPENED";
            panel.IsVisible = true;
            scroll.IsVisible = false;
            return;
        }
        else
        {
            Header.Content = "EXPLORER";
            panel.IsVisible = false;
            scroll.IsVisible = true;
        }
    }
    void FillTree()
    {
        treeRoot.Children.Clear();
        var rootNode = BuildNode(ProjectPath);
        var rootControl = BuildNodeControl(rootNode, 0);
        treeRoot.Children.Add(rootControl);
    }


    ExplorerNode BuildNode(string path)
    {
        var node = new ExplorerNode { Header = Path.GetFileName(path), Tag = path, IsDirectory = Directory.Exists(path) };


        if (node.IsDirectory)
        {
            foreach (var dir in Directory.GetDirectories(path).OrderBy(d => d)) node.Children.Add(BuildNode(dir));
            foreach (var file in Directory.GetFiles(path).OrderBy(f => f)) node.Children.Add(new ExplorerNode { Header = Path.GetFileName(file), Tag = file, IsDirectory = false });
        }


        return node;
    }

#nullable enable
    Control BuildNodeControl(ExplorerNode node, int indent)
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Stretch };

        var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Stretch };

        // Toggle for directories
        TextBlock? toggle = null;
        if (node.IsDirectory)
        {
            toggle = new TextBlock
            {
                Text = node.IsExpanded ? "-" : "+",
                Width = 20,
                Height = 20,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(indent, 0, 5, 0),
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            headerPanel.Children.Add(toggle);
        }
        else
        {
            // Add spacing for files so text aligns
            headerPanel.Children.Add(new Border { Width = indent + 20 });
        }

        var headerButton = new Button
        {
            Content = node.Header,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            // Margin = new Thickness(indent, 0, 0, 0),
            Margin = new Thickness(0, 0, 0, 0),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Foreground = Brushes.White,
            Focusable = false
        };

        headerButton.PointerEntered += (sender, e) => HoverEffect(headerButton, true);

        headerPanel.Children.Add(headerButton);
        panel.Children.Add(headerPanel);

        // Children container
        var childrenPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(IndentStep, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Stretch };
        foreach (var child in node.Children)
        {
            var childControl = BuildNodeControl(child, 0);
            childrenPanel.Children.Add(childControl);
        }
        childrenPanel.IsVisible = node.IsExpanded; // collapse by default if needed
        panel.Children.Add(childrenPanel);

#nullable disable
        headerButton.Click += (s, e) =>
        {
            // Console.WriteLine($"Selected: {node.Tag}");
            if (!node.IsDirectory)
            {
                Commands.RunCommand("file.open", node.Tag);
            }
            else
            {
                node.IsExpanded = !node.IsExpanded;
                toggle.Text = node.IsExpanded ? "-" : "+";
                childrenPanel.IsVisible = node.IsExpanded;
            }
        };

        return panel;

    }
    void HoverEffect(Button btn, bool hover)
    {
        if (hover)
        {
            btn.Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
        }
        else
        {
            btn.Background = Brushes.Transparent;
        }
    }
}