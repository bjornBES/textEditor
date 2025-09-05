
using System.Text.Json;
using ExtensionLibGlobal;

public static class Commands
{
    public static readonly List<CommandEntry> CommandEntries = new();
    public static void AddCommand(string commandName, string id, Delegate callback, bool showInCC = true)
    {
        CommandEntries.Add(new CommandEntry(commandName, id, callback, showInCC));
    }
    public static void AddCommand(string commandName, string id, string clientId, Type[] types, bool showInCC = true)
    {
        CommandEntries.Add(new CommandEntry(commandName, id, clientId, types, showInCC));
    }
    public static object RunCommand(string id, params object[] args)
    {
        CommandEntry entry = null;
        if (!(args == null || args.Length == 0))
        {
            if ((args != null || args.Length == 0) && args[0] is JsonElement)
            {
                FromJsonElement(ref args);
            }
        }
        foreach (CommandEntry k in CommandEntries)
        {
            if (k.CommandId == id)
            {
                if (k.Types.Length == args.Length)
                {
                    if (k.Types.Zip(args, (expected, actual) => expected.IsAssignableFrom(actual.GetType())).All(match => match))
                    {
                        entry = k;
                        break;
                    }
                }
            }
        }

        if (entry == null)
        {
            return null;
        }

        if (entry.IsClientCommand)
        {
            DebugWriter.WriteLine("Commands", $"Running client command '{entry.CommandId}' for client '{entry.ClientId}'");

            CommandPackage commandPackage = new CommandPackage()
            {
                CommandId = entry.CommandId,
                Arguments = args
            };
            Extensions.WritePackageToClient(entry.ClientId, PackageTypes.Command, commandPackage);

            return null; // Or some appropriate response
        }

        return entry.Callback.DynamicInvoke(args);
    }

    private static void FromJsonElement(ref object[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == null) continue;

            if (args[i] is JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.String:
                        args[i] = element.GetString();
                        break;
                    case JsonValueKind.Number:
                        if (element.TryGetInt32(out int intValue))
                        {
                            args[i] = intValue;
                        }
                        else if (element.TryGetDouble(out double doubleValue))
                        {
                            args[i] = doubleValue;
                        }
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        args[i] = element.GetBoolean();
                        break;
                    case JsonValueKind.Array:
                        // Handle arrays if necessary
                        break;
                    case JsonValueKind.Object:
                        // Handle objects if necessary
                        break;
                    case JsonValueKind.Null:
                        args[i] = null;
                        break;
                }
            }
        }

    }
}