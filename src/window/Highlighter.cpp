#include "Highlighter.hpp"
#include "MainEditorWindow.hpp"
#include "ProjectConfigDir/ProjectConfigs.hpp"
#include "Theme/HighlightRuleGenerator/HighlightRuleGenerator.hpp"
#include <QColor>
#include <vector>

Highlighter::Highlighter(QTextDocument *parent)
    : QSyntaxHighlighter(parent)
{
}

void Highlighter::UpdateHighlighting(QTextDocument *parent, QString path)
{
    highlightingRules.clear();
    QString extension = "." + QFileInfo(path).suffix();

    HighlightRuleGenerator highlightRuleGen;

    std::vector<RuleSyntax> ruleSyntaxRule = highlightRuleGen.GetLanguageRules(extension.toStdString());

    for (const RuleSyntax &syntaxRule : ruleSyntaxRule)
    {
        HighlightRule rule;
        QTextCharFormat keywordFormat;

        Color fgcolor = syntaxRule.fgcolor;
        Color bgcolor = syntaxRule.bgcolor;

        keywordFormat.setForeground(QColor(fgcolor.R, fgcolor.G, fgcolor.B, fgcolor.A));
        keywordFormat.setBackground(QColor(bgcolor.R, bgcolor.G, bgcolor.B, bgcolor.A));
        rule.pattern = QRegularExpression(QString::fromStdString(syntaxRule.pattern));
        rule.priority = syntaxRule.priority;
        rule.format = keywordFormat;

        std::cout << "syntaxRule = {" << syntaxRule.scope << ", " << syntaxRule.pattern << ", (" << fgcolor.A << ", " << fgcolor.G << ", " << fgcolor.B << ", " << fgcolor.A << "), (" << bgcolor.A << ", " << bgcolor.G << ", " << bgcolor.B << ", " << bgcolor.A << ")" << "}" << std::endl;

        highlightingRules.append(rule);
    }

    rehighlight();
}

void Highlighter::highlightBlock(const QString &text)
{
    for (const HighlightRule &rule : qAsConst(highlightingRules))
    {
        QRegularExpressionMatchIterator matchIterator = rule.pattern.globalMatch(text);
        while (matchIterator.hasNext())
        {
            QRegularExpressionMatch match = matchIterator.next();
            setFormat(match.capturedStart(), match.capturedLength(), rule.format); // ← CORRECTED
        }
    }
}
