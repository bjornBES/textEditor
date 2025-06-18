#pragma once

#include <string>
#include <vector>
#include <optional>
#include <variant>
#include "json.hpp"


// Scope can be either a string or a list of strings
using ScopeType = std::variant<std::string, std::vector<std::string>>;

struct Settings {
    std::optional<std::string> foreground;
    std::optional<std::string> background;
    std::optional<std::string> fontStyle;
};

// Settings
NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(Settings, foreground, background, fontStyle)

struct TokenColor {
    std::optional<std::string> name;
    ScopeType scope;
    Settings settings;
};

struct ThemeFile {
    std::string name;
    std::string type;
    std::vector<TokenColor> tokenColors;
};
// ThemeFile
NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ThemeFile, name, type, tokenColors)

void GetTheme(std::string themeFile, ThemeFile *ret);