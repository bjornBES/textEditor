#include "ProjectConfigs.hpp"
#include "../Environment.hpp"
#include "../program.hpp"

#include <iostream>
#include <fstream>
#include <filesystem>
#include <string>

#ifndef _WIN32
#include <unistd.h> // for getuid and getpwuid (on Linux)
#include <pwd.h>    // for passwd structure (on Linux)
#endif

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
    buffer += "here is a config file";
}

void ProjectConfigs::SetWorkspacePath(string path)
{
    mainConfig->WorkspaceSettings = new Workspace_t;
    mainConfig->EditorSettings = new Editor_t;

    mainConfig->WorkspaceSettings->WorkspacePath = path;

    string themesPath;
    string applicationDataPath = GetApplicationDataPath() + PREFERRED_SEPARATOR + projectName;
    mainConfig->EditorSettings->ApplicationDataPath = applicationDataPath;
    mainConfig->EditorSettings->ExtensionsPath = applicationDataPath + PREFERRED_SEPARATOR + "extensions";

    cout << "path = " << applicationDataPath << std::endl;
    if (!fs::exists(applicationDataPath))
    {
        cout << "\"" << applicationDataPath << "\" dose not exists" << std::endl;
        fs::create_directory(applicationDataPath);
    }

    string configPath = GetConfigPath(mainConfig->WorkspaceSettings->WorkspacePath);
    
    cout << "configPath = " << configPath << std::endl;
    configPath += "/settings.txt";
    cout << "configPath = " << configPath << std::endl;
    
    if (!fs::exists(configPath))
    {
        std::ofstream(configPath).close();
    }
    
    string buffer;
    ConfigToString(mainConfig, buffer);
    WriteConfigFile(mainConfig);
    cout << "buffer = " << buffer << std::endl;
    
    std::ofstream ConfigFile(configPath);
    ConfigFile << buffer;
    ConfigFile.close();
}
ProjectConfigs::ProjectConfigs()
{
    mainConfig = new Config;
}

ProjectConfigs::~ProjectConfigs()
{
    // doing this to be 100% sure that they are delete
    delete mainConfig->WorkspaceSettings;
    delete mainConfig->EditorSettings;
    delete mainConfig;
}

void ProjectConfigs::WriteConfigFile(Config *config)
{

}
void ProjectConfigs::ReadConfigFile(Config *config)
{
    config = mainConfig;
}