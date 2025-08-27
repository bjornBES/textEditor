#include <iostream>
#include <fstream>
#include "window/Highlighter.hpp"
#include "Environment.hpp"
#include "ProjectConfigDir/ProjectConfigs.hpp"
#include "Extensions/Extensions.hpp"
#include "window/MainEditorWindow.hpp"

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

#include "json.hpp"

using json = nlohmann::json;

int main(int argc, char *argv[])
{
    InitEnvironment();

    globalProgConfigs = std::make_unique<ProjectConfigs>();

    QApplication app(argc, argv);
    TextEditorMainWindow editor;
    editor.show();
    int ret = app.exec();
    return ret;
}

#include "program.moc"