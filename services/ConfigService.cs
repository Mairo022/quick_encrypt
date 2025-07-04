﻿using System.Text.RegularExpressions;
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

    readonly Dictionary<string, GroupItem> _groups = new();
    public IReadOnlyDictionary<string, GroupItem> Groups => _groups;
    
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
        string[] groupKeys = ["paths", "action", "delete", "overwrite"];
        
        var isGroupFilled = (GroupItem gr) => 
            gr.Action != CommandAction.unknown && gr.Paths.Count != 0;

        using var reader = new StreamReader(File.OpenRead(_groupsFilePath));
        
        var line = string.Empty;
        var section = string.Empty;
        
        while ((line = reader.ReadLine()) != null)
        {
            if (line == string.Empty) continue;

            var isSection = Regex.IsMatch(line, @"\[(.*)\]");
            
            if (isSection)
            {
                if (section != string.Empty && isGroupFilled(group)) _groups.Add(section, group);
                
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
                        if (!Enum.TryParse(value, true, out CommandAction parsedAction)) continue;
                        group.Action = parsedAction;
                        break;
                    }
                    case "paths":
                    {
                        group.AddPaths(value.Split(";"));
                        break;
                    }
                    case "overwrite":
                        group.Overwrite = bool.TryParse(value, out var overwrite) && overwrite;
                        break;
                    case "delete":
                        group.Delete = bool.TryParse(value, out var delete) && delete;
                        break;
                }
            }
            
        }
        
        if (section != string.Empty && isGroupFilled(group)) _groups.Add(section, group);
    }

    bool WriteGroupsToFile()
    {
        var tempFilePath = _groupsFilePath + ".tmp";

        try
        {
            using var writer = new StreamWriter(File.OpenWrite(tempFilePath));

            foreach (var group in _groups)
            {
                var key = $"[{group.Key}]";
                var action = $"action={group.Value.Action}";
                var paths = "paths=" + string.Join(";", group.Value.Paths);
                var overwrite = $"overwrite={group.Value.Overwrite}";
                var delete = $"delete={group.Value.Delete}";

                writer.WriteLine(key);
                writer.WriteLine(action);
                writer.WriteLine(paths);
                writer.WriteLine(overwrite);
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
    
    public GroupCommand CliFormatGroupCommand(string input)
    {
        if (input == "g --list") return new GroupCommand{ GroupAction = GroupAction.List };
        
        var inputSplit = input.Split(" ");
        if (inputSplit.Length < 2) return new GroupCommand();

        string[] properties = ["paths", "action", "overwrite", "delete"];
        string[] flags = ["--delete", "--info"];
        
        var name = inputSplit[1];
        
        var groupExists = _groups.TryGetValue(name, out var group);
        var groupCommand = new GroupCommand();

        if (inputSplit.Length is 2 or 3 && !groupExists) return groupCommand;
        if (inputSplit.Length is 2 && group == null) return groupCommand;
        
        if (groupExists && group != null)
        {
            groupCommand.Name = name;
            groupCommand.Action = group.Action;
            groupCommand.AddPaths(group.Paths);
            groupCommand.Delete = group.Delete;
            groupCommand.Overwrite = group.Overwrite;
        }
        
        switch (inputSplit.Length)
        {
            case 2:
                groupCommand.GroupAction = GroupAction.Execute;
                break;
            case 3:
                var isFlag = inputSplit[2].StartsWith("--");

                if (isFlag)
                {
                    var flag = inputSplit[2];
                    if (!flags.Contains(flag)) break;
                    
                    if (flag == "--delete") groupCommand.GroupAction = GroupAction.Delete;
                    if (flag == "--info") groupCommand.GroupAction = GroupAction.Info;

                    break;
                }
                
                var isKvp = inputSplit[2].Contains('=');

                if (isKvp)
                {
                    var kvpSplit = inputSplit[2].Split("=");
                    var property = kvpSplit[0];
                    var value = kvpSplit[1];
                    
                    if (!properties.Contains(property)) break;
                    
                    switch (property)
                    {
                        case "action" when Enum.TryParse(value, true, out CommandAction parsedKvpAction):
                            groupCommand.Action = parsedKvpAction;
                            break;
                        case "paths":
                            groupCommand.AddPaths(value.Split(';'));
                            break;
                        case "overwrite" when bool.TryParse(value, out var parsedOverwrite):
                            groupCommand.Overwrite = parsedOverwrite;
                            break;
                        case "delete" when bool.TryParse(value, out var parsedDelete):
                            groupCommand.Delete = parsedDelete;
                            break;
                    }

                    groupCommand.GroupAction = GroupAction.Save;
                }

                break;
            case 4 or 5 or 6:
                var action = inputSplit[2];
                var paths = inputSplit[3];
                var delete = inputSplit.ElementAtOrDefault(4) ?? bool.FalseString;
                var overwrite = inputSplit.ElementAtOrDefault(5) ?? bool.FalseString;

                if (!Enum.TryParse(action, true, out CommandAction parsedAction)) break;

                groupCommand.GroupAction = GroupAction.Save;
                groupCommand.Action = parsedAction;
                groupCommand.Name = name;
                groupCommand.AddPaths(paths.Split(';'));
                groupCommand.Overwrite = bool.TryParse(overwrite, out var parsedOverwrite2) &&  parsedOverwrite2;
                groupCommand.Delete = bool.TryParse(delete, out var parsedDelete2) && parsedDelete2;
                
                break;
        }


        return groupCommand;
    }
    
    public void CliExample()
    {
        Console.WriteLine();
        Console.WriteLine("Available group commands: ");
        Console.WriteLine("1. Execute a group: g [group name]");
        Console.WriteLine("2. Create a group: g [group name] [action] [paths] [delete] [overwrite]");
        Console.WriteLine("3. Modify a group: g [group name] [property]=value");
        Console.WriteLine("4. Delete a group: g [group name] --delete");
        Console.WriteLine("5. View group info: g [group name] --info");
        Console.WriteLine("6. View all groups: g --list");
    }

    public void AddGroup(Dictionary<string, GroupItem> group)
    {
        _groups.Add(group.First().Key, group.First().Value);
        WriteGroupsToFile();
    }
    
    public bool SaveGroup(GroupCommand groupCommand)
    {
        var groupExists = _groups.TryGetValue(groupCommand.Name, out var group);

        try
        {
            if (!groupExists || (group == null && groupCommand.Paths.Count > 0))
            {
                group = new GroupItem{
                    Action = groupCommand.Action, 
                    Delete = groupCommand.Delete,
                    Overwrite = groupCommand.Overwrite,
                };
                group.AddPaths(groupCommand.Paths);
                _groups[groupCommand.Name] = group;

                return WriteGroupsToFile();
            }

            if (group == null) return false;
            
            group.Action = groupCommand.Action;
            
            if (groupCommand.Paths.Count > 0)
            {
                group.ClearPaths();
                group.AddPaths(groupCommand.Paths);
                group.Delete = groupCommand.Delete;
                group.Overwrite = groupCommand.Overwrite;
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
        return _groups.GetValueOrDefault(groupName);
    }

    public void PrintGroup(string groupName)
    {
        Console.WriteLine();
        Console.WriteLine($"{groupName}");
        Console.WriteLine($"{new string('-', groupName.Length)}");
        Console.WriteLine($"Action: {_groups[groupName].Action}");
        Console.WriteLine($"Overwrite: {_groups[groupName].Overwrite}");
        Console.WriteLine($"Delete original: {_groups[groupName].Delete}");
        Console.Write("Paths: ");

        var i = 0;
        foreach (var path in _groups[groupName].Paths)
        {
            if (i == 0) Console.WriteLine(path);
            else Console.WriteLine("       " + path);
            i++;
        }
    }

    public void PrintAllGroups()
    {
        foreach (var group in _groups)
        {
            Console.WriteLine(group.Key);
        }
    }

    public bool DeleteGroup(string groupName)
    {
        return _groups.Remove(groupName) && WriteGroupsToFile();
    }

}