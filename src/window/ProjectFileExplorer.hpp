#pragma once

#include <QWidget>
#include <QTreeView>
#include <QFileSystemModel>
#include <QString>

class ProjectFileExplorer : public QWidget {
    Q_OBJECT

public:
    explicit ProjectFileExplorer(QWidget *parent = nullptr);

    void setRootPath(const QString &path);
    QString currentPath(const QModelIndex &index) const;
    QTreeView *getTreeView() const;

signals:
    void fileActivated(const QString &filePath);

private:
    QTreeView *treeView;
    QFileSystemModel *fileSystemModel;
};