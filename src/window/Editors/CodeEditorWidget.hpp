#pragma once


#include <QPlainTextEdit>
#include <QObject>
#include <QWidget>
#include <QString>

class LineNumberArea;  // Forward declaration

class CodeEditorWidget : public QPlainTextEdit
{
    Q_OBJECT

public:
    QString FileURL;

    explicit CodeEditorWidget(QWidget *parent = nullptr);

    void lineNumberAreaPaintEvent(QPaintEvent *event);
    int lineNumberAreaWidth();

    // File operations
    bool openFile(const QString &filePath);
    bool saveFile(const QString &filePath);
    bool save();
    QString currentFile() const;
    bool isModified() const;

protected:
    void resizeEvent(QResizeEvent *event) override;

private slots:
    void updateLineNumberAreaWidth(int newBlockCount);
    void highlightCurrentLine();
    void updateLineNumberArea(const QRect &rect, int dy);
    void documentWasModified();

private:
    QWidget *lineNumberArea;
};