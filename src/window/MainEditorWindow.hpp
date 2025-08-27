#pragma once

#include "window/Highlighter.hpp"
#include "window/Editors/CodeEditorWidget.hpp"
#include "window/ProjectFileExplorer.hpp"
#include "window/WindowTheme.hpp"

#include <QApplication>
#include <QMainWindow>
#include <QTextEdit>
#include <QFileDialog>
#include <QFile>
#include <QTextStream>
#include <QMessageBox>
#include <QMenuBar>
#include <QMenu>
#include <QLabel>
#include <QTreeView>
#include <QFileSystemModel>
#include <QSplitter>
#include <QVBoxLayout>
#include <QTabBar>
#include <QTabWidget>
#include <QStatusBar>
#include <QDockWidget>
#include <QAction>
#include <QToolBar>
#include <QCloseEvent>
#include <QComboBox>
#include <QListWidget>
#include <QPlainTextEdit>
#include <QFileInfo>
#include <QTextDocument>

extern Highlighter *highlighter;

class TextEditorMainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit TextEditorMainWindow(QWidget *parent = nullptr);

private:
    // Layout and UI
    void setupLayout();
    void setupMenu();
    void setupPanels();
    void setupStyling();

    // Editor
    void openFileInEditor(const QString &path);
    void openFileInTab(const QString &path);
    void openFile();
    void saveFile();

    // File Explorer
    void setupFileExplorer(const QString &projectPath = QString());
    void onFileExplorerActivated(const QModelIndex &index);

    // Panels
    QTabWidget *leftPanel;
    QTabWidget *editorPanel;
    QTabWidget *rightPanel;
    QTabWidget *bottomPanel;
    QSplitter *horizontalSplitter;
    QToolBar *topBar;

    // File Explorer
    QDockWidget *fileExplorerDock = nullptr;
    ProjectFileExplorer *fileExplorerWidget;

    // Status bar
    QStatusBar *statusBar;

    WindowTheme *windowTheme;

};
