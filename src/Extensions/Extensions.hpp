#pragma once
#include <string>
#include <vector>
#include <optional>

#include "json.hpp"
using json = nlohmann::json;

// ----------------- Inner Structs ------------------

struct ExtLanguage {
    std::string LanguageId;
    std::vector<std::string> extensions;
};

NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ExtLanguage, LanguageId, extensions)

struct ExtGrammar {
    std::string Language;
    std::string scopeName;
    std::string path;
};


NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ExtGrammar, Language, scopeName, path)

struct ExtTheme {
    std::string name;
    std::string path;
};

NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ExtTheme, name, path)

struct Contributes {
    std::optional<std::vector<ExtLanguage>> languages;
    std::optional<std::vector<ExtGrammar>> grammars;
    std::optional<std::vector<ExtTheme>> themes;
};


NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(Contributes, languages, grammars, themes)

// ----------------- Root Struct ------------------

struct ExtensionManifest {
    std::string name;
    std::string displayName;
    std::string description;
    std::string version;
    int order;
    std::string main;
    Contributes contributes;
};

NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ExtensionManifest, name, displayName, description, version, order, main, contributes)

void GetExtension(std::string fileExt, ExtensionManifest *ret);
std::vector<ExtensionManifest> GetAllExtensions();

