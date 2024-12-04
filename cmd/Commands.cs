using EncryptionTool.models;
using EncryptionTool.services;

namespace EncryptionTool.cmd;

public static class Commands
{
    public static void Encrypt(Arguments arguments)
    {
        foreach (var path in arguments.Path)
        {
            if (File.Exists(path)) 
                AesCbcEncryptionService.EncryptFile(path, arguments.Password);
            else if (Directory.Exists(path)) 
                AesCbcEncryptionService.EncryptDirectory(path, arguments.Password);
            else 
                Console.WriteLine($"Invalid path: {path}");
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
