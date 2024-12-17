using EncryptionTool.models;
using EncryptionTool.services;
using System.Text;

namespace EncryptionTool.utils;

public static class ArgumentParser
{
    public static Arguments GetParsedArguments(string[] args, out ConfigService? config)
    {
        Arguments arguments = new();
        config = null;

        foreach (var arg in args)
        {
            var keyValue = arg.Split(":", 2);

            var key = keyValue[0].ToLower();
            var value = keyValue.Length == 2 ? keyValue[1] : "";

            if (!arguments.AllowedArguments.Contains(key)) continue;

            switch (key)
            {
                case "path":
                    foreach (var path in ExtractPaths(value)) arguments.AddPath(path);
                    break;
                case "password":
                    arguments.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(value));
                    break;
                case "action":
                    if (!Enum.TryParse(value, true, out AllowedArgumentsActions parsedValue)) break;
                    arguments.Action = parsedValue;
                    break;
                case "delete":
                    arguments.Delete = bool.TryParse(value, out var parsedDelete) && parsedDelete;
                    break;
                case "group":
                    config = new ConfigService();
                    var group = config.GetGroup(value);
                    
                    if (group == null) break;

                    arguments.Action = group.Action;
                    arguments.Delete = group.Delete;
                    foreach (var path in group.Paths) arguments.AddPath(path);
                    
                    goto AfterParsing;
            }
        }
        AfterParsing:
        
        Array.Clear(args);

        return arguments;
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

