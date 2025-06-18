#pragma once

#include "../Settings/Editor.hpp"
#include "../Settings/Workspace.hpp"
#include <string>
#include <memory>
using std::string;

#define PREFERRED_SEPARATOR std::filesystem::path::preferred_separator

typedef struct
{
    Workspace_t *WorkspaceSettings;
    Editor_t *EditorSettings;
} Config;

class ProjectConfigs
{
private:
    /* data */

    
    /* func */
    
    string GetThemePath(string projectPath);
    string GetConfigPath(string workSpacePath);
    void ConfigToString(Config* config, string& pBuffer);
public:
    Config *mainConfig;
    void SetWorkspacePath(string path);

    ProjectConfigs();
    ~ProjectConfigs();

    void WriteConfigFile(Config *config);
    void ReadConfigFile(Config *config);
};

extern std::unique_ptr<ProjectConfigs> globalProgConfigs;