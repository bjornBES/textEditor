
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ExtensionLibGlobal;

public class PackageManeger
{
    public static async Task OnPackageReceived(string clientId, string packageId, byte[] data)
    {
        Console.WriteLine($"[Main] Package from {clientId} with ID '{packageId}' received ({data.Length} bytes)");

        if (packageId == "cmd")
        {
            string json = Encoding.UTF8.GetString(data);
            CommandPackage commandPackage = JsonSerializer.Deserialize<CommandPackage>(json);
            Commands.RunCommand(commandPackage.CommandId, commandPackage.Arguments);
        }

        // Optional: decode if it's text
            try
            {
                string decoded = Encoding.UTF8.GetString(data);
                Console.WriteLine($"[Main] Content (UTF-8): {decoded}");
            }
            catch
            {
                Console.WriteLine("[Main] Could not decode as UTF-8 text.");
            }
        // Optional: store or forward...
        // await File.WriteAllBytesAsync($"packages/{clientId}_{packageId}.bin", data);
    }
}