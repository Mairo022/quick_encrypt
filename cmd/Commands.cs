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
            {
                var file = File.ReadAllBytes(path);
                var fileEncrypted = AesCbcEncryptionService.EncryptBytes(file, arguments.Password);

                File.WriteAllBytes(path + ".bin", fileEncrypted);
            }
            else if (Directory.Exists(path))
            {
                AesCbcEncryptionService.EncryptDirectory(path, arguments.Password);
            }
            else
            {
                Console.WriteLine($"Invalid path: {path}");
            }
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
