namespace EncryptionTool.models;

public class GroupCommand
{
    public GroupAction Action { get; set; } = GroupAction.Invalid;
    public AllowedArgumentsActions Process { get; set; } = AllowedArgumentsActions.unknown;
    public string Name { get; set; } = string.Empty;
    public bool Delete { get; set; }
    public List<string> Paths { get; } = [];

}
