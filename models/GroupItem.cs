namespace EncryptionTool.models;

public class GroupItem
{
    readonly HashSet<string> _paths = [];
    
    public CommandAction Action { get; set; }
    public IReadOnlySet<string> Paths => _paths;
    public bool Delete { get; set; }
    public bool Overwrite { get; set; }
    
    public void AddPath(string path)
    {
        if (File.Exists(path) || Directory.Exists(path)) _paths.Add(path);
    }

    public void AddPaths(IEnumerable<string> paths)
    {
        foreach (var path in paths) AddPath(path);
    }

    public void RemovePath(string path)
    {
        _paths.Remove(path);
    }
    
    public void ClearPaths() => _paths.Clear();
}