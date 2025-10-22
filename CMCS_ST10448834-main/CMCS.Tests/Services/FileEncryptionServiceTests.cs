using System.Security.Cryptography;
using System.Text;

namespace CMCS.Services
{
    public class FileEncryptionService : IFileEncryptionService
    {
        private readonly string _encryptionKey;

        public FileEncryptionService()
        {
            // Use a fixed 32-byte key for consistent testing
            _encryptionKey = "ThisIsA32ByteKeyForTesting123456"; // 32 characters = 32 bytes
        }

        public async Task<byte[]> EncryptFileAsync(byte[] fileData)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();

            // Write IV to the beginning of the stream
            await msEncrypt.WriteAsync(aes.IV, 0, aes.IV.Length);

            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            await csEncrypt.WriteAsync(fileData, 0, fileData.Length);
            await csEncrypt.FlushFinalBlockAsync();

            return msEncrypt.ToArray();
        }

        public async Task<byte[]> DecryptFileAsync(byte[] encryptedData)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);

            // Extract IV from the beginning of the encrypted data
            var iv = new byte[16];
            Array.Copy(encryptedData, 0, iv, 0, 16);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(encryptedData, 16, encryptedData.Length - 16);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var msResult = new MemoryStream();

            await csDecrypt.CopyToAsync(msResult);
            return msResult.ToArray();
        }

        public string GenerateEncryptionKey()
        {
            // Generate a 32-byte (256-bit) key
            using var rng = RandomNumberGenerator.Create();
            var keyBytes = new byte[32];
            rng.GetBytes(keyBytes);
            return Convert.ToBase64String(keyBytes);
        }
    }
}
