using EncryptionTool.models;
using EncryptionTool.services;
using EncryptionTool.utils;

namespace EncryptionTool.cmd;

public static class EncryptionCommands
{
    public static void Encrypt(Arguments arguments)
    {
        var file = FileService.ReadFile(arguments.File);
        var fileEncrypted = EncryptionService.Aes256Encrypt(file, arguments.Password);

        FileService.WriteFile(arguments.File + ".ec", fileEncrypted);

        arguments.Clear();
    }

    public static void Decrypt(Arguments arguments)
    {
        var file = FileService.ReadFile(arguments.File);
        var fileDecrypted = EncryptionService.AesDecrypt(file, arguments.Password);
        var filepathDecrypted = GetDecryptedFilepath(arguments.File);

        FileService.WriteFile(filepathDecrypted, fileDecrypted);

        arguments.Clear();
    }

    private static string GetDecryptedFilepath(string filepathEncrypted)
    {
        var filepathSplit = StringUtils.SplitFromLastOccurence(filepathEncrypted, '\\');
        if (filepathSplit.Length != 2) throw new Exception("Could not split filepath by \\");

        var filenameSplit = StringUtils.SplitFromLastOccurence(filepathSplit[1], '.');
        if (filepathSplit.Length != 2) throw new Exception("Could not split filename by .");

        return filepathSplit[0] + "\\dc." + filenameSplit[0][1..];
    }
}
