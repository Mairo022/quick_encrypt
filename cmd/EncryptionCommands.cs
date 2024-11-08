using EncryptionTool.models;
using EncryptionTool.services;

namespace EncryptionTool.cmd;

public static class EncryptionCommands
{
    public static void EncryptFile(Arguments arguments)
    {
        var file = File.ReadAllBytes(arguments.File);
        var fileEncrypted = AesCbcEncryptionService.EncryptBytes(file, arguments.Password);

        File.WriteAllBytes(arguments.File + ".bin", fileEncrypted);
    }

    public static void EncryptDirectory(Arguments arguments)
    {
        AesCbcEncryptionService.EncryptDirectory(arguments.Dir, arguments.Password);
    }

    public static void DecryptFile(Arguments arguments)
    {
        AesCbcEncryptionService.DecryptFile(arguments.File, arguments.Password);
    }
}
