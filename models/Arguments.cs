namespace EncryptionTool.models;

public enum AllowedArgumentsActions
{
    unknown,
    encrypt,
    decrypt
}

public class Arguments()
{
    readonly List<string> _path = [];
    
    public AllowedArgumentsActions? Action { get; set; } = null;
    public IReadOnlyList<string> Path => _path;
    public byte[] Password { get; set; } = [];

    public readonly string[] AllowedArguments = ["action", "path", "password"];

    public void Clear()
    {
        Action = null;
        Array.Clear(Password);
        _path.Clear();
    }

    public void AddPath(string path)
    {
        if (File.Exists(path) || Directory.Exists(path))
        {
            _path.Add(path);
            return;
        }

        Console.WriteLine($"Invalid path: {path}");
    }
}
