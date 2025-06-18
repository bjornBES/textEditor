#pragma once

#include "Highlighter.hpp"
#include "window/Editors/CodeEditorWidget.hpp"

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

class DraggableTabWidget;

class TextEditorMainWindow : public QMainWindow
{
    Q_OBJECT

public:
    TextEditorMainWindow(QWidget *parent = nullptr);

protected:
    void closeEvent(QCloseEvent *event) override;

private slots:
    void openFile();
    void saveFile();
    // void saveAsFile();
    void openFolder();
    void fileExplorerDoubleClicked(const QModelIndex &index);
    void updateStatus(const QString &message);
    void openCommonProject();
    void closeTab(int index);
    void changeTheme(const QString &theme);

private:
    void setupUi();
    void createActions();
    void setupConnections();
    void loadFileToEditor(const QString &filePath);
    bool maybeSave(CodeEditorWidget *editor);
    void applyTheme(const QString &theme);

    void addBottomPanelWidget(QWidget *widget, const QIcon &icon, const QString &label);
    void addLeftPanelWidget(QWidget *widget, const QIcon &icon, const QString &label);
    void addRightPanelWidget(QWidget *widget, const QIcon &icon, const QString &label);


    QMenu *fileMenu;
    QTabWidget *tabWidget;
    QLabel *statusLabel;
    QComboBox *themeSelector;

    DraggableTabWidget *editorPanel;
    
    QDockWidget *leftDock;
    QDockWidget *rightDock;
    QDockWidget *bottomDock;

    DraggableTabWidget *leftPanelTabs;
    DraggableTabWidget *rightPanelTabs;
    DraggableTabWidget *bottomPanelTabs;

    QFileSystemModel *fileModel;
    QTreeView *fileExplorer;

    QPlainTextEdit *terminalOutput;
    QListWidget *logOutput;

    QAction *actionOpenFile;
    QAction *actionSaveFile;
    QAction *actionOpenFolder;
    QAction *actionExit;
    QAction *actionOpenDefault;
};
