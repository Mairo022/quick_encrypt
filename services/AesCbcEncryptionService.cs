using EncryptionTool.utils;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;
using static EncryptionTool.Globals;

namespace EncryptionTool.services;

public static class AesCbcEncryptionService
{
    public static byte[] EncryptBytes(byte[] input, byte[] key)
    {
        var cipher = InitCipher(key, out byte[] iv);
        var fileType = CreateEncryptedFiletypeBytes(key, FILE_TYPES.FILE);

        using var combinedStream = new MemoryStream();
        {
            using var binaryWriter = new BinaryWriter(combinedStream);
            {
                binaryWriter.Write(fileType);
                binaryWriter.Write(iv);
                binaryWriter.Write(cipher.DoFinal(input));
            }

            return combinedStream.ToArray();
        }   
    }

    public static byte[] DecryptBytes(byte[] encryptedBytes, byte[] key)
    {
        byte[] iv = encryptedBytes[..16];

        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

        return cipher.DoFinal(encryptedBytes[16..]);
    }

    public static void EncryptDirectory(string dir, byte[] key)
    {
        var filenames = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        var parentDir = Directory.GetParent(dir).ToString();
        var outputFilepath = Path.Combine(parentDir, Path.GetFileName(dir) + ".bin");
        outputFilepath = FileService.GetUniqueFilepath(outputFilepath);

        long encryptedSize = GetTotalEncryptedFilesSize(filenames);
        var fileType = CreateEncryptedFiletypeBytes(key, FILE_TYPES.DIRECTORY);

        var cipher = InitCipher(key, out byte[] iv);

        using var fileWriteStream = new FileStream(outputFilepath, FileService.CreateForWriting(encryptedSize));
        fileWriteStream.Write(fileType);
        fileWriteStream.Write(iv);

        foreach (var filename in filenames)
        {
            using var fileReadStream = new FileStream(filename, FileMode.Open, FileAccess.Read);

            var filenameRelativeToParent = filename[(parentDir.Length+1)..];
            
            // Set file header
            var filepath = Encoding.UTF8.GetBytes(filenameRelativeToParent).Concat(new byte[264 - filenameRelativeToParent.Length]).ToArray();
            var fileSize = BitConverter.GetBytes(new FileInfo(filename).Length);
            var fileHeader = ArrayUtils.UniteByteArrays(filepath, fileSize);

            var encrypted = cipher.ProcessBytes(fileHeader);
            fileWriteStream.Write(encrypted);

            int bytesRead;
            var buffer = new byte[1024];

            while ((bytesRead = fileReadStream.Read(buffer, 0 , buffer.Length)) > 0)
            {
                encrypted = cipher.ProcessBytes(buffer, 0 , bytesRead);
                fileWriteStream.Write(encrypted);
            }
        }

        var final = cipher.DoFinal();
        if (final != null && final.Length > 0)
        {
            fileWriteStream.Write(final);
        }

        fileWriteStream.Close();
    }

