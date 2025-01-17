using System.Runtime.InteropServices;
using System.Security;
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
    public static void EncryptFile(string filepath, byte[] key, bool isFileOverwriteEnabled = false)
    {
        var outputFilepath = GetFilepath(isFileOverwriteEnabled, $"{filepath}.bin");
        HandleFileOverwriteFlag(isFileOverwriteEnabled, outputFilepath);
        
        var fileType = CreateEncryptedFiletypeBytes(key, FILE_TYPES.FILE);
        var encryptedSize = fileType.Length + 16 + new FileInfo(filepath).Length;
        
        var cipher = InitCipherEncrypt(key, out var iv);
        
        using var fileReadStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using var fileWriteStream = new FileStream(outputFilepath, FileUtils.CreateForWriting(encryptedSize));
        using var cipherStream = new CipherStream(fileWriteStream, null, cipher);
        
        fileWriteStream.Write(fileType, 0, fileType.Length);
        fileWriteStream.Write(iv, 0, iv.Length);

        var bufferSize = 1024;
        var buffer = new byte[bufferSize];
        
        while (fileReadStream.Position < fileReadStream.Length)
        {
            var bytesRead = fileReadStream.Read(buffer, 0, bufferSize);
            cipherStream.Write(buffer, 0, bytesRead);
        }
    }

    public static byte[] DecryptBytes(byte[] encryptedBytes, byte[] key)
    {
        byte[] iv = encryptedBytes[..16];

        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

        return cipher.DoFinal(encryptedBytes[16..]);
    }

    public static void EncryptDirectory(string dir, byte[] key, bool isFileOverwriteEnabled = false)
    {
        var parentDir = Directory.GetParent(dir);
        
        if (parentDir == null) throw new InvalidOperationException("Can't encrypt root directory");
        
        var filenames = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
        var parentDirStr = parentDir.ToString();
        var outputFilepath = Path.Combine(parentDirStr, Path.GetFileName(dir) + ".bin");
        outputFilepath = GetFilepath(isFileOverwriteEnabled, outputFilepath);
        
        HandleFileOverwriteFlag(isFileOverwriteEnabled, outputFilepath);
        
        long encryptedSize = GetTotalEncryptedFilesSize(filenames, parentDirStr);
        var fileType = CreateEncryptedFiletypeBytes(key, FILE_TYPES.DIRECTORY);

        var cipher = InitCipherEncrypt(key, out byte[] iv);
        
        using var fileWriteStream = new FileStream(outputFilepath, FileUtils.CreateForWriting(encryptedSize));
        
        fileWriteStream.Write(fileType);
        fileWriteStream.Write(iv);

        foreach (var filename in filenames)
        {
            using var fileReadStream = new FileStream(filename, FileMode.Open, FileAccess.Read);

            var filenameRelativeToParent = filename[(parentDirStr.Length+1)..];
            
            // Set file header
            var filepath = Encoding.UTF8.GetBytes(filenameRelativeToParent);
            var filepathSize = BitConverter.GetBytes(filepath.Length);
            var fileSize = BitConverter.GetBytes(new FileInfo(filename).Length);
            
            var fileHeader = ArrayUtils.UniteByteArrays(fileSize, filepathSize, filepath);

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

    public static void DecryptFile(string fPath, byte[] key, bool isFileOverwriteEnabled = false)
    {
        var dir = Path.GetDirectoryName(fPath);

        if (dir == null) throw new InvalidOperationException("Cannot find directory");

        // Prepare for file reading
        using var fileReadStream = new FileStream(fPath, FileMode.Open, FileAccess.Read);
        var bufferSize = 1024;
        var buffer = new byte[bufferSize];

        // Get filetype
        var filetypeArea = new byte[32];
        fileReadStream.ReadExactly(filetypeArea);
        var filetype = GetDecryptedFiletype(filetypeArea, key);

        // Set cipher
        var iv = new byte[16];
        fileReadStream.ReadExactly(iv);
        var cipher = InitCipherDecrypt(key, iv);

        // Prepare for decryption
        using var cipherStream = new CipherStream(fileReadStream, cipher, null);
        FileStream? fileWriteStream = null;

        if (FILE_TYPES.DIRECTORY == filetype)
        {
            var encryptedFileSize = new FileInfo(fPath).Length;
            long fileEndPos = 0;

            while (fileReadStream.Position < fileReadStream.Length)
            {
                var isNewFile = fileEndPos < fileReadStream.Position;

                if (isNewFile)
                {
                    cipherStream.ReadExactly(buffer, 0 , 12);
                    
                    var filesize = BitConverter.ToInt64(buffer.AsSpan()[..8]);
                    var filenameSize = BitConverter.ToInt32(buffer[8..12]);
                    
                    cipherStream.ReadExactly(buffer, 0 , filenameSize);
                    
                    var filename = Encoding.UTF8.GetString(buffer[..filenameSize]);
                    var filepath = GetFilepath(isFileOverwriteEnabled, Path.Combine(dir, filename));
                    fileEndPos = fileReadStream.Position + filesize;

                    FileUtils.CreateFileDirectories(filepath);
                    HandleFileOverwriteFlag(isFileOverwriteEnabled, filepath);

                    if (filesize < 0 || filesize > encryptedFileSize)
                        throw new Exception("Error reading file size");
                    
                    fileWriteStream = new(filepath, FileUtils.CreateForWriting(filesize));
                }

                // Read either buffer size or amount to end of file (that is rounded to 16 byte block)
                // Overshot via rounding is automatically handled, as on next read the buffer's first element is right after fileEndPos
                var bytesToFileEnd = (int)(fileEndPos - fileReadStream.Position);
                var bytesRead = cipherStream.Read(buffer, 0, Math.Min(bufferSize, bytesToFileEnd));

                if (fileWriteStream == null) 
                    throw new Exception("File write stream not initialised before writing");

                if (bytesToFileEnd == bytesRead)
                {
                    fileWriteStream.Write(buffer, 0, bytesRead);
                    fileWriteStream.Close();
                    
                    fileEndPos = 0;

                    continue;
                }
                
                fileWriteStream.Write(buffer, 0, bytesRead);
            }
        }

        if (FILE_TYPES.FILE == filetype)
        {
            var filepath = fPath;
            var filesize = new FileInfo(filepath).Length - 48;

            filepath = GetFilepath(isFileOverwriteEnabled, filepath[^4..] != ".bin" ? filepath : filepath[..^4]);
            HandleFileOverwriteFlag(isFileOverwriteEnabled, filepath);

            fileWriteStream = new(filepath, FileUtils.CreateForWriting(filesize - fileReadStream.Position));

            while (fileReadStream.Position < fileReadStream.Length)
            {
                var bytesRead = cipherStream.Read(buffer, 0, bufferSize);
                fileWriteStream.Write(buffer, 0 , bytesRead);
            }
        }
   
        fileWriteStream?.Close();
    }

    public static byte[] Pbkdf2HashBytes(byte[] input)
    {
        if (input.Length == 0) return [];
        
        try
        {
            var salt = Encoding.UTF8.GetBytes(SALT);
            var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(input, salt, 600000, HashAlgorithmName.SHA256, 256 / 8);

            return pbkdf2;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
        finally
        {
            Array.Clear(input);
        }
    }

    public static byte[] Pbkdf2HashSecureString(SecureString input)
    {
        if (input.Length == 0) return [];
        
        var bstr = Marshal.SecureStringToBSTR(input);
        var length = Marshal.ReadInt32(bstr, -4);
        var key = new byte[length];
    
        var keyPin = GCHandle.Alloc(key, GCHandleType.Pinned);
        try
        {
            Marshal.Copy(bstr, key, 0, length);
            Marshal.ZeroFreeBSTR(bstr);

            var salt = Encoding.UTF8.GetBytes(SALT);
            var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(key, salt, 600000, HashAlgorithmName.SHA256, 256 / 8);

            return pbkdf2;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
        finally 
        {
            Array.Clear(key);
            keyPin.Free();
        }
    }

    static long GetTotalEncryptedFilesSize(string[] filepaths, string? parentDir)
    {
        if (filepaths.Length == 0) return 0;

        var getRelativePathLength = (string filepath) =>
            parentDir == null
                ? Encoding.UTF8.GetBytes(filepath).Length
                : Encoding.UTF8.GetBytes(Path.GetRelativePath(parentDir, filepath)).Length;
        
        // filetype
        var typeIv = 16;
        var typeVal = 16;
        var type = typeIv + typeVal;

        // file
        var iv = 16;
        var size = 8;
        var path = 4;
        var padding = 16 - 1;

        // files
        var files = iv + filepaths.Sum(filename => 
                           size + 
                           path + 
                           getRelativePathLength(filename) + 
                           new FileInfo(filename).Length
                           ) 
                       + padding;

        return type + files;
    }

    static IBufferedCipher InitCipherDecrypt(byte[] key, byte[] iv)
    {
        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

        return cipher;
    }

    static IBufferedCipher InitCipherEncrypt(byte[] key, out byte[] iv)
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
        var filetypeDecrypted = DecryptBytes(filetypeEncrypted, key);
        var filetype = BitConverter.ToInt32(filetypeDecrypted, 0);

        return filetype switch
        {
            (int)FILE_TYPES.FILE => FILE_TYPES.FILE,
            (int)FILE_TYPES.DIRECTORY => FILE_TYPES.DIRECTORY,
            _ => FILE_TYPES.NONE
        };
    }

    static byte[] CreateEncryptedFiletypeBytes(byte[] key, FILE_TYPES filetype)
    {
        var cipher = InitCipherEncrypt(key, out byte[] iv);
        var encrypted = cipher.DoFinal(BitConverter.GetBytes((int)filetype));

        return ArrayUtils.UniteByteArrays(iv, encrypted);
    }
    
    static string GetFilepath(bool isFileOverwriteEnabled, string filepath) => 
        isFileOverwriteEnabled ? filepath : FileUtils.GetUniqueFilepath(filepath);
    
    static void HandleFileOverwriteFlag(bool isFileOverwriteEnabled, string filepath)
    {
        if (isFileOverwriteEnabled && File.Exists(filepath)) File.Delete(filepath);
    }
}
