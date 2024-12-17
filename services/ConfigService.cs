using System.Text.RegularExpressions;
using EncryptionTool.models;
using EncryptionTool.utils;


namespace EncryptionTool.services;

public class ConfigService
{
    readonly string _groupsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
        "QuickEncrypt",
        "groups.ini"
        );

    Dictionary<string, GroupItem> Groups { get; } = new();
    
    public ConfigService()
    {
        CreateConfigFiles();
        ReadGroups();
    }

    void CreateConfigFiles()
    {
        FileUtils.CreateFileDirectories(_groupsFilePath);

        if (!File.Exists(_groupsFilePath))
            File.Create(_groupsFilePath).Close();
    }
    
    void ReadGroups()
    {
        GroupItem group = new();
        string[] groupKeys = ["paths", "action", "delete"];
        
        var isGroupFilled = (GroupItem gr) => 
            gr.Action != AllowedArgumentsActions.unknown && gr.Paths.Count != 0;

        using var reader = new StreamReader(File.OpenRead(_groupsFilePath));
        
        var line = string.Empty;
        var section = string.Empty;
        
        while ((line = reader.ReadLine()) != null)
        {
            if (line == string.Empty) continue;

            var isSection = Regex.IsMatch(line, @"\[(.*)\]");
            
            if (isSection)
            {
                if (section != string.Empty && isGroupFilled(group)) Groups.Add(section, group);
                
                section = line[1..^1];
                group = new GroupItem();
                continue;
            }

            var lineSplit = line.Split("=");
            var isKvp = lineSplit.Length == 2;

            if (isKvp)
            {
                var key = lineSplit[0];
                var value = lineSplit[1];
                
                if (!groupKeys.Contains(key)) continue;

                switch (key)
                {
                    case "action":
                    {
                        if (!Enum.TryParse(value, true, out AllowedArgumentsActions parsedAction)) continue;
                        group.Action = parsedAction;
                        break;
                    }
                    case "paths":
                    {
                        var paths = value.Split(";");
                        group.Paths.AddRange(paths);
                        break;
                    }
                    case "delete":
                        group.Delete = bool.TryParse(value, out var delete) && delete;
                        break;
                }
            }
            
        }
        
        if (section != string.Empty && isGroupFilled(group)) Groups.Add(section, group);
    }

    bool WriteGroupsToFile()
    {
        var tempFilePath = _groupsFilePath + ".tmp";

        try
        {
            using var writer = new StreamWriter(File.OpenWrite(tempFilePath));

            foreach (var group in Groups)
            {
                var key = $"[{group.Key}]";
                var action = $"action={group.Value.Action}";
                var paths = "paths=" + string.Join(";", group.Value.Paths);
                var delete = $"delete={group.Value.Delete}";

                writer.WriteLine(key);
                writer.WriteLine(action);
                writer.WriteLine(paths);
                writer.WriteLine(delete);
                writer.WriteLine();
            }
            
            writer.Close();
            
            if (FileUtils.IsFileInUse(new FileInfo(_groupsFilePath)) ||
                FileUtils.IsFileInUse(new FileInfo(tempFilePath)))
            {
                return false;
            }
            
            File.Delete(_groupsFilePath);
            File.Move(tempFilePath, _groupsFilePath);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        finally
        {
            if (!FileUtils.IsFileInUse(new FileInfo(tempFilePath)))
                File.Delete(tempFilePath);
        }
    }

    public GroupCommand CliFormatCommand(string input)
    {
        if (input == "g --list") return new GroupCommand{ Action = GroupAction.List };
        
        var inputSplit = input.Split(" ");
        if (inputSplit.Length < 2) return new GroupCommand();

        string[] properties = ["paths", "action", "delete"];
        string[] flags = ["--delete", "--info"];
        
        var name = inputSplit[1];
        
        var groupExists = Groups.TryGetValue(name, out var group);
        var groupCommand = new GroupCommand();

        if (inputSplit.Length is 2 or 3 && !groupExists) return groupCommand;
        if (inputSplit.Length is 2 && group == null) return groupCommand;
        
        if (groupExists && group != null)
        {
            groupCommand.Name = name;
            groupCommand.Process = group.Action;
            groupCommand.Paths.AddRange(group.Paths);
            groupCommand.Delete = group.Delete;
        }
        
        switch (inputSplit.Length)
        {
            case 2:
                groupCommand.Action = GroupAction.Execute;
                break;
            case 3:
                var isFlag = inputSplit[2].StartsWith("--");

                if (isFlag)
                {
                    var flag = inputSplit[2];
                    if (!flags.Contains(flag)) break;
                    
                    if (flag == "--delete") groupCommand.Action = GroupAction.Delete;
                    if (flag == "--info") groupCommand.Action = GroupAction.Info;

                    break;
                }
                
                var isKvp = inputSplit[2].Contains('=');

                if (isKvp)
                {
                    var kvpSplit = inputSplit[2].Split("=");
                    var property = kvpSplit[0];
                    var value = kvpSplit[1];
                    
                    if (!properties.Contains(property)) break;

                    if ("action" == property && Enum.TryParse(value, true, out AllowedArgumentsActions parsedKvpAction))
                    {
                        groupCommand.Action = GroupAction.Save;
                        groupCommand.Process = parsedKvpAction;
                    }
                    
                    if ("delete" == property && bool.TryParse(value, out var parsedDelete))
                    {
                        groupCommand.Action = GroupAction.Save;
                        groupCommand.Delete = parsedDelete;
                    }

                    if ("paths" == property)
                    {
                        groupCommand.Action = GroupAction.Save;
                        groupCommand.Paths.AddRange(value.Split(';'));
                    }
                }

                break;
            case 4 or 5:
                var action = inputSplit[2];
                var paths = inputSplit[3];
                var delete = inputSplit.ElementAtOrDefault(4) ?? bool.FalseString;

                if (!Enum.TryParse(action, true, out AllowedArgumentsActions parsedAction)) break;

                groupCommand.Action = GroupAction.Save;
                groupCommand.Process = parsedAction;
                groupCommand.Name = name;
                groupCommand.Paths.AddRange(paths.Split(';'));
                groupCommand.Delete = bool.TryParse(delete, out var parsedDelete2) && parsedDelete2;
                
                break;
        }


        return groupCommand;
    }
    
    public void CliExample()
    {
        Console.WriteLine();
        Console.WriteLine("Available group commands are: ");
        Console.WriteLine("1. Execute a group: g [group name]");
        Console.WriteLine("2. Create a group: g [group name] [action] [paths] [delete]");
        Console.WriteLine("3. Modify a group: g [group name] [property]=value");
        Console.WriteLine("4. Delete a group: g [group name] --delete");
        Console.WriteLine("5. View group info: g [group name] --info");
        Console.WriteLine("6. View all groups: g --list");
    }
    
    public bool SaveGroup(GroupCommand groupCommand)
    {
        var groupExists = Groups.TryGetValue(groupCommand.Name, out var group);

        try
        {
            if (!groupExists || (group == null && groupCommand.Paths.Count > 0))
            {
                Groups.Add(groupCommand.Name, new GroupItem
                {
                    Action = groupCommand.Process, 
                    Paths = groupCommand.Paths,
                    Delete = groupCommand.Delete
                });

                return WriteGroupsToFile();
            }

            if (group == null) return false;
            
            group.Action = groupCommand.Process;
            
            if (groupCommand.Paths.Count > 0)
            {
                group.Paths.Clear();
                group.Paths.AddRange(groupCommand.Paths);
                group.Delete = groupCommand.Delete;
            }

            return WriteGroupsToFile();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public GroupItem? GetGroup(string groupName)
    {
        return Groups.GetValueOrDefault(groupName);
    }

    public void PrintGroup(string groupName)
    {
        Console.WriteLine();
        Console.WriteLine($"Group: {groupName}");
        Console.WriteLine($"Action: {Groups[groupName].Action}");
        Console.WriteLine($"Delete original files: {Groups[groupName].Delete}");
        Console.WriteLine("Paths:");
        
        foreach (var path in Groups[groupName].Paths)
        {
            Console.WriteLine(path);
        }
    }

    public void PrintAllGroups()
    {
        foreach (var group in Groups)
        {
            Console.WriteLine(group.Key);
        }
    }

    public bool DeleteGroup(string groupName)
    {
        return Groups.Remove(groupName) && WriteGroupsToFile();
    }

}