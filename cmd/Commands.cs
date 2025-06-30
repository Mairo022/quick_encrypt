using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;

namespace EncryptionTool.cmd;

public static class Commands
{
    public static void Encrypt(Command command)
    {
        foreach (var path in command.Paths)
        {
            if (File.Exists(path))
            {
                AesCbcEncryptionService.EncryptFile(path, command.Password, command.Overwrite);
                
                if (command.Delete) FileUtils.OverwriteAndDeleteFile(new FileInfo(path));
            }
            else if (Directory.Exists(path))
            {
                AesCbcEncryptionService.EncryptDirectory(path, command.Password, command.Overwrite);
                
                if (command.Delete) FileUtils.OverwriteAndDeleteDirectory(new DirectoryInfo(path));
            }
            else Console.WriteLine($"COMMANDS: Invalid path: {path}");
        }
    }

    public static void Decrypt(Command command)
    {
        foreach (var path in command.Paths)
        {
            if (File.Exists(path))
            {
                AesCbcEncryptionService.DecryptFile(path, command.Password, command.Overwrite);
                
                if (command.Delete) FileUtils.OverwriteAndDeleteFile(new FileInfo(path));
            }
            else Console.WriteLine($"COMMANDS: Invalid path: {path}");
        }
    }

    public static void Delete(Command command)
    {
        foreach (var path in command.Paths)
        {
            if (Directory.Exists(path)) FileUtils.OverwriteAndDeleteDirectory(new DirectoryInfo(path));
            else if (File.Exists(path)) FileUtils.OverwriteAndDeleteFile(new FileInfo(path));            
        }
    }
}
