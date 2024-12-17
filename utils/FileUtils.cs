namespace EncryptionTool.utils;

public static class FileUtils
{
    public static string GetUniqueFilepath(string filepath)
    {
        if (!File.Exists(filepath)) return filepath;

        var dir = Path.GetDirectoryName(filepath);
        var extension = Path.GetExtension(filepath);
        var filename = Path.GetFileNameWithoutExtension(filepath);

        var newFilepath = "";
        var n = 2;

        do
        {
            newFilepath = Path.Combine(dir, $"{filename} ({n}){extension}");
            n++;
            if (n == 100) throw new Exception($"Tried to create filename for {n} times, couldn't");
        } while (File.Exists(newFilepath));

        return newFilepath;
    }

    public static void CreateFileDirectories(string filepath)
    {
        var filepathDir = Path.GetDirectoryName(filepath);

        if (!Directory.Exists(filepathDir))
        {
            Directory.CreateDirectory(filepathDir);
        }
    }

    public static FileStreamOptions CreateForWriting(long allocSize)
    {
        return new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.Write,
            PreallocationSize = allocSize
        };
    }
    
    public static bool IsFileInUse(FileInfo file)
    {
        if (!File.Exists(file.FullName)) return false;
        
        try
        {
            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (IOException)
        {
            return true;
        }
        return false;
    }

    public static void OverwriteAndDeleteDirectory(DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);
        
        var files = Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            OverwriteAndDeleteFile(new FileInfo(file));
        }
        
        directory.Delete(true);
    }

    public static void OverwriteAndDeleteFile(FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);
        
        using var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Write);

        var bufferSize = 1024;
        var fileSize = stream.Length;
        var buffer = new byte[bufferSize];
        Array.Fill(buffer, (byte) 0);

        while (stream.Position < fileSize)
        {
            stream.Write(buffer, 0, (int) Math.Min(bufferSize, fileSize - stream.Position));
        }
        
        stream.Close();
        file.Delete();
    }
}
