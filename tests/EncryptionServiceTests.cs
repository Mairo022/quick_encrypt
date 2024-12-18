using EncryptionTool.services;
using FluentAssertions;

namespace Tests;


public class EncryptionServiceTests
{
    readonly byte[] _key = new byte[32];
    
    public EncryptionServiceTests()
    {
        new Random().NextBytes(_key);
    }
    
    [Fact]
    public void EncryptDecryptFile_ReturnsOriginalFile()
    {
        var testFilePath = Path.GetTempFileName();
        var encryptedFilePath = testFilePath + ".bin";
        var decryptedFilePath = testFilePath[..^4] + " (2).tmp";
        CreateFile(testFilePath, 4096);
        
        AesCbcEncryptionService.EncryptFile(testFilePath, _key);
        AesCbcEncryptionService.DecryptFile(encryptedFilePath, _key);
        
        var originalFile = File.ReadAllBytes(testFilePath);
        var decryptedFile = File.ReadAllBytes(decryptedFilePath);
        var filesMatch = originalFile.SequenceEqual(decryptedFile);
        
        filesMatch.Should().BeTrue();
        
        File.Delete(testFilePath);
        File.Delete(encryptedFilePath);
        File.Delete(decryptedFilePath);
    }

    [Fact]
    public void EncryptDecryptDirectory_ReturnsOriginalDirectory()
    {
        var dirPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var encryptedDirPath = dirPath + ".bin";
        var filesCount = 5;
        CreateDirectoryWithFiles(dirPath, filesCount);
        
        AesCbcEncryptionService.EncryptDirectory(dirPath, _key);
        AesCbcEncryptionService.DecryptFile(encryptedDirPath, _key);
        
        var filepaths = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
        var filesMatch = new bool[filesCount];
        
        for (int i = 0, j = 0; i < filepaths.Length; i += 2, j++)
        {
            var filepathOriginal = filepaths[i + 1];
            var filepathDecrypted = filepaths[i];
            
            var originalFile = File.ReadAllBytes(filepathOriginal);
            var decryptedFile = File.ReadAllBytes(filepathDecrypted);
            
            filesMatch[j] = originalFile.SequenceEqual(decryptedFile);
        }
        
        filepaths.Should().HaveCount(filesCount * 2);
        filesMatch.Should().AllBeEquivalentTo(true);
        
        DeleteDirectory(dirPath);
        File.Delete(encryptedDirPath);
    }

    [Fact]
    public void DecryptBytes_ReturnsOriginalBytes()
    {
        const int fileTypeBytesLen = 32;
        
        var testFilePath = Path.GetTempFileName();
        var encryptedFilePath = testFilePath + ".bin";
        CreateFile(testFilePath, 2022);
        
        AesCbcEncryptionService.EncryptFile(testFilePath, _key);
        AesCbcEncryptionService.DecryptFile(encryptedFilePath, _key);
        
        var originalFile = File.ReadAllBytes(testFilePath);
        var encryptedFile = File.ReadAllBytes(encryptedFilePath);
        var decryptedFileBytes = AesCbcEncryptionService.DecryptBytes(encryptedFile, _key);
        var bytesMatch = originalFile.SequenceEqual(decryptedFileBytes[fileTypeBytesLen..]);
        
        bytesMatch.Should().BeTrue();
        
        File.Delete(testFilePath);
        File.Delete(encryptedFilePath);
    }

    void CreateFile(string filepath, int fileSize)
    {
        var randomBytes = new byte[fileSize];
        new Random().NextBytes(randomBytes);
        
        File.WriteAllBytes(filepath, randomBytes);
    }
    
    void CreateDirectoryWithFiles(string dirPath, int filesCount)
    {
        Directory.CreateDirectory(dirPath);

        for (var i = 0; i < filesCount; i++)
        {
            var filename = $"{i}.tmp";
            var filepath = Path.Combine(dirPath, filename);
            var filesize = new Random().Next(0, 10000);
            
            CreateFile(filepath, filesize);            
        }
    }
    
    void DeleteDirectory(string dirPath)
    {
        var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);

        foreach (var file in files) File.Delete(file);
        Directory.Delete(dirPath);
    }
}