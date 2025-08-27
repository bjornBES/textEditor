#pragma once

#include <QPalette>
#include <QColor>
#include <QApplication>

struct EditorThemePalette
{
    QColor background;
    QColor foreground;
    struct EditorlineNumbers
    {
        std::optional<QColor> background;
        std::optional<QColor> foreground;
    };
};
struct ThemePalette
{
    EditorThemePalette editor;
};

class WindowTheme
{
private:
    QPalette *currentPalette;
    
public:
    QPalette *PaletteCache;
    WindowTheme();
    ~WindowTheme();
};

