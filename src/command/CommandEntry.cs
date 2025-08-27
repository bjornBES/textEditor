[Serializable]
public class CommandEntry
{
    public string DisplayName { get; set; }
    public string CommandId { get; set; }
    public Delegate Callback { get; set; }
    public Type[] Types{ get; set; }
    public bool ShowInCC { get; set; }

    public CommandEntry(string name, string id, Delegate callback, bool showInCC = true)
    {
        ShowInCC = showInCC;
        DisplayName = name;
        CommandId = id;
        Callback = callback;
        var paramTypes = callback.Method.GetParameters().Select(p => p.ParameterType).ToArray();
        Types = paramTypes;
    }
}