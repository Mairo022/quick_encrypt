using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace EncryptionTool.services;

public class EncryptionService()
{
    public static byte[] Aes256Encrypt(byte[] input, byte[] key)
    {
        SecureRandom random = new();

        byte[] iv = new byte[16];
        random.NextBytes(iv);

        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

        using var combinedStream = new MemoryStream();
        {
            using var binaryWriter = new BinaryWriter(combinedStream);
            {
                binaryWriter.Write(iv);
                binaryWriter.Write(cipher.DoFinal(input));
            }

            return combinedStream.ToArray();
        }
    }

    public static byte[] AesDecrypt(byte[] encryptedBytes, byte[] key)
    {
        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        byte[] iv = encryptedBytes[..16];

        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

        return cipher.DoFinal(encryptedBytes[16..]);
    }

    public static byte[] PBKDF2Hash(byte[] input, byte[] salt)
    {
        byte[] pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(input, salt, 200000, HashAlgorithmName.SHA256, 256 / 8);

        return pbkdf2;
    }
}