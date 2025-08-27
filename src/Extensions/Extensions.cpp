#include "Extensions.hpp"
#include "../ProjectConfigDir/ProjectConfigs.hpp"

#include <iostream>
#include <filesystem>
#include <optional>
#include <iostream>
#include <fstream>

#define MAXCACHESIZE 20

namespace fs = std::filesystem;

ExtensionManifest manifest;

/*
// Serialization
nlohmann::json ToJSON()
{
    nlohmann::json j = manifest;
    return j;
}

// Deserialization
void FromJSON(const nlohmann::json &j)
{
    manifest = j.get<ExtensionManifest>();
}
*/

// Manual from_json and to_json for Contributes
inline void to_json(json &j, const Contributes &c)
{
    if (c.languages.has_value())
        j["languages"] = c.languages.value();
    if (c.grammars.has_value())
        j["grammars"] = c.grammars.value();
    if (c.themes.has_value())
        j["themes"] = c.themes.value();
}

inline void from_json(const json &j, Contributes &c)
{
    if (j.contains("languages"))
        j.at("languages").get_to(c.languages.emplace());
    if (j.contains("grammars"))
        j.at("grammars").get_to(c.grammars.emplace());
    if (j.contains("themes"))
        j.at("themes").get_to(c.themes.emplace());
}

ExtensionManifest *extensionCache[MAXCACHESIZE];

std::vector<std::string> GetExtensionPaths()
{
    ProjectConfigs *projConfig = globalProgConfigs.get();

    vector<std::string> extPaths = vector<std::string>();

    std::string extPath = projConfig->ExtensionsPath;

    if (!fs::exists(extPath))
    {
        std::cerr << "Path does not exist: " << extPath << "\n";
        exit(EXIT_FAILURE);
    }

    if (!fs::is_directory(extPath))
    {
        std::cerr << "Path is not a directory: " << extPath << "\n";
        exit(EXIT_FAILURE);
    }

    try
    {
        for (const auto &dirEntry : fs::directory_iterator(extPath))
        {
            if (dirEntry.is_directory())
            {
                std::string path = dirEntry.path();
                try
                {
                    for (const auto &entry : fs::directory_iterator(path))
                    {
                        if (entry.is_regular_file() && entry.path().extension() == ".json")
                        {
                            std::cout << entry.path() << std::endl;
                            extPaths.push_back(entry.path());
                        }
                    }
                }
                catch (const fs::filesystem_error &e)
                {
                    std::cerr << "Error: " << e.what() << '\n';
                }
            }
        }
    }
    catch (const fs::filesystem_error &e)
    {
        std::cerr << "Filesystem error: " << e.what() << '\n';
    }
    return extPaths;
}

std::vector<ExtensionManifest> GetAllExtensions()
{
    std::vector<ExtensionManifest> ret;
    vector<std::string> extPaths = GetExtensionPaths();
    try
    {
        for (std::string &path : extPaths)
        {
            std::ifstream infile(path);
            json j;
            infile >> j;

            ExtensionManifest extension = j.template get<ExtensionManifest>();
            ret.push_back(extension);

            infile.close();
        }
    }
    catch (const fs::filesystem_error &e)
    {
        std::cerr << "Filesystem error: " << e.what() << '\n';
    }
    return ret;
}

void GetExtension(std::string fileExt, ExtensionManifest *ret)
{
    ProjectConfigs *projConfig = globalProgConfigs.get();
    Config *config = projConfig->mainConfig;

    vector<std::string> extPaths = GetExtensionPaths();

    int lastOrder = -1;
    try
    {
        for (std::string &path : extPaths)
        {
            std::ifstream infile(path);
            json j;
            infile >> j;

            // convert from JSON: copy each value from the JSON object
            // j["name"].template get<std::string>();
            // j["address"].template get<std::string>();

            ExtensionManifest extension = j.template get<ExtensionManifest>();
            int order = j["order"].template get<int>();

            if (lastOrder >= order)
            {
                continue;
            }

            // Safe unwrap of optional
            std::optional<std::vector<ExtLanguage>> &langs = extension.contributes.languages;
            if (!langs.has_value())
            {
                continue;
            }

            for (const ExtLanguage &lang : langs.value())
            {
                for (const std::string &ext : lang.extensions)
                {
                    if (ext == fileExt)
                    {
                        *ret = extension;
                        lastOrder = order;
                        break;
                    }
                }
            }
            infile.close();
        }
    }
    catch (const fs::filesystem_error &e)
    {
        std::cerr << "Filesystem error: " << e.what() << '\n';
    }
}