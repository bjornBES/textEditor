
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ExtensionLibGlobal;

public class PackageManeger
{
    public static async Task OnPackageReceived(string clientId, string packageId, byte[] data)
    {
        DebugWriter.WriteLine("PackageMan", $"Package from {clientId} with ID '{packageId}' received ({data.Length} bytes)");

        PackageTypes packageTypes = PackageId.FromIdToPackageType(packageId);

        if (packageTypes == PackageTypes.Command)
        {
            string json = Encoding.UTF8.GetString(data);
            CommandPackage commandPackage = JsonSerializer.Deserialize<CommandPackage>(json);
            Commands.RunCommand(commandPackage.CommandId, commandPackage.Arguments);
        }
        else if (packageTypes == PackageTypes.Addon)
        {
            string json = Encoding.UTF8.GetString(data);
            AddonPackage addonPackage = JsonSerializer.Deserialize<AddonPackage>(json);
            AddonManager.LoadAddon(clientId, addonPackage);
        }

        await Task.CompletedTask;
    }
}