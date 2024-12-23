using System.Security;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;

namespace EncryptionTool.cmd;

public class Cli(Command command, ConfigService? config)
{
    ConfigService? _config = config;

    public void Run()
    {
        if (ExecuteStartupCommand(command)) return;
        command.Clear();
        
        _config = null ?? new ConfigService();
        
        WriteStartText();
        while (true)
        {
            Console.Write("\nEnter command: ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (input == "q") break;

            if (input == "c")
            {
                Console.Clear();
                WriteStartText();
                continue;
            }
            
            if (input.StartsWith('g'))
            {

                if (input == "g")
                {
                    _config.CliExample();
                    continue;
                }

                var groupCommand = _config.CliFormatGroupCommand(input);
                ExecuteGroupCommand(groupCommand);
                
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
            if (!Enum.TryParse(action, true, out CommandAction parsedValue))
            {
                Console.WriteLine("Invalid command");
                continue;
            }
            command.Action = parsedValue;

            // Validate path
            foreach (var path in ArgumentsParser.ExtractPaths(pathString))
            {
                command.AddPath(path);
            }
            
            if (command.Paths.Count == 0)
            {
                continue;
            }
            
            // Get key and perform
            var key = GetHashedKeyFromConsole();

            if (key.Length == 0)
            {
                Console.WriteLine("Something went wrong with the password");
                continue;
            }

            command.Password = key;
            ActOnInputs(command);
        }
    }

    void ExecuteGroupCommand(GroupCommand groupCommand)
    {
        if (_config == null)
        {
            Console.WriteLine("Error: Config is null when executing group command");
            return;
        }
        
        switch (groupCommand.GroupAction)
        {
            case GroupAction.Execute:
                foreach (var path in groupCommand.Paths) command.AddPath(path);

                if (command.Paths.Count == 0)
                {
                    Console.WriteLine("No valid path provided");
                    return;
                }
                
                var keyGroup = GetHashedKeyFromConsole();

                if (keyGroup.Length == 0)
                {
                    Console.WriteLine("Something went wrong with the password");
                    return;
                }
                
                command.Action = groupCommand.Action;
                command.Password = keyGroup;
                command.Delete = groupCommand.Delete;
                
                ActOnInputs(command);
                break;
            
            case GroupAction.Save:
                Console.WriteLine(_config.SaveGroup(groupCommand)
                    ? $"Group {groupCommand.Name} is saved"
                    : $"Group {groupCommand.Name} failed to save");
                break;
            
            case GroupAction.Delete:
                Console.WriteLine(_config.DeleteGroup(groupCommand.Name)
                    ? $"Group {groupCommand.Name} is removed"
                    : $"Group {groupCommand.Name} is not removed");
                break;
            
            case GroupAction.Info:
                _config.PrintGroup(groupCommand.Name);
                break;
            
            case GroupAction.List:
                _config.PrintAllGroups();
                break;
            
            default:
                Console.WriteLine("Something is wrong with the group");
                break;
        }
    }

    static bool ExecuteStartupCommand(Command command)
    {
        if (command is not { Action: not CommandAction.unknown, Paths.Count: > 0 }) return false;
        if (command.Password.Length > 0 && ActOnInputs(command)) return true;
        
        if (command.Password.Length == 0)
        {
            var key = GetHashedKeyFromConsole();
            command.Password = key;

            if (key.Length == 0) Console.WriteLine("Something went wrong with the password");
            if (key.Length > 0 && ActOnInputs(command)) return true;
        }

        return false;
    }
    
    static bool ActOnInputs(Command command)
    {
        try
        {
            Console.WriteLine($"Starting {command.Action}ion");

            if (CommandAction.encrypt == command.Action) Commands.Encrypt(command);
            if (CommandAction.decrypt == command.Action) Commands.Decrypt(command);

            Console.WriteLine($"{StringUtils.FirstLetterToUpper(command.Action.ToString()) ?? ""}ion successful");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed {command.Action}ing:");
            Console.WriteLine(ex.ToString());

            return false;
        }
        finally
        {
            command.Clear();
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

    static void WriteStartText()
    {
        Console.WriteLine("Available actions:");
        Console.WriteLine("• encrypt [path]");
        Console.WriteLine("• decrypt [path]");
        Console.WriteLine("• g - create a group");
        Console.WriteLine("• c - clear console");
        Console.WriteLine("• q - quit the program");
    }
}