using System.Security;
using EncryptionTool.cmd;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;

var arguments = ArgumentParser.GetParsedArguments(args);

if (arguments is { Action: not null, Path.Count: > 0, Password.Length: > 0 })
{
    if (ActOnInputs(arguments))
    {
        return;
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
    Console.WriteLine("c - clear console\n");
    Console.WriteLine("Example: encrypt /path/to/directory");
}
