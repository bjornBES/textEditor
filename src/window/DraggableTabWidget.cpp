#include "DraggableTabWidget.hpp"
#include <QVBoxLayout>
#include <QStackedWidget>
#include <QTabBar>
#include <QTabWidget>


DraggableTabWidget::DraggableTabWidget(QWidget *parent)
    : QTabWidget(parent)
{
    setTabPosition(QTabWidget::West);  // Change as needed (North, South, East, West)
    setMovable(true);
    setTabsClosable(false);
}

void DraggableTabWidget::addPanel(QWidget *widget, const QString &name, const QIcon &icon)
{
    addTab(widget, icon, name);
}

void DraggableTabWidget::removePanel(const QString &name)
{
    for (int i = 0; i < count(); ++i)
    {
        if (tabText(i) == name)
        {
            QWidget *widget = this->widget(i);
            removeTab(i);
            widget->deleteLater();
            break;
        }
    }
}

void DraggableTabWidget::showPanel(const QString &name)
{
    for (int i = 0; i < count(); ++i)
    {
        if (tabText(i) == name)
        {
            setCurrentIndex(i);
            break;
        }
    }
}

QWidget* DraggableTabWidget::currentPanel() const
{
    return currentWidget();
}
