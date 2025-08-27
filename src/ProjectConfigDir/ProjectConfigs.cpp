#include "ProjectConfigs.hpp"
#include "../Environment.hpp"
#include "../program.hpp"

#include <iostream>
#include <fstream>
#include <filesystem>

#include "../Settings/Editor.hpp"
#include "../Settings/Workspace.hpp"

#ifndef _WIN32
#include <unistd.h> // for getuid and getpwuid (on Linux)
#include <pwd.h>    // for passwd structure (on Linux)
#endif

#include <string>
#include <vector>
#include <unordered_map>
#include <optional>
#include <json.hpp>
#include <type_traits>
using json = nlohmann::json;

namespace fs = std::filesystem;

std::unique_ptr<ProjectConfigs> globalProgConfigs;

string ProjectConfigs::GetThemePath(string projectPath)
{
    string themesPath = projectPath + PREFERRED_SEPARATOR + "themes";
    cout << "path = " << themesPath << std::endl;
    if (!fs::exists(themesPath))
    {
        cout << "\"" << themesPath << "\" dose not exists" << std::endl;
        fs::create_directory(themesPath);
    }
    return themesPath;
}

string ProjectConfigs::GetConfigPath(string workSpacePath)
{
    string configPath;
    cout << "path.last = " << workSpacePath[workSpacePath.length() - 1] << std::endl;
    if (workSpacePath[workSpacePath.length() - 1] == PREFERRED_SEPARATOR)
    {
        configPath = workSpacePath + ".editor";
    }
    else
    {
        configPath = workSpacePath + PREFERRED_SEPARATOR + ".editor";
    }
    cout << "configPath = " << configPath << std::endl;

    if (!fs::exists(configPath))
    {
        cout << "configPath dose not exists" << std::endl;
        fs::create_directory(configPath);
    }
    return configPath;
}

void ProjectConfigs::ConfigToString(Config *config, string &buffer)
{
    buffer = "{";
    buffer += "here is a config file";
}

void ProjectConfigs::SetWorkspacePath(string path)
{
    WorkspacePath = path;

    string configPath = GetConfigPath(WorkspacePath);

    cout << "configPath = " << configPath << std::endl;
    configPath += "/settings.json";
    cout << "configPath = " << configPath << std::endl;

    if (!fs::exists(configPath))
    {
        UserConfigPath = configPath;
    }
}
ProjectConfigs::ProjectConfigs()
{
    mainConfig = new Config;

    string themesPath;
    string applicationDataPath = GetApplicationDataPath() + PREFERRED_SEPARATOR + projectName;
    ApplicationDataPath = applicationDataPath;
    ExtensionsPath = applicationDataPath + PREFERRED_SEPARATOR + "extensions";
    GlobalConfigPath = ApplicationDataPath + PREFERRED_SEPARATOR + "settings.json";

    cout << "path = " << applicationDataPath << std::endl;
    if (!fs::exists(applicationDataPath))
    {
        cout << "\"" << applicationDataPath << "\" dose not exists" << std::endl;
        fs::create_directory(applicationDataPath);
    }
    if (!fs::exists(GlobalConfigPath))
    {
        ofstream(GlobalConfigPath).close();
        WriteConfigFile(mainConfig, false);
    }
    else
    {
        ReadConfigFile(mainConfig, false);
    }
}

ProjectConfigs::~ProjectConfigs()
{
    // doing this to be 100% sure that they are delete
    delete mainConfig;
}


inline void to_json(json &j, const Workspace &e)
{
    #define GetEntry(field) \
    if (e.field.has_value()) j[#field] = e.field.value();

    GetEntry(test);

    #undef GetEntry
}

inline void from_json(const json &j, Workspace &e)
{
    // Required fields

    #define PutEntry(field, type) \
    if (j.contains(#field) && !j[#field].is_null()) \
    e.field = j.at(#field).get<type>();

    PutEntry(test, std::string);

    #undef PutEntry
}

inline void to_json(json &j, const Editor &e)
{
    #define GetEntry(field) \
    if (e.field.has_value()) j[#field] = e.field.value();

    GetEntry(InsertSpaces);
    GetEntry(TabSize);

    #undef GetEntry
}

inline void from_json(const json &j, Editor &e)
{
    #define PutEntry(field, type) \
    if (j.contains(#field) && !j[#field].is_null()) \
    e.field = j.at(#field).get<type>();

    PutEntry(InsertSpaces, bool);
    PutEntry(TabSize, int);

    #undef PutEntry
}

inline void to_json(json& j, const Config& c)
{
    j = json {
        {"workspace", c.workspace},
        {"editor", c.editor}
    };

    if (!c.extensions.empty())
        j["extensions"] = c.extensions;
}

inline void from_json(const json& j, Config& c) 
{
    j.at("workspace").get_to(c.workspace);
    j.at("editor").get_to(c.editor);

    if (j.contains("extensions") && j["extensions"].is_object())
        c.extensions = j["extensions"].get<unordered_map<string, json>>();
    else
        c.extensions.clear();
}

void ProjectConfigs::WriteConfigFile(Config *config, bool GetUser)
{
    
    std::string path = GetUser ? UserConfigPath : GlobalConfigPath;
    
    ofstream file(path);
    if (!file)
    {
        std::cerr << "Failed to open config file for writing: " << path << std::endl;
        return;
    }
    
    json j = *config;
    file << j.dump(4);
    file.close();
}
void ProjectConfigs::ReadConfigFile(Config *config, bool GetUser)
{
    
    std::string path;
    
    if (GetUser)
    {
        path = UserConfigPath;
    }
    else
    {
        path = GlobalConfigPath;
    }
    
    ifstream file(path);
    json j;

    file >> j;

    *config = j.get<Config>();

    file.close();
}