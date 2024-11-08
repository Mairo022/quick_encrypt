namespace EncryptionTool.models;

public enum AllowedArgumentsActions
{
    encrypt,
    decrypt
}

public class Arguments()
{
    public AllowedArgumentsActions? Action { get; set; } = null;
    public string File { get; set; } = string.Empty;
    public string Dir { get; set; } = string.Empty;
    public byte[] Password { get; set; } = [];

    public readonly string[] AllowedArguments = ["action", "file", "password"];

    public void Clear()
    {
        this.Action = null;
        this.File = string.Empty;
        Array.Clear(this.Password);
    }
}
