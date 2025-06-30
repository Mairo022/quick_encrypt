namespace guiAvalonia.Models;

public class GroupsListItem(string name, string actions)
{
    public string Name { get; set; } = name;
    public string Actions { get; set; } = actions;
}