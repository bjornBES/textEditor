#include "MainEditorWindow.hpp"
#include "window/Editors/CodeEditorWidget.hpp"
#include "window/DraggableTabWidget.hpp"
#include "ProjectConfigDir/ProjectConfigs.hpp"

#include <QTabBar>
#include <QTabWidget>
#include <QStatusBar>
#include <QDockWidget>
#include <QToolBar>
#include <QApplication>
#include <QMainWindow>
#include <QTextEdit>
#include <QFileDialog>
#include <QFile>
#include <QTextStream>
#include <QMessageBox>
#include <QMenuBar>
#include <QMenu>
#include <QTreeView>
#include <QFileSystemModel>
#include <QSplitter>
#include <QVBoxLayout>
#include <QHBoxLayout>

TextEditorMainWindow::TextEditorMainWindow(QWidget *parent) : QMainWindow(parent)
{
    setupUi();
setupConnections();
}

void TextEditorMainWindow::setupUi()
{
    resize(1000, 700);
    setWindowTitle("Text Editor Workspace");

    editorPanel = new DraggableTabWidget(this);

    leftPanelTabs = new DraggableTabWidget(this);
    leftDock = new QDockWidget(this);
    leftDock->setAllowedAreas(Qt::LeftDockWidgetArea | Qt::RightDockWidgetArea | Qt::BottomDockWidgetArea);
    leftDock->setWidget(leftPanelTabs);
    leftDock->setMinimumWidth(200);
    addDockWidget(Qt::LeftDockWidgetArea, leftDock);

    rightPanelTabs = new DraggableTabWidget(this);
    rightDock = new QDockWidget(this);
    rightDock->setAllowedAreas(Qt::LeftDockWidgetArea | Qt::RightDockWidgetArea | Qt::BottomDockWidgetArea);
    rightDock->setWidget(rightPanelTabs);
    rightDock->setMinimumWidth(200);
    addDockWidget(Qt::RightDockWidgetArea, rightDock);

    bottomPanelTabs = new DraggableTabWidget(this);
    bottomDock = new QDockWidget(this);
    bottomDock->setAllowedAreas(Qt::LeftDockWidgetArea | Qt::RightDockWidgetArea | Qt::BottomDockWidgetArea);
    bottomDock->setWidget(bottomPanelTabs);
    bottomDock->setMinimumWidth(200);
    addDockWidget(Qt::BottomDockWidgetArea, bottomDock);
    
    fileModel = new QFileSystemModel(this);
    fileModel->setRootPath(QDir::homePath());
    fileModel->setFilter(QDir::AllEntries | QDir::NoDotAndDotDot | QDir::AllDirs | QDir::Hidden);
    
    fileExplorer = new QTreeView;
    fileExplorer->setModel(fileModel);
    fileExplorer->setRootIndex(fileModel->index(QDir::homePath()));
    fileExplorer->setHeaderHidden(true);
    connect(fileExplorer, &QTreeView::doubleClicked, this, &TextEditorMainWindow::fileExplorerDoubleClicked);
    
    addLeftPanelWidget(fileExplorer, QIcon::fromTheme("folder"), "Explorer");
    
    /*
    QSplitter *horizontalSplitter = new QSplitter(Qt::Horizontal, this);
    QSplitter *verticalSplitter = new QSplitter(Qt::Vertical, this);
    */

    tabWidget = new QTabWidget;
    tabWidget->setTabsClosable(true);
    tabWidget->setMovable(true);
    tabWidget->setAcceptDrops(false); // Prevent drag-drop into central editor panel
    connect(tabWidget, &QTabWidget::tabCloseRequested, this, &TextEditorMainWindow::closeTab);

    terminalOutput = new QPlainTextEdit;
    terminalOutput->setReadOnly(true);
    terminalOutput->appendPlainText("Welcome to the terminal...");

    logOutput = new QListWidget;
    logOutput->addItem("Log output...");

    addBottomPanelWidget(terminalOutput, QIcon::fromTheme("utilities-terminal"), "Terminal");
    addBottomPanelWidget(logOutput, QIcon::fromTheme("view-list-details"), "Logs");

    setCentralWidget(tabWidget);

    // Tool bar
    QToolBar *toolbar = addToolBar("Main Toolbar");
    createActions();
    toolbar->addAction(actionOpenFile);
    toolbar->addAction(actionSaveFile);
    toolbar->addAction(actionOpenFolder);


    themeSelector = new QComboBox;
    themeSelector->addItem("Light");
    themeSelector->addItem("Dark");
    connect(themeSelector, &QComboBox::currentTextChanged, this, &TextEditorMainWindow::changeTheme);
    toolbar->addWidget(themeSelector);

    // meun bar
    fileMenu = menuBar()->addMenu("&File");

    fileMenu->addAction(actionOpenFile);
    fileMenu->addAction(actionSaveFile);
    fileMenu->addAction(actionOpenFolder);
    fileMenu->addAction(actionExit);

    fileMenu = menuBar()->addMenu("&Dev");
    fileMenu->addAction(actionOpenDefault);

    // Status bar
    statusLabel = new QLabel("Ready");
    statusBar()->addWidget(statusLabel);
}

