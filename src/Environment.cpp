#include "Environment.hpp"

#include <cstdlib>  // for getenv
#include <string>

#ifndef _WIN32
#include <unistd.h> // for getuid and getpwuid (on Linux)
#include <pwd.h>    // for passwd structure (on Linux)
#endif

std::string LocalApplicationData;
std::string ApplicationData;

void InitEnvironment()
{
#ifdef _WIN32
    const char *appdata = std::getenv("APPDATA");
    if (appdata)
        ApplicationData = std::string(appdata);
    else
        return ""; // fallback or error handling
#else
    // Try XDG_CONFIG_HOME
    const char *xdg_config = std::getenv("XDG_CONFIG_HOME");
    if (xdg_config)
    {
        ApplicationData = std::string(xdg_config);
    }
    else
    {
        // fallback to ~/.config
        const char *home = std::getenv("HOME");
        if (home)
        {
            ApplicationData = std::string(home) + "/.config";
        }
        else
        {
            // If HOME is not set, try to get it from passwd
            struct passwd *pwd = getpwuid(getuid());
            if (pwd)
            {
                ApplicationData = std::string(pwd->pw_dir) + "/.config";
            }
        }
    }
#endif
}

std::string GetLocalApplicationDataPath()
{
    return LocalApplicationData;
}

std::string GetApplicationDataPath()
{
    return ApplicationData;
}
