namespace EncryptionTool.models;

public class GroupItem
{
    public AllowedArgumentsActions Action { get; set; }
    public List<string> Paths { get; init; } = [];
    public bool Delete { get; set; }
}