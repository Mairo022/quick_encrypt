using EncryptionTool.cmd;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;
using System.Text;

Arguments arguments = ArgumentParser.GetParsedArguments(args);

if (arguments.Action != null && 
    arguments.File != string.Empty && 
    arguments.Password.Length > 0)
{
    ActOnInputs(arguments);
    return;
}

var input = "";

Console.WriteLine("To exit, type 'q'");

while (input != "q")
{

    if (arguments.Action == null)
    {
        Console.Write("Pick an action (encrypt/decrypt): ");
        input = Console.ReadLine();

        if (Enum.TryParse<AllowedArgumentsActions>(input, true, out AllowedArgumentsActions parsedInput))
        {
            arguments.Action = parsedInput;
        }
        else 
        {
            Console.WriteLine("Invalid action");
        }
            
        continue;
    }

    if (arguments.File == String.Empty)
    {
        Console.Write("Provide filepath: ");
        input = Console.ReadLine();

        if (File.Exists(input))
        { 
            arguments.File = input;
        } 
        else
        {
            Console.WriteLine("Could not find the file");
        }

        continue;
    }

    if (arguments.Password.Length == 0)
    {
        Console.Write("Provide file password: ");

        arguments.Password = EncryptionService.PBKDF2Hash(Encoding.UTF8.GetBytes(Console.ReadLine()));

        continue;
    }

    Console.Write("Proceed?(y/n): ");
    input = Console.ReadLine();

    if (input == "y")
    {
        ActOnInputs(arguments);
        break;
    }
    else if (input == "n")
    {
        break;
    }
}

static void ActOnInputs(Arguments arguments)
{
    try
    {
        if (AllowedArgumentsActions.encrypt == arguments.Action)
        {
            EncryptionCommands.Encrypt(arguments);
        }
        else if (AllowedArgumentsActions.decrypt == arguments.Action)
        {
            EncryptionCommands.Decrypt(arguments);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }

    return;
}