void TextEditorMainWindow::setupConnections()
{
    // Setup signals and slots, e.g. to open files from explorer, update UI, etc.
}

void TextEditorMainWindow::addBottomPanelWidget(QWidget *widget, const QIcon &icon, const QString &label) {
    bottomPanelTabs->addPanel(widget, label, icon);
}
void TextEditorMainWindow::addLeftPanelWidget(QWidget *widget, const QIcon &icon, const QString &label) {
    leftPanelTabs->addPanel(widget, label, icon);
}
void TextEditorMainWindow::addRightPanelWidget(QWidget *widget, const QIcon &icon, const QString &label) {
    rightPanelTabs->addPanel(widget, label, icon);
}

void TextEditorMainWindow::createActions()
{
    actionOpenFile = new QAction(QIcon::fromTheme("document-open"), "&Open File", this);
    connect(actionOpenFile, &QAction::triggered, this, &TextEditorMainWindow::openFile);

    actionSaveFile = new QAction(QIcon::fromTheme("document-save"), "&Save File", this);
    connect(actionSaveFile, &QAction::triggered, this, &TextEditorMainWindow::saveFile);

    actionOpenFolder = new QAction(QIcon::fromTheme("folder-open"), "&Open Folder", this);
    connect(actionOpenFolder, &QAction::triggered, this, &TextEditorMainWindow::openFolder);

    actionExit = new QAction("E&xit", this);
    connect(actionExit, &QAction::triggered, this, &QWidget::close);

    actionOpenDefault = new QAction("&OpenDefault", this);
    connect(actionOpenDefault, &QAction::triggered, this, &TextEditorMainWindow::openCommonProject);
}

void TextEditorMainWindow::openFile()
{
    QString fileName = QFileDialog::getOpenFileName(this, "Open File", QDir::homePath());
    if (!fileName.isEmpty())
        loadFileToEditor(fileName);
}

void TextEditorMainWindow::openCommonProject()
{
    QString dir = QString("/mnt/D drive/projects/textEditor/project");
    if (!dir.isEmpty())
    {
        fileModel->setRootPath(dir);
        fileExplorer->setRootIndex(fileModel->index(dir));
        globalProgConfigs.get()->SetWorkspacePath(dir.toStdString());
        updateStatus("Workspace opened: " + dir);
    }
}

void TextEditorMainWindow::saveFile() {
    if (QWidget *current = tabWidget->currentWidget()) {
        CodeEditorWidget *editor = qobject_cast<CodeEditorWidget *>(current);
        if (editor) {
            QString filePath = editor->FileURL;
            if (filePath.isEmpty()) {
                filePath = QFileDialog::getSaveFileName(this, "Save File");
                if (filePath.isEmpty())
                    return; // User cancelled
                editor->setProperty("filepath", filePath);
                tabWidget->setTabText(tabWidget->indexOf(editor), QFileInfo(filePath).fileName());
            }

            QFile file(filePath);
            if (file.open(QIODevice::WriteOnly | QIODevice::Text)) {
                QTextStream out(&file);
                out << editor->toPlainText();
                editor->document()->setModified(false);
                updateStatus("File saved: " + filePath);
            } else {
                QMessageBox::warning(this, "Error", "Cannot save file.");
            }
        }
    }
}


