#pragma once

#include <QSyntaxHighlighter>
#include <QTextCharFormat>
#include <QRegularExpression>

class Highlighter : public QSyntaxHighlighter
{
    Q_OBJECT

public:
    explicit Highlighter(QTextDocument *parent = nullptr);

    void UpdateHighlighting(QTextDocument *parent, QString path);
    void UpdateWithCurSyntaxHighlighting(QTextDocument *parent);

    struct HighlightRule
    {
        QString name;
        QRegularExpression pattern;
        QTextCharFormat format;
        QColor color;
        bool enabled = true;
        int priority = 0;
        bool multiLine = false;
        QRegularExpression endPattern;
    };
protected:
    void highlightBlock(const QString &text) override;

private:
    QVector<HighlightRule> highlightingRules;
};