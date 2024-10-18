namespace EncryptionTool.services;

public static class FileService
{
    public static byte[] ReadFile(string filepath)
    {
        return File.ReadAllBytes(filepath);
    }

    public static void WriteFile(string filepath, byte[] data)
    {
        File.WriteAllBytes(filepath, data);
    }
}
