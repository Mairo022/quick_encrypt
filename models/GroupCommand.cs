namespace EncryptionTool.models;

public class GroupCommand : Command
{
    public GroupAction GroupAction { get; set; } = GroupAction.Invalid;
    public string Name { get; set; } = string.Empty;
}
