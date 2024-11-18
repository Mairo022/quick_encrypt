using EncryptionTool.models;
using EncryptionTool.services;
using System.Text;

namespace EncryptionTool.utils;

public static class ArgumentParser
{
    public static Arguments GetParsedArguments(string[] args)
    {
        Arguments arguments = new();

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
            }
        }
        
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