void TextEditorMainWindow::openFolder()
{
    QString dir = QFileDialog::getExistingDirectory(this, "Open Folder", QDir::homePath());
    if (!dir.isEmpty())
    {
        fileModel->setRootPath(dir);
        fileExplorer->setRootIndex(fileModel->index(dir));
        globalProgConfigs.get()->SetWorkspacePath(dir.toStdString());
        updateStatus("Workspace opened: " + dir);
    }
}

void TextEditorMainWindow::fileExplorerDoubleClicked(const QModelIndex &index)
{
    QString filePath = fileModel->filePath(index);
    QFileInfo info(filePath);

    if (info.isDir()) {
        // Navigate into the directory
        fileExplorer->setRootIndex(fileModel->index(filePath));
        updateStatus("Navigated to folder: " + filePath);
    } else if (info.isFile()) {
        loadFileToEditor(filePath);
    }
}

void TextEditorMainWindow::loadFileToEditor(const QString &filePath)
{
    // Check if file is already open
    for (int i = 0; i < tabWidget->count(); ++i) {
        CodeEditorWidget *existingEditor = qobject_cast<CodeEditorWidget *>(tabWidget->widget(i));
        if (existingEditor && existingEditor->FileURL == filePath) {
            tabWidget->setCurrentIndex(i);
            return;
        }
    }

    QFile file(filePath);
    if (file.open(QIODevice::ReadOnly | QIODevice::Text))
    {
        CodeEditorWidget *editor = new CodeEditorWidget(this);

        tabWidget->addTab(editor, QFileInfo(filePath).fileName());
        tabWidget->setCurrentWidget(editor);

        editor->setPlainText(file.readAll());
        editor->FileURL = filePath;

        Highlighter *highlighter = new Highlighter(editor->document());
        highlighter->UpdateHighlighting(editor->document(), filePath);

        updateStatus("Opened file: " + filePath);
    }
}

void TextEditorMainWindow::closeTab(int index) {
    QWidget *widget = tabWidget->widget(index);
    CodeEditorWidget *editor = qobject_cast<CodeEditorWidget *>(widget);
    if (editor && maybeSave(editor)) {
        tabWidget->removeTab(index);
        delete widget;
    }
}

bool TextEditorMainWindow::maybeSave(CodeEditorWidget *editor) {
    if (editor->isModified()) {
        QMessageBox::StandardButton ret = QMessageBox::warning(this, "Unsaved Changes",
                                                               "This document has unsaved changes. Save now?",
                                                               QMessageBox::Save | QMessageBox::Discard | QMessageBox::Cancel);
        if (ret == QMessageBox::Save) {
            QString path = editor->FileURL;
            if (!path.isEmpty()) {
                QFile file(path);
                if (file.open(QIODevice::WriteOnly | QIODevice::Text)) {
                    QTextStream out(&file);
                    out << editor->toPlainText();
                    return true;
                }
            }
            return false;
        } else if (ret == QMessageBox::Cancel) {
            return false;
        }
    }
    return true;
}

void TextEditorMainWindow::closeEvent(QCloseEvent *event) {
    for (int i = 0; i < tabWidget->count(); ++i) {
        CodeEditorWidget *editor = qobject_cast<CodeEditorWidget *>(tabWidget->widget(i));
        if (editor && !maybeSave(editor)) {
            event->ignore();
            return;
        }
    }
    event->accept();
}

void TextEditorMainWindow::changeTheme(const QString &theme) {
    applyTheme(theme);
}

void TextEditorMainWindow::applyTheme(const QString &theme) {
    QString styleSheet;
    if (theme == "Dark") {
        styleSheet = "CodeEditorWidget { background-color: #1e1e1e; color: #dcdcdc; }"
                     "QTreeView { background-color: #2e2e2e; color: #dcdcdc; }"
                     "QTabBar::tab { background: #3c3c3c; color: #dcdcdc; padding: 6px; }"
                     "QTabBar::tab:selected { background: #555555; }";
    } else {
        styleSheet = "CodeEditorWidget { background-color: white; color: black; }"
                     "QTreeView { background-color: white; color: black; }"
                     "QTabBar::tab { background: lightgray; color: black; padding: 6px; }"
                     "QTabBar::tab:selected { background: white; }";
    }
    qApp->setStyleSheet(styleSheet);
}


void TextEditorMainWindow::updateStatus(const QString &message)
{
    statusLabel->setText(message);
}

#include "MainEditorWindow.moc"