using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;


const string originalFilepath = "sample.txt";
const string encryptedFilepath = "sample.txt.encrypted";
const string decryptedFilepath = "sample.txt.decrypted";

CreateSampleFile(originalFilepath);
HandleFileEncryption(originalFilepath, encryptedFilepath);
HandleFileDecryption(encryptedFilepath, decryptedFilepath);
CompareFiles(originalFilepath, decryptedFilepath);

static void HandleFileEncryption(string filepath, string encryptedFilepath)
{
    string password = "password";
    string salt = "salt";

    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
    byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
    byte[] key = PBKDF2Hash(passwordBytes, saltBytes);

    try
    {
        byte[] file = File.ReadAllBytes(filepath);
        byte[] fileEncrypted = Aes256Encrypt(file, key);

        File.WriteAllBytes(encryptedFilepath, fileEncrypted);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

static void HandleFileDecryption(string encryptedFilepath, string decryptedFilepath)
{
    string password = "password";
    string salt = "salt";

    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
    byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
    byte[] key = PBKDF2Hash(passwordBytes, saltBytes);

    try
    {
        var encryptedFile = File.ReadAllBytes(encryptedFilepath);
        var decryptedFile = AesDecrypt(encryptedFile, key);

        File.WriteAllBytes(decryptedFilepath, decryptedFile);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

static byte[] Aes256Encrypt(byte[] input, byte[] key)
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

static byte[] AesDecrypt(byte[] encryptedBytes, byte[] key)
{
    var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
    byte[] iv = encryptedBytes[..16];

    cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

    return cipher.DoFinal(encryptedBytes[16..]);
}


static byte[] PBKDF2Hash(byte[] input, byte[] salt)
{
    var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(input, salt, 200000, HashAlgorithmName.SHA256, 256 / 8);

    return pbkdf2;
}

static void CreateSampleFile(string filepath)
{
    using StreamWriter writer = new(filepath);
    {
        writer.WriteLine("Sample text file");
    }
    writer.Close();
}

static void CompareFiles(string originalFilepath, string decryptedFilepath)
{
    try
    {
        var originalFile = File.ReadAllBytes(originalFilepath);
        var decryptedFile = File.ReadAllBytes(decryptedFilepath);

        var originalHash = SHA256.HashData(originalFile);
        var decryptedHash = SHA256.HashData(decryptedFile);

        if (originalHash.SequenceEqual(decryptedHash))
        {
            Console.WriteLine("Original and decrypted file hashes are equal");
        } 
        else
        {
            throw new Exception("Original and decrypted file hashes are not equal");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}