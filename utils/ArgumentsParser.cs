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
            string[] keyValue = arg.Split(":", 2);

            string key = keyValue[0].ToLower();
            string value = keyValue.Length == 2 ? keyValue[1] : "";

            if (!arguments.AllowedArguments.Contains(key)) continue;

            switch (key)
            {
                case "file":
                    arguments.File = value;
                    break;
                case "password":
                    arguments.Password = EncryptionService.PBKDF2Hash(Encoding.UTF8.GetBytes(value));
                    break;
                case "action":
                    if (!Enum.TryParse<AllowedArgumentsActions>(value, true, out AllowedArgumentsActions parsedValue)) break;
                    arguments.Action = parsedValue;
                    break;
            }
        }

        return arguments;
    }
}

