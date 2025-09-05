using System.Text.Json;
using ExtensionLibGlobal;

public static class AddonManager
{
    public static void LoadAddon(string clientId, AddonPackage addonPackage)
    {
        DebugWriter.WriteLine("AddonManager", $"Loading addon: type {addonPackage.FeatureTypes}");

        object[] data = (object[])addonPackage.Data;
        if (addonPackage.FeatureTypes == FeatureTypes.command)
        {
            string commandName = data[0].ToString();
            string commandId = data[1].ToString();
            Type[] types;

            if (data[2] is JsonElement element && element.ValueKind == JsonValueKind.Array)
            {
                // Assuming your JSON contains type names (e.g. ["System.String","System.Int32"])
                string[] typeNames = JsonSerializer.Deserialize<string[]>(element.GetRawText());
                types = Array.ConvertAll(typeNames, Type.GetType);
            }
            else if (data[2] is Type[] castTypes)
            {
                // Already a Type[]
                types = castTypes;
            }
            else
            {
                throw new InvalidOperationException("Invalid type data in AddonPackage.");
            }
            Commands.AddCommand(commandName, commandId, clientId, types, true);
            DebugWriter.WriteLine("AddonManager", "Command '{commandName}' with ID '{commandId}' added.");
        }
    }

}