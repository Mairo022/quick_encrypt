namespace EncryptionTool.utils;

public static class StringUtils
{
    public static string[] SplitFromLastOccurence(string str, char c)
    {
        int indexOfChar = str.LastIndexOf(c);

        if (indexOfChar == -1) return [];

        string firstPart = str[0..(indexOfChar)];
        string secondPart = str[(indexOfChar)..];

        return [firstPart, secondPart];
    }
}