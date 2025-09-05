
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using ExtensionLibGlobal;

public static class Extensions
{
    static List<ExtensionProcess> extensionThreads = new List<ExtensionProcess>();
    public static void StartExtensions()
    {
        string[] files = Directory.GetFiles(ProjectConfigs.ExtensionsPath, "Extension.json", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            ExtensionManifest extension = GetExtension(file);
            if (extension != null)
            {
                StartExtension(extension, file);
            }
        }
    }

    static ExtensionManifest GetExtension(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ExtensionManifest>(json);
    }
    public static void StartExtension(ExtensionManifest extension, string path)
    {
        string extPath;
        {
            string fileName = Path.GetFileName(path);
            extPath = path.Replace(fileName, "");
        }
        if (!string.IsNullOrEmpty(extension.Main))
        {
            string projectPath = Path.Combine(extPath, extension.Main);
            string command = $"dotnet run --project \"{projectPath}\" -- {extension.Name}";

            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" -- {extension.Name}",
                UseShellExecute = false,
                CreateNoWindow = false
            };

            // processInfo = StartInTerminal(command, processInfo);

            Process process = Process.Start(processInfo);

            ExtensionProcess extensionProcess = new ExtensionProcess()
            {
                Extension = extension,
                process = process,
            };

            extensionThreads.Add(extensionProcess);
        }
    }

    private static ProcessStartInfo StartInTerminal(string command, ProcessStartInfo processInfo)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            processInfo.FileName = "cmd.exe";
            processInfo.Arguments = $"/c start cmd.exe /k \"{command}\"";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            processInfo.FileName = "osascript";
            processInfo.Arguments = $"-e 'tell application \"Terminal\" to do script \"{command}\"'";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (File.Exists("/usr/bin/gnome-terminal"))
            {
                processInfo.FileName = "gnome-terminal";
                processInfo.Arguments = $"-- bash -ic \"{command}; exec bash\"";
            }
            else if (File.Exists("/usr/bin/xterm"))
            {
                processInfo.FileName = "xterm";
                processInfo.Arguments = $"-hold -e {command}";
            }
            else
            {
                throw new PlatformNotSupportedException("No terminal emulator found");
            }
        }

        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = false;
        processInfo.RedirectStandardInput = true;
        return processInfo;
    }


    public static void CloseAllClients()
    {
        foreach (ExtensionProcess process in extensionThreads)
        {
            WriteToClient(process.Extension.Name, "exit");
        }
    }

    public static bool WriteToClient(string extensionId, string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        bool result = server.SendToClient(extensionId, bytes);

        if (!result)
        {
            DebugWriter.WriteLine("EXT", "Could not send {message} to {extensionId}");
        }
        return result;
    }
    public static bool WriteToClient(string extensionId, byte[] message)
    {
        bool result = server.SendToClient(extensionId, message);

        if (!result)
        {
            DebugWriter.WriteLine("EXT", "Could not send bytes to {extensionId}");
        }
        return result;
    }
    public static bool WritePackageToClient<T>(string extensionId, PackageTypes packageType, T obj)
    {
        string message = "";
        if (packageType != PackageTypes.Message)
        {
            message = JsonSerializer.Serialize(obj);
        }
        else if (packageType == PackageTypes.Message && typeof(T) == typeof(string))
        {
            message = obj.ToString();
        }
        else
        {
            DebugWriter.WriteLine("EXT", "we are fucked");
            DebugWriter.WriteLine("EXT", $"package type is not valid {packageType}");
        }
        bool result = server.SendPackageToClient(extensionId, packageType, message);

        if (!result)
        {
            DebugWriter.WriteLine("EXT", "Could not send bytes to {extensionId}");
        }
        return result;
    }
}

public class ExtensionProcess
{
    public ExtensionManifest Extension;
    public Process process;
}
