namespace EncryptionTool.models;

public class Command()
{
    readonly HashSet<string> _paths = [];

    public CommandAction Action { get; set; } = CommandAction.unknown;
    public IReadOnlySet<string> Paths => _paths;
    public bool Delete { get; set; }
    public bool Overwrite { get; set; }
    public byte[] Password { get; set; } = [];
    
    public void Clear()
    {
        Action = CommandAction.unknown;
        Array.Clear(Password);
        _paths.Clear();
    }

    public void AddPath(string path)
    {
        if (File.Exists(path) || Directory.Exists(path))
        {
            _paths.Add(path);
            return;
        }

        Console.WriteLine($"Invalid path: {path}");
    }

    public void AddPaths(IEnumerable<string> paths)
    {
        foreach (var path in paths) AddPath(path);
    }
}
