using System.Security.Cryptography;

namespace CMCS.Services
{
    public interface IFileEncryptionService
    {
        Task<byte[]> EncryptFileAsync(byte[] fileData);
        Task<byte[]> DecryptFileAsync(byte[] encryptedData);
        string GenerateEncryptionKey();
    }
}