    public static void DecryptFile(string fPath, byte[] key)
    {
        var dir = Path.GetDirectoryName(fPath);

        // Prepare for file reading
        using var fileReadStream = new FileStream(fPath, FileMode.Open, FileAccess.Read);
        var bufferSize = 1024;
        var buffer = new byte[bufferSize];

        // Get filetype
        var filetypeArea = new byte[32];
        fileReadStream.Read(filetypeArea, 0, 32);
        var filetype = GetDecryptedFiletype(filetypeArea, key);

        // Set cipher
        var iv = new byte[16];
        fileReadStream.Read(iv, 0, 16);
        var cipher = BufferedCipher(key, iv);

        // Prepare for decryption
        using var cipherStream = new CipherStream(fileReadStream, cipher, null);
        FileStream fileWriteStream = null;

        if (FILE_TYPES.DIRECTORY == filetype)
        {
            var filepath = "";
            var isNewFile = true;
            long fileEndPos = 0;

            while (fileReadStream.Position < fileReadStream.Length)
            {
                isNewFile = fileEndPos < fileReadStream.Position;

                if (isNewFile)
                {
                    cipherStream.Read(buffer, 0, GetFileHeaderSize());

                    var filename = Encoding.UTF8.GetString(buffer[..264].Where(b => b != 0).ToArray());
                    var filesize = BitConverter.ToInt64(buffer.AsSpan()[264..272]);

                    filepath = Path.Combine(dir, filename);
                    fileEndPos = fileReadStream.Position + filesize;

                    FileService.CreateFileDirectories(filepath);
                    filepath = FileService.GetUniqueFilepath(filepath);

                    // TODO: Create some sort of safety, in case file is read wrong
                    fileWriteStream = new(filepath, FileService.CreateForWriting(fileEndPos - fileReadStream.Position));
                }

                // Read either buffer size or amount to end of file (that is rounded to 16 byte block)
                // Overshot via rounding is automatically handled, as on next read the buffer's first element is right after fileEndPos
                var bytesToFileEnd = (int)(fileEndPos - fileReadStream.Position);
                var bytesRead = cipherStream.Read(buffer, 0, Math.Min(bufferSize, bytesToFileEnd));

                if (fileWriteStream == null) 
                { 
                    throw new Exception("File write stream not initialised before writing");
                }

                if (bytesToFileEnd == bytesRead)
                {
                    fileWriteStream.Write(buffer, 0, bytesRead);
                    fileWriteStream.Close();

                    filepath = String.Empty;
                    fileEndPos = 0;

                    continue;
                }
                
                fileWriteStream.Write(buffer, 0, bytesRead);
            }
        }

        if (FILE_TYPES.FILE == filetype)
        {
            var filepath = fPath;
            var filesize = new FileInfo(filepath).Length - GetFileHeaderSize() - 48;

            if (filepath[^4..] != ".bin") filepath = FileService.GetUniqueFilepath(filepath);
            else filepath = FileService.GetUniqueFilepath(filepath[..^4]);

            fileWriteStream = new(filepath, FileService.CreateForWriting(filesize - fileReadStream.Position));

            while (fileReadStream.Position < fileReadStream.Length)
            {
                var bytesRead = cipherStream.Read(buffer, 0, bufferSize);
                fileWriteStream.Write(buffer, 0 , bytesRead);
            }

            fileWriteStream.Close();
        }
    }

    public static byte[] PBKDF2Hash(byte[] input)
    {
        var salt = Encoding.UTF8.GetBytes(SALT);
        var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(input, salt, 600000, HashAlgorithmName.SHA256, 256 / 8);

        return pbkdf2;
    }

    static int GetFileHeaderSize()
    {
        // Windows limit is 260, +4 is a filler
        var path = 264;
        var fileSize = sizeof(long);

        if ((path + fileSize) % 16 != 0) throw new Exception("File header size is not a multiple of CBC block size");

        return path + fileSize;
    }

    static long GetTotalEncryptedFilesSize(string[] filepaths)
    {
        // filetype
        var typeIV = 16;
        var typeVal = 16;
        var type = typeIV + typeVal;

        // file
        var iv = 16;
        var header = GetFileHeaderSize(); // 272
        var padding = 16 - 1;

        // files
        var files = iv + filepaths.Sum(filename => header + new FileInfo(filename).Length) + padding;

        return type + files;
    }

    static IBufferedCipher BufferedCipher(byte[] key, byte[] iv)
    {
        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

        return cipher;
    }

    static IBufferedCipher InitCipher(byte[] key, out byte[] iv)
    {
        SecureRandom random = new();

        iv = new byte[16];
        random.NextBytes(iv);

        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

        return cipher;
    }

    static FILE_TYPES GetDecryptedFiletype(byte[] filetypeEncrypted, byte[] key)
    {
        byte[] filetypeDecrypted = DecryptBytes(filetypeEncrypted, key);
        int filetype = BitConverter.ToInt32(filetypeDecrypted, 0);

        return filetype switch
        {
            (int)FILE_TYPES.FILE => FILE_TYPES.FILE,
            (int)FILE_TYPES.DIRECTORY => FILE_TYPES.DIRECTORY,
            _ => FILE_TYPES.NONE
        };
    }

    static byte[] CreateEncryptedFiletypeBytes(byte[] key, FILE_TYPES filetype)
    {
        var cipher = InitCipher(key, out byte[] iv);
        var encrypted = cipher.DoFinal(BitConverter.GetBytes((int)filetype));

        return ArrayUtils.UniteByteArrays(iv, encrypted);
    }
}
