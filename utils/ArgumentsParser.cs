using EncryptionTool.models;
using EncryptionTool.services;
using System.Text;

namespace EncryptionTool.utils;

public static class ArgumentParser
{
    public static Arguments GetParsedArguments(string[] args)
    {
        Arguments arguments = new();

        foreach (string arg in args)
        {
            var keyValue = arg.Split(":", 2);

            var key = keyValue[0].ToLower();
            var value = keyValue.Length == 2 ? keyValue[1] : "";

            if (!arguments.AllowedArguments.Contains(key)) continue;

            switch (key)
            {
                case "file":
                    arguments.File = value;
                    break;
                case "dir":
                    arguments.Dir = value;
                    break;
                case "password":
                    arguments.Password = AesCbcEncryptionService.Pbkdf2HashBytes(Encoding.UTF8.GetBytes(value));
                    break;
                case "action":
                    if (!Enum.TryParse<AllowedArgumentsActions>(value, true, out AllowedArgumentsActions parsedValue)) break;
                    arguments.Action = parsedValue;
                    break;
            }
        }
        
        Array.Clear(args);

        return arguments;
    }
}

