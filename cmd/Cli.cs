﻿using System.Security;
using System.Text;
using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;

namespace EncryptionTool.cmd;

public class Cli(Command command, ConfigService config)
{
    public void Run()
    {
        if (ExecuteStartupCommand(command)) return;
        command.Clear();
        
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
                    config.CliExample();
                    continue;
                }

                var groupCommand = config.CliFormatGroupCommand(input);
                ExecuteGroupCommand(groupCommand);
                
                continue;
            }

            if (input.StartsWith("delete"))
            {
                var path = input[("delete".Length +1)..];
                command.AddPath(path);
                command.Action = CommandAction.delete;

                if (command.Paths.Count == 0)
                {
                    command.Clear();
                    continue;
                }
                
                ExecuteDeleteCommand(command);
                continue;
            }
            
            var inputSplit = FormatInput(input.Trim().Split(" "));

            if (inputSplit.Length < 2)
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
            
            if (command.Paths.Count == 0) continue;

            if (inputSplit.Contains("--delete")) command.Delete = true;
            if (inputSplit.Contains("--overwrite")) command.Overwrite = true;
            
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
                command.Overwrite = groupCommand.Overwrite;
                
                ActOnInputs(command);
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
    }

    static bool ExecuteStartupCommand(Command command)
    {
        if (command is not { Action: not CommandAction.unknown, Paths.Count: > 0 }) return false;
        if (command.Password.Length > 0 && ActOnInputs(command)) return true;
        
        if (command.Action == CommandAction.delete)
        {
            return ExecuteDeleteCommand(command);
        }
        
        if (command.Password.Length == 0)
        {
            var key = GetHashedKeyFromConsole();
            command.Password = key;

            if (key.Length == 0) Console.WriteLine("Something went wrong with the password");
            if (key.Length > 0 && ActOnInputs(command)) return true;
        }

        return false;
    }

    static bool ExecuteDeleteCommand(Command command)
    {
        try
        {
            Console.WriteLine($"Deleting {command.Paths.First()}");
            Commands.Delete(command);
            Console.WriteLine("Deleted successfully");
            
            return true;
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to delete");
            return false;
        }
        finally
        {
            command.Clear();
        }
    }
    
    static bool ActOnInputs(Command command)
    {
        try
        {
            Console.WriteLine($"Starting {command.Action}ion");

            if (CommandAction.encrypt == command.Action) Commands.Encrypt(command);
            if (CommandAction.decrypt == command.Action) Commands.Decrypt(command);

            Console.WriteLine($"{StringUtils.FirstLetterToUpper(command.Action.ToString())}ion successful");

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
    
    static string[] FormatInput(string[] inputArr)
    {
        var hasMarks = inputArr[1].StartsWith('"');

        if (!hasMarks) return inputArr;

        var formattedInput = new List<string>();
        var stringBuilder = new StringBuilder();

        for (var i = 2; i < inputArr.Length; i++)
        {
            if (!inputArr[i].EndsWith('"')) continue;
            
            stringBuilder.Append(inputArr[1][1..]);
            
            for (var j = 2; j < i; j++)
            {
                stringBuilder.Append(' ');
                stringBuilder.Append(inputArr[j]);
            }
            
            stringBuilder.Append(' ');
            stringBuilder.Append(inputArr[i][..^1]);
            
            formattedInput.Add(inputArr[0]);
            formattedInput.Add(stringBuilder.ToString());
            formattedInput.AddRange(inputArr[(i + 1)..]);
        }

        return formattedInput.ToArray();
    }

    static void WriteStartText()
    {
        Console.WriteLine("Available actions:");
        Console.WriteLine("• encrypt [path] [--delete] [--overwrite]");
        Console.WriteLine("• decrypt [path] [--delete] [--overwrite]");
        Console.WriteLine("• delete [path]");
        Console.WriteLine("• g - show group commands");
        Console.WriteLine("• c - clear console");
        Console.WriteLine("• q - quit");
    }
}