
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

public static class Extensions
{
    static List<Process> extensionThreads = new List<Process>();
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
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" -- {extension.Name}",
                UseShellExecute = false,
                RedirectStandardInput = true,
                CreateNoWindow = false,
            };

            Process process = Process.Start(processInfo);
            extensionThreads.Add(process);
        }
    }

    public static void CloseAllClients()
    {
        foreach (Process process in extensionThreads)
        {
            process.StandardInput.WriteLine("exit");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
        }
    }

    public static bool WriteToClient(string extensionId, string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        bool result = server.SendToClient(extensionId, bytes);

        if (!result)
        {
            Console.WriteLine($"[EXT] Could not send {message} to {extensionId}");
        }
        return result;
    }
    public static bool WriteToClient(string extensionId, byte[] message)
    {
        bool result = server.SendToClient(extensionId, message);

        if (!result)
        {
            Console.WriteLine($"[EXT] Could not send bytes to {extensionId}");
        }
        return result;
    }
}
