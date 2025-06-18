#pragma once

#include <QTabWidget>
#include <QIcon>

class DraggableTabWidget : public QTabWidget
{
    Q_OBJECT

public:
    explicit DraggableTabWidget(QWidget *parent = nullptr);

    void addPanel(QWidget *widget, const QString &name, const QIcon &icon = QIcon());
    void removePanel(const QString &name);
    void showPanel(const QString &name);
    QWidget* currentPanel() const;
};