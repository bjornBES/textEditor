
using System.Text.Json;

public static class Commands
{
    public static readonly List<CommandEntry> CommandEntries = new();
    public static void AddCommand(string commandName, string id, Delegate callback, bool showInCC = true)
    {
        CommandEntries.Add( new CommandEntry(commandName, id, callback, showInCC));
    }
    public static object RunCommand(string id, params object[] args)
    {
        CommandEntry entry = null;
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

        return entry.Callback.DynamicInvoke(args);
    }
}