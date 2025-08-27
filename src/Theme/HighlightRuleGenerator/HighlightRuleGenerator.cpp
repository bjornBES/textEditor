#include "HighlightRuleGenerator.hpp"
#include "Extensions/Extensions.hpp"
#include "ProjectConfigDir/ProjectConfigs.hpp"

#include "Theme/ThemeGenerator/Theme.hpp"
#include "Theme/GrammarGenerator/Grammar.hpp"

#include <iostream>
#include <string>
#include <vector>
#include <optional>

std::vector<RuleSyntax> ResolvePatterns(const Grammar &grammar, const ThemeFile &themeFile);

HighlightRuleGenerator::HighlightRuleGenerator()
{
}

HighlightRuleGenerator::~HighlightRuleGenerator()
{
}

std::vector<RuleSyntax> HighlightRuleGenerator::GetLanguageRules(std::string fileExtension)
{
    std::vector<RuleSyntax> ret{};

    ExtensionManifest extension;
    GetExtension(fileExtension, &extension);

    ProjectConfigs *projConfig = globalProgConfigs.get();
    Config *config = projConfig->mainConfig;

    if (!extension.contributes.themes.has_value())
    {
        std::cout << "extension not good with theme" << std::endl;
        exit(EXIT_FAILURE);
    }

    if (!extension.contributes.grammars.has_value())
    {
        std::cout << "extension not good with grammar" << std::endl;
        exit(EXIT_FAILURE);
    }

    vector<ExtTheme> themes = extension.contributes.themes.value();
    vector<ExtGrammar> grammars = extension.contributes.grammars.value();
    std::string basePath = projConfig->ExtensionsPath + PREFERRED_SEPARATOR + extension.name + PREFERRED_SEPARATOR;
    for (const ExtTheme &theme : themes)
    {
        std::string themePath = basePath + theme.path;
        std::cout << "themePath = " << themePath << std::endl;

        ThemeFile themeFile;

        GetTheme(themePath, &themeFile);
        for (const ExtGrammar &extGrammar : grammars)
        {
            std::string grammarPath = basePath + extGrammar.path;

            Grammar garmmar;

            GetGrammar(grammarPath, &garmmar);

            std::vector<RuleSyntax> temp = ResolvePatterns(garmmar, themeFile);
            for (RuleSyntax &rule : temp)
            {
                if (ret.size() == 0)
                {
                    ret.push_back(rule);
                    continue;
                }
                for (size_t i = 0; i < ret.size(); i++)
                {
                    RuleSyntax retRule = ret.at(i);
                    if (retRule.scope == rule.scope && retRule.pattern == rule.pattern)
                    {
                        continue;
                    }
                    break;
                }
                ret.push_back(rule);
            }
        }
    }

    return ret;
}

// Utility to parse hex string of two characters
uint8_t parseHexByte(const std::string& str, size_t offset) {
    return static_cast<uint8_t>(std::stoi(str.substr(offset, 2), nullptr, 16));
}

// Unified parser
Color parseColor(const std::string& str) {
        Color color{0, 0, 0, 255}; // Default alpha is 255

    if (str.empty()) {
        throw std::invalid_argument("Empty color string");
    }

    if (str[0] == '#') {
        if (str.length() == 9) {
            // #RRGGBBAA
            color.R = parseHexByte(str, 1);
            color.G = parseHexByte(str, 3);
            color.B = parseHexByte(str, 5);
            color.A = parseHexByte(str, 7);
        } else if (str.length() == 7) {
            // #RRGGBB
            color.R = parseHexByte(str, 1);
            color.G = parseHexByte(str, 3);
            color.B = parseHexByte(str, 5);
            color.A = 255;
        } else {
            throw std::invalid_argument("Invalid hex color format");
        }
    } else {
        // Assume comma-separated decimal: R,G,B[,A]
        std::stringstream ss(str);
        std::string token;
        std::vector<int> values;

        while (std::getline(ss, token, ',')) {
            values.push_back(std::stoi(token));
        }

        if (values.size() < 3 || values.size() > 4) {
            throw std::invalid_argument("Invalid decimal color format");
        }

        color.R = static_cast<uint8_t>(values[0]);
        color.G = static_cast<uint8_t>(values[1]);
        color.B = static_cast<uint8_t>(values[2]);
        color.A = values.size() == 4 ? static_cast<uint8_t>(values[3]) : 255;
    }

    return color;
}

std::vector<RuleSyntax> ResolvePatterns(const Grammar &grammar, const ThemeFile &themeFile)
{
    int priority = 0;
    std::vector<RuleSyntax> rules;

    auto matchScopeToSettings = [&](const std::string &scope) -> std::optional<Settings>
    {
        for (const auto &token : themeFile.tokenColors)
        {
            if (std::holds_alternative<std::string>(token.scope))
            {
                if (scope == std::get<std::string>(token.scope))
                {
                    return token.settings;
                }
            }
            else if (std::holds_alternative<std::vector<std::string>>(token.scope))
            {
                const auto &scopes = std::get<std::vector<std::string>>(token.scope);
                if (std::find(scopes.begin(), scopes.end(), scope) != scopes.end())
                {
                    return token.settings;
                }
            }
        }
        return std::nullopt;
    };

    std::function<void(const Pattern &)> processPattern;
    processPattern = [&](const Pattern &pattern)
    {
        if (pattern.include.has_value())
        {
            const std::string &includeStr = pattern.include.value();

            // Handle includes like "#key"
            if (!includeStr.empty() && includeStr[0] == '#')
            {
                std::string key = includeStr.substr(1);
                auto it = grammar.repository.find(key);
                if (it != grammar.repository.end())
                {
                    processPattern(it->second); // recursively process included pattern
                }
                return;
            }

            // Future: handle "include": "source.js" (external grammar)
            return;
        }

        // If pattern has a scope, resolve it against the theme
        if (pattern.scope.has_value())
        {
            RuleSyntax rule;
            rule.scope = pattern.scope.value();
            rule.pattern = pattern.match.value_or(""); // or .begin if it's a begin-end pattern
            rule.priority = priority;

            auto settings = matchScopeToSettings(rule.scope);
            rule.fgcolor = settings && settings->foreground ? parseColor(settings->foreground.value()) : parseColor("#00000000");
            rule.bgcolor = settings && settings->background ? parseColor(settings->background.value()) : parseColor("#00000000");

            rules.push_back(rule);
            priority++;
        }

        // Also process any nested patterns
        if (pattern.patterns.has_value())
        {
            for (const auto &subPattern : pattern.patterns.value())
            {
                processPattern(subPattern);
            }
        }
    };

    // Process main patterns
    for (const auto &pattern : grammar.patterns)
    {
        processPattern(pattern);
    }

    // Optionally process repository directly
    for (const auto &[key, pattern] : grammar.repository)
    {
        processPattern(pattern);
    }

    return rules;
}
