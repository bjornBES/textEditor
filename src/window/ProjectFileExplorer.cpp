#include "ProjectFileExplorer.hpp"
#include <QVBoxLayout>

ProjectFileExplorer::ProjectFileExplorer(QWidget *parent) : QWidget(parent) { 
    QVBoxLayout *layout = new QVBoxLayout(this);
    treeView = new QTreeView(this);
    fileSystemModel = new QFileSystemModel(this);

    fileSystemModel->setRootPath(QDir::rootPath());
    fileSystemModel->setFilter(QDir::NoDotAndDotDot | QDir::AllDirs | QDir::Files);

    treeView->setModel(fileSystemModel);
    treeView->setColumnWidth(0, 250);
    layout->addWidget(treeView);
    setLayout(layout);

    connect(treeView, &QTreeView::activated, this, [=](const QModelIndex &index) {
        if (!fileSystemModel->isDir(index)) {
            emit fileActivated(fileSystemModel->filePath(index));
        }
    });
}

void ProjectFileExplorer::setRootPath(const QString &path) {
    treeView->setRootIndex(fileSystemModel->index(path));
}

QString ProjectFileExplorer::currentPath(const QModelIndex &index) const {
    return fileSystemModel->filePath(index);
}

QTreeView *ProjectFileExplorer::getTreeView() const {
    return treeView;
}