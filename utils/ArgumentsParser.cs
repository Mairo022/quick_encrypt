using EncryptionTool.models;
using EncryptionTool.services;
using System.Text;

namespace EncryptionTool.utils;

public static class ArgumentsParser
{
    static readonly string[] AllowedArguments = ["action", "path", "password", "group", "delete"];
    
    public static Command GetCommand(string[] args, Lazy<ConfigService> configLazy)
    {
        Command command = new();

        foreach (var arg in args)
        {
            var keyValue = arg.Split(":", 2);

            var key = keyValue[0].ToLower();
            var value = keyValue.Length == 2 ? keyValue[1] : "";

            if (!AllowedArguments.Contains(key)) continue;

            switch (key)
            {
                case "path":
                    foreach (var path in ExtractPaths(value)) command.AddPath(path);
                    break;
                case "password":
                    command.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(value));
                    break;
                case "action":
                    if (!Enum.TryParse(value, true, out CommandAction parsedValue)) break;
                    command.Action = parsedValue;
                    break;
                case "delete":
                    command.Delete = bool.TryParse(value, out var parsedDelete) && parsedDelete;
                    break;
                case "group":
                    var config = configLazy.Value;
                    var group = config.GetGroup(value);
                    
                    if (group == null) break;

                    command.Action = group.Action;
                    command.Delete = group.Delete;
                    foreach (var path in group.Paths) command.AddPath(path);
                    goto AfterParsing;
            }
        }
        AfterParsing:
        
        Array.Clear(args);

        return command;
    }

    public static string[] ExtractPaths(string pathsString)
    {
        var pathsSplit = pathsString.Split(';');
        var paths = new string[pathsSplit.Length];
        var i = 0;
        
        foreach (var path in pathsSplit)
        {
            paths[i] = path;
            i++;
        }

        return paths;
    }
}

