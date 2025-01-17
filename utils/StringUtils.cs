namespace EncryptionTool.utils;

public static class StringUtils
{
    public static string FirstLetterToUpper(string? str)
    {
        if (str == null)
            return "";

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
}