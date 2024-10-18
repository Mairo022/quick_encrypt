namespace EncryptionTool.utils;

public enum AllowedArguments
{
    action,
    file,
    password
}

enum AllowedArgumentsActions
{
    encrypt,
    decrypt
}

public static class ArgumentParser
{
    public static Dictionary<AllowedArguments, string> GetParsedArguments(string[] args)
    {
        Dictionary<AllowedArguments, string> arguments = new();

        foreach (string arg in args)
        {
            string[] keyValue = arg.Split(":", 2);

            string key = keyValue[0];
            string value = keyValue.Length == 2 ? keyValue[1] : "";

            if (!Enum.TryParse<AllowedArguments>(key, true, out AllowedArguments parsedKey)) continue;

            if (parsedKey == AllowedArguments.action)
            {
                if (Enum.IsDefined(typeof(AllowedArgumentsActions), value))
                    arguments.Add(parsedKey, value);
            }
            else arguments.Add(parsedKey, value);
        }

        return arguments;
    }
}

