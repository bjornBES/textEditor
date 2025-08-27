#include "Theme.hpp"

#include <string>
#include <vector>
#include <optional>
#include <variant>
#include <fstream>

using json = nlohmann::json;
// ScopeType variant
inline void from_json(const json& j, ScopeType& s) {
    if (j.is_string())
        s = j.get<std::string>();
    else if (j.is_array())
        s = j.get<std::vector<std::string>>();
    else
        throw json::type_error::create(302, "Invalid type for scope", nullptr);
}

inline void to_json(json& j, const ScopeType& s) {
    std::visit([&j](auto&& val) {
        j = val;
    }, s);
}

// TokenColor
inline void from_json(const json& j, TokenColor& t) {
    if (j.contains("name"))
        t.name = j.at("name").get<std::string>();
    from_json(j.at("scope"), t.scope);
    j.at("settings").get_to(t.settings);
}

inline void to_json(json& j, const TokenColor& t) {
    if (t.name.has_value())
        j["name"] = t.name.value();
    to_json(j["scope"], t.scope);
    j["settings"] = t.settings;
}

inline void from_json(const nlohmann::json& j, Settings& s) {
    if (j.contains("foreground")) s.foreground = j.at("foreground").get<std::string>();
    if (j.contains("background")) s.background = j.at("background").get<std::string>();
    if (j.contains("fontStyle"))  s.fontStyle = j.at("fontStyle").get<std::string>();
}

// TokenColor
inline void from_json(const json& j, ThemeFile& t)
{
    t.name = j.at("name").get<std::string>();
    t.type = j.at("type").get<std::string>();
    if (j.contains("colors"))
    {
        t.colors = j.at("colors").get<std::unordered_map<std::string, std::string>>();
    }
    if (j.contains("semanticHighlighting"))
    {
        t.semanticHighlighting = j.at("semanticHighlighting").get<bool>();
    }
    t.tokenColors = j.at("tokenColors").get<std::vector<TokenColor>>();
}

inline void to_json(json& j, const ThemeFile& t)
{
    j["name"] = t.name;
    j["type"] = t.type;
    j["colors"] = t.colors;
    j["semanticHighlighting"] = t.semanticHighlighting;
    j["tokenColors"] = t.tokenColors;
}

void GetTheme(std::string themeFile, ThemeFile *ret)
{
    std::ifstream inFile(themeFile);
    json j = json::parse(inFile);

    *ret = j.get<ThemeFile>();

    inFile.close();
}
