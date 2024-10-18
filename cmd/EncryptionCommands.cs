using EncryptionTool.services;
using EncryptionTool.utils;
using System.Text;

namespace EncryptionTool.cmd;

public static class EncryptionCommands
{
    public static void Encrypt(Dictionary<AllowedArguments, string> arguments)
    {
        var filepath = arguments[AllowedArguments.file];
        var password = arguments[AllowedArguments.password];

        var salt = Encoding.UTF8.GetBytes("salt");
        var key = EncryptionService.PBKDF2Hash(Encoding.UTF8.GetBytes(password), salt);

        salt = [];
        password = "";
        arguments.Clear();

        var file = FileService.ReadFile(filepath);
        var fileEncrypted = EncryptionService.Aes256Encrypt(file, key);

        FileService.WriteFile(filepath + ".ec", fileEncrypted);
    }

    public static void Decrypt(Dictionary<AllowedArguments, string> arguments)
    {
        var filepath = arguments[AllowedArguments.file];
        var password = arguments[AllowedArguments.password];

        var salt = Encoding.UTF8.GetBytes("salt");
        var key = EncryptionService.PBKDF2Hash(Encoding.UTF8.GetBytes(password), salt);

        salt = [];
        password = "";
        arguments.Clear();

        var file = FileService.ReadFile(filepath);
        var fileDecrypted = EncryptionService.AesDecrypt(file, key);
        var filepathDecrypted = GetDecryptedFilepath(filepath);

        FileService.WriteFile(filepathDecrypted, fileDecrypted);
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
