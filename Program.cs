using EncryptionTool.cmd;
using EncryptionTool.utils;

Dictionary<AllowedArguments, string> arguments = ArgumentParser.GetParsedArguments(args);

if (arguments.Count == 3)
{
    ActOnInputs(arguments);
    return;
}

var input = "";

Console.WriteLine("To exit, type 'q'");

while (input != "q")
{

    if (!arguments.ContainsKey(AllowedArguments.action))
    {
        Console.Write("Pick an action (encrypt/decrypt): ");
        input = Console.ReadLine();

        if (Enum.IsDefined(typeof(AllowedArgumentsActions), input.ToLower()))
        {
            arguments.Add(AllowedArguments.action, input);
        }
        else 
        {
            Console.WriteLine("Invalid action");
        }
            
        continue;
    }

    if (!arguments.ContainsKey(AllowedArguments.file))
    {
        Console.Write("Provide filepath: ");
        input = Console.ReadLine();

        if (File.Exists(input))
        { 
            arguments.Add(AllowedArguments.file, input);
        } 
        else
        {
            Console.WriteLine("Could not find the file");
        }

        continue;
    }

    if (!arguments.ContainsKey(AllowedArguments.password))
    {
        Console.Write("Provide file password: ");
        input = Console.ReadLine();

        arguments.Add(AllowedArguments.password, input);

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

static void ActOnInputs(Dictionary<AllowedArguments, string> arguments)
{
    try
    {
        if (AllowedArgumentsActions.encrypt.ToString() == arguments[AllowedArguments.action])
        {
            EncryptionCommands.Encrypt(arguments);
        }
        else if (AllowedArgumentsActions.decrypt.ToString() == arguments[AllowedArguments.action])
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
