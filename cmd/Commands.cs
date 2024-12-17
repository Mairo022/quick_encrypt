using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;

namespace EncryptionTool.cmd;

public static class Commands
{
    public static void Encrypt(Arguments arguments)
    {
        foreach (var path in arguments.Path)
        {
            if (File.Exists(path))
            {
                AesCbcEncryptionService.EncryptFile(path, arguments.Password);
                
                if (arguments.Delete) FileUtils.OverwriteAndDeleteFile(new FileInfo(path));
            }
            else if (Directory.Exists(path))
            {
                AesCbcEncryptionService.EncryptDirectory(path, arguments.Password);
                
                if (arguments.Delete) FileUtils.OverwriteAndDeleteDirectory(new DirectoryInfo(path));
            }
            else Console.WriteLine($"Invalid path: {path}");
        }
    }

    public static void Decrypt(Arguments arguments)
    {
        foreach (var path in arguments.Path)
        {
            if (File.Exists(path)) AesCbcEncryptionService.DecryptFile(path, arguments.Password);
            else Console.WriteLine($"Invalid path: {path}");
        }
    }
}
