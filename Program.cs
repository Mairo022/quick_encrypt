using EncryptionTool.cmd;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;
using System.Text;

Arguments arguments = ArgumentParser.GetParsedArguments(args);

if (arguments.Action != null 
    && (arguments.File != string.Empty || arguments.Dir != string.Empty) 
    && arguments.Password.Length > 0
    )
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
    var path = inputSplit[1].Trim();

    // Validate action
    if (!Enum.TryParse<AllowedArgumentsActions>(action, true, out AllowedArgumentsActions parsedValue))
    {
        Console.WriteLine("Invalid command");
        continue;
    }
    arguments.Action = parsedValue;

    // Validate path
    if (File.Exists(path))
    {
        arguments.File = path;
    }
    else if (Directory.Exists(path))
    {
        if (!Directory.EnumerateFileSystemEntries(path).Any())
        {
            Console.WriteLine("Directory is empty");
            continue;
        }
        arguments.Dir = path;
    }
    else
    {
        Console.WriteLine($"Invalid filepath or directory: {path}");
        continue;
    }

    // Get key and perform
    Console.Write("Enter password: ");

    arguments.Password = AesCbcEncryptionService.PBKDF2Hash(Encoding.UTF8.GetBytes(Console.ReadLine()));
    ActOnInputs(arguments);
}

static bool ActOnInputs(Arguments arguments)
{
    try
    {
        Console.WriteLine($"Starting {arguments.Action}ion");

        if (AllowedArgumentsActions.encrypt == arguments.Action)
        {
            if (arguments.File != String.Empty) EncryptionCommands.EncryptFile(arguments);
            else if (arguments.Dir != String.Empty) EncryptionCommands.EncryptDirectory(arguments);
        }
        else if (AllowedArgumentsActions.decrypt == arguments.Action)
        {
            EncryptionCommands.DecryptFile(arguments);
        }

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

static void CLIWriteStartText()
{
    Console.WriteLine("Choose an action by typing the action name followed by the path:");
    Console.WriteLine("1. encrypt [path]");
    Console.WriteLine("2. decrypt [path]");
    Console.WriteLine("q - quit the program");
    Console.WriteLine("c - clear console\n");
    Console.WriteLine("Example: encrypt /path/to/directory");
}
