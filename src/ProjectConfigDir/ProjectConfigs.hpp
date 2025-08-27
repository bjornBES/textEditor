#pragma once

#include "../Settings/Editor.hpp"
#include "../Settings/Workspace.hpp"
#include <string>
#include <memory>
#include <unordered_map>
#include <json.hpp>
using json = nlohmann::json;

#define PREFERRED_SEPARATOR std::filesystem::path::preferred_separator

struct Config
{
    Workspace workspace;
    Editor editor;

    unordered_map<std::string, json> extensions; 
};

class ProjectConfigs
{
private:
    string GetThemePath(string projectPath);
    string GetConfigPath(string workSpacePath);
    void ConfigToString(Config* config, string& pBuffer);
public:

    std::string WorkspacePath = "/";
    std::string ExtensionsPath;
    std::string ApplicationDataPath;
    std::string UserConfigPath;
    std::string GlobalConfigPath;
    Config *mainConfig;
    
    ProjectConfigs();
    ~ProjectConfigs();
    
    void SetWorkspacePath(string path);

    void WriteConfigFile(Config *config, bool GetUser);
    void ReadConfigFile(Config *config, bool GetUser);
};

extern std::unique_ptr<ProjectConfigs> globalProgConfigs;