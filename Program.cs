using System.Security;
using EncryptionTool.cmd;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;

var arguments = ArgumentParser.GetParsedArguments(args, out var config);
config = null ?? new ConfigService();

if (arguments is { Action: not null, Path.Count: > 0 })
{
    if (arguments.Password.Length > 0 && ActOnInputs(arguments)) return;
    if (arguments.Password.Length == 0)
    {
        var key = GetHashedKeyFromConsole();
        arguments.Password = key;

        if (key.Length == 0) Console.WriteLine("Something went wrong with the password");
        if (key.Length > 0 && ActOnInputs(arguments)) return;
    }
}
arguments.Clear();

CLIWriteStartText();

string input;
var cliActive = true;

while (cliActive)
{
    Console.Write("\nEnter command: ");
    input = Console.ReadLine() ?? "";

    if (input == "q")
    { 
        cliActive = false;
        continue;
    }

    if (input == "c")
    {
        Console.Clear();
        CLIWriteStartText();
        continue;
    }
    
    if (input.StartsWith('g'))
    {
        input = input.Trim();
        
        if (input == "g")
        {
            config.CliExample();
            continue;
        }

        var groupCommand = config.CliFormatCommand(input);

        switch (groupCommand.Action)
        {
            case GroupAction.Execute:
                foreach (var path in groupCommand.Paths) arguments.AddPath(path);

                if (arguments.Path.Count == 0)
                {
                    Console.WriteLine("No valid path provided");
                    continue;
                }
                
                var keyGroup = GetHashedKeyFromConsole();

                if (keyGroup.Length == 0)
                {
                    Console.WriteLine("Something went wrong with the password");
                    continue;
                }
                
                arguments.Action = groupCommand.Process;
                arguments.Password = keyGroup;
                arguments.Delete = groupCommand.Delete;
                
                ActOnInputs(arguments);
                break;
            
            case GroupAction.Save:
                Console.WriteLine(config.SaveGroup(groupCommand)
                    ? $"Group {groupCommand.Name} is saved"
                    : $"Group {groupCommand.Name} failed to save");
                break;
            
            case GroupAction.Delete:
                Console.WriteLine(config.DeleteGroup(groupCommand.Name)
                    ? $"Group {groupCommand.Name} is removed"
                    : $"Group {groupCommand.Name} is not removed");
                break;
            
            case GroupAction.Info:
                config.PrintGroup(groupCommand.Name);
                break;
            
            case GroupAction.List:
                config.PrintAllGroups();
                break;
            
            default:
                Console.WriteLine("Something is wrong with the group");
                break;
        }
        
        continue;
    }

    var inputSplit = input?.Trim().Split(" ", 2);

    if (inputSplit == null || inputSplit.Length != 2)
    {
        Console.WriteLine("Invalid command");
        continue;
    }

    var action = inputSplit[0];
    var pathString = inputSplit[1].Trim();

    // Validate action
    if (!Enum.TryParse(action, true, out AllowedArgumentsActions parsedValue))
    {
        Console.WriteLine("Invalid command");
        continue;
    }
    arguments.Action = parsedValue;

    // Validate path
    foreach (var path in ArgumentParser.ExtractPaths(pathString))
    {
        arguments.AddPath(path);
    }
    
    if (arguments.Path.Count == 0)
    {
        continue;
    }
    
    // Get key and perform
    var key = GetHashedKeyFromConsole();

    if (key.Length == 0)
    {
        Console.WriteLine("something went wrong with the password");
        continue;
    }

    arguments.Password = key;
    ActOnInputs(arguments);
}

return;

static bool ActOnInputs(Arguments arguments)
{
    try
    {
        Console.WriteLine($"Starting {arguments.Action}ion");

        if (AllowedArgumentsActions.encrypt == arguments.Action) Commands.Encrypt(arguments);
        if (AllowedArgumentsActions.decrypt == arguments.Action) Commands.Decrypt(arguments);

        Console.WriteLine($"{StringUtils.FirstLetterToUpper(arguments.Action.ToString()) ?? ""}ion successful");

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed {arguments.Action}ing:");
        Console.WriteLine(ex.ToString());

        return false;
    }
    finally
    {
        arguments.Clear();
    }
}

static byte[] GetHashedKeyFromConsole()
{
    Console.Write("Enter password: ");
    SecureString password = new();
    ConsoleKeyInfo key;

    do
    {
        key = Console.ReadKey(true);

        if (!char.IsControl(key.KeyChar))
        {
            password.AppendChar(key.KeyChar);
            Console.Write("*");
        }
        else
        {
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.RemoveAt(password.Length - 1);
                Console.Write("\b \b");
            }
        }
    }
    while (key.Key != ConsoleKey.Enter);
    
    Console.Write("\n");

    return AesCbcEncryptionService.Pbkdf2HashSecureString(password);
}

static void CLIWriteStartText()
{
    Console.WriteLine("Choose an action by typing the action name followed by the path:");
    Console.WriteLine("1. encrypt [path]");
    Console.WriteLine("2. decrypt [path]");
    Console.WriteLine("q - quit the program");
    Console.WriteLine("g - create a group");
    Console.WriteLine("c - clear console\n");
    Console.WriteLine("Example: encrypt /path/to/directory");
}
