#include "MainEditorWindow.hpp"
#include "ProjectConfigDir/ProjectConfigs.hpp"
#include "Extensions/Extensions.hpp"
#include <QFileDialog>
#include <QTreeView>
#include <QFile>
#include <QTextStream>
#include <QStatusBar>
#include <QMessageBox>
#include <QProcess>
#include <QFileInfo>

Highlighter *highlighter;
const QString MainTitle = "CS Code";
TextEditorMainWindow::TextEditorMainWindow(QWidget *parent) : QMainWindow(parent)
{
    setWindowTitle(MainTitle);
    resize(1200, 800);

    setupLayout();
    setupMenu();

    editorPanel->setTabsClosable(true);
    connect(editorPanel, &QTabWidget::tabCloseRequested, this, [this](int index)
            {
        QWidget *w = editorPanel->widget(index);
        editorPanel->removeTab(index);
        delete w; });

    highlighter = new Highlighter(nullptr);

    std::vector<ExtensionManifest> Extensions = GetAllExtensions();
    for (const ExtensionManifest &ext : Extensions)
    {
        QString pyPath = globalProgConfigs.get()->ExtensionsPath + PREFERRED_SEPARATOR + ext.name + PREFERRED_SEPARATOR + ext.main;
        QStringList arguments{pyPath};
        QProcess p;
    }
}

void TextEditorMainWindow::setupLayout()
{
    setupPanels();
    setupFileExplorer();
    setupStyling();
}

void TextEditorMainWindow::setupMenu()
{
    QMenu *fileMenu = new QMenu("&File", this);
    QAction *open = fileMenu->addAction("&Open File");
    QAction *save = fileMenu->addAction("&Save File");
    QAction *openFolder = fileMenu->addAction("Open &Folder");
    QAction *openDir = fileMenu->addAction("Open &Directory");
    QMenu *devMenu = new QMenu("&Dev", this);
    QAction *openDefault = devMenu->addAction("&Open Default Project [DEBUG]");

    connect(open, &QAction::triggered, this, &TextEditorMainWindow::openFile);
    connect(save, &QAction::triggered, this, &TextEditorMainWindow::saveFile);
    connect(openFolder, &QAction::triggered, this, [this]()
            {
        QString folder = QFileDialog::getExistingDirectory(this, "Select Project Folder");
        if (!folder.isEmpty()) {
            setupFileExplorer(folder);
        } });
    connect(openDefault, &QAction::triggered, this, [this]()
            {
        QString folder = QString("/mnt/D drive/projects/textEditor/project");
        if (!folder.isEmpty()) {
            setupFileExplorer(folder);
        } });
    connect(openDir, &QAction::triggered, this, [this]()
            {
        QString dirPath = QFileDialog::getExistingDirectory(this, "Open Directory");
        if (!dirPath.isEmpty()) {
            openFileInTab(dirPath);
        } });

    QMenuBar *customMenuBar = new QMenuBar(this);
    customMenuBar->addMenu(fileMenu);
    customMenuBar->addMenu(devMenu);
    setMenuBar(customMenuBar);
}

void TextEditorMainWindow::setupPanels()
{
    statusBar = new QStatusBar(this);
    setStatusBar(statusBar);

    leftPanel = new QTabWidget;
    editorPanel = new QTabWidget;
    rightPanel = new QTabWidget;
    bottomPanel = new QTabWidget;

    editorPanel->setTabsClosable(true);
    bottomPanel->setMaximumHeight(200);

    /*
    fileExplorerWidget = new ProjectFileExplorer;
    leftPanel->addTab(fileExplorerWidget, "Explorer");

    connect(fileExplorerWidget, &ProjectFileExplorer::fileActivated,
    this, &TextEditorMainWindow::openFileInTab);

    bottomPanel->addTab(new QLabel("Terminal"), "Terminal");
    bottomPanel->addTab(new QLabel("Problems"), "Problems");
    rightPanel->addTab(new QLabel("Git"), "Git");
    */

    horizontalSplitter = new QSplitter(Qt::Horizontal);
    horizontalSplitter->addWidget(leftPanel);
    horizontalSplitter->addWidget(editorPanel);
    horizontalSplitter->addWidget(rightPanel);

    leftPanel->setMinimumWidth(200);
    rightPanel->setMinimumWidth(200);

    QWidget *central = new QWidget;
    QVBoxLayout *mainLayout = new QVBoxLayout(central);
    mainLayout->setContentsMargins(0, 0, 0, 0);
    mainLayout->addWidget(horizontalSplitter);
    mainLayout->addWidget(bottomPanel);

    setCentralWidget(central);
}

void TextEditorMainWindow::setupStyling()
{
    QApplication::setStyle("Fusion");
    QPalette darkPalette;
    darkPalette.setColor(QPalette::Window, QColor(30, 30, 30));
    darkPalette.setColor(QPalette::Base, QColor(40, 44, 52));
    darkPalette.setColor(QPalette::Text, QColor(220, 220, 220));
    darkPalette.setColor(QPalette::Highlight, QColor(100, 100, 255));
    QApplication::setPalette(darkPalette);

    //     windowTheme = new WindowTheme();
}

void TextEditorMainWindow::openFile()
{
    QString path = QFileDialog::getOpenFileName(this, "Open File");
    if (!path.isEmpty())
    {
        openFileInTab(path);
    }
}

void TextEditorMainWindow::saveFile()
{
    CodeEditorWidget *editor = qobject_cast<CodeEditorWidget *>(editorPanel->currentWidget());
    if (!editor)
        return;

    QString path = editor->FileURL;
    if (path.isEmpty())
    {
        path = QFileDialog::getSaveFileName(this, "Save File");
        if (path.isEmpty())
            return;
        editor->FileURL = path;
    }

    QFile file(path);
    if (file.open(QIODevice::WriteOnly | QIODevice::Text))
    {
        QTextStream out(&file);
        out << editor->toPlainText();
        statusBar->showMessage("Saved: " + path);
        editorPanel->setTabText(editorPanel->currentIndex(), QFileInfo(path).fileName());
    }
    else
    {
        QMessageBox::warning(this, "Save Failed", "Could not save file.");
    }
}

void TextEditorMainWindow::openFileInTab(const QString &filePath)
{
    for (int i = 0; i < editorPanel->count(); ++i)
    {
        CodeEditorWidget *ed = qobject_cast<CodeEditorWidget *>(editorPanel->widget(i));
        if (ed && ed->FileURL == filePath)
        {
            editorPanel->setCurrentIndex(i);
            return;
        }
    }

    QFile file(filePath);
    if (!file.open(QIODevice::ReadOnly | QIODevice::Text))
    {
        QMessageBox::warning(this, "Open Failed", "Cannot open file: " + filePath);
        return;
    }

    QTextStream in(&file);
    CodeEditorWidget *editor = new CodeEditorWidget(editorPanel);
    editorPanel->addTab(editor, QFileInfo(filePath).fileName());
    editorPanel->setCurrentWidget(editor);

    editor->setPlainText(in.readAll());
    editor->FileURL = filePath;

    highlighter = new Highlighter(editor->document());

    highlighter->UpdateHighlighting(editor->document(), filePath);
}

void TextEditorMainWindow::setupFileExplorer(const QString &projectPath)
{
    if (!projectPath.isEmpty())
    {
        globalProgConfigs.get()->SetWorkspacePath(projectPath.toStdString());
        fileExplorerWidget->setRootPath(projectPath);
    }
}

#include "MainEditorWindow.moc"
