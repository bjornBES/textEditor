#pragma once

#include <window/Highlighter.hpp>

#include <string>
#include <vector>
#include <QSyntaxHighlighter>
#include <QTextCharFormat>
#include <QRegularExpression>

struct Color
{
    int R;
    int G;
    int B;
    int A;
};

struct RuleSyntax
{
    std::string scope;
    std::string pattern;
    Color fgcolor;
    Color bgcolor;
    int priority;
};

class HighlightRuleGenerator
{
private:
    
public:
    HighlightRuleGenerator();
    ~HighlightRuleGenerator();

    std::vector<RuleSyntax> GetLanguageRules(std::string fileExtension);

};

