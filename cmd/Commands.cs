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
                AesCbcEncryptionService.EncryptFile(path, command.Password);
                
                if (command.Delete) FileUtils.OverwriteAndDeleteFile(new FileInfo(path));
            }
            else if (Directory.Exists(path))
            {
                AesCbcEncryptionService.EncryptDirectory(path, command.Password);
                
                if (command.Delete) FileUtils.OverwriteAndDeleteDirectory(new DirectoryInfo(path));
            }
            else Console.WriteLine($"Invalid path: {path}");
        }
    }

    public static void Decrypt(Command command)
    {
        foreach (var path in command.Paths)
        {
            if (File.Exists(path))
            {
                AesCbcEncryptionService.DecryptFile(path, command.Password);
                
                if (command.Delete) FileUtils.OverwriteAndDeleteFile(new FileInfo(path));
            }
            else Console.WriteLine($"Invalid path: {path}");
        }
    }
}
