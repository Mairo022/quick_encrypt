namespace EncryptionTool.utils;

public static class ArrayUtils
{
    public static byte[] UniteByteArrays(params byte[][] arrays)
    {
        byte[] unitedArray = new byte[arrays.Sum(x => x.Length)];
        int offset = 0;

        foreach (byte[] arr in arrays)
        {
            Buffer.BlockCopy(arr, 0, unitedArray, offset, arr.Length);
            offset += arr.Length;
        }

        return unitedArray;
    }
}