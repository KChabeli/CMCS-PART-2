using CMCS.Models;

namespace CMCS.Services
{
    public interface IDocumentService
    {
        Task<List<Document>> GetDocumentsByClaimIdAsync(int claimId);
        Task<Document?> GetDocumentByIdAsync(int documentId);
        Task<Document> UploadDocumentAsync(int claimId, IFormFile file, string? description = null);
        Task<bool> DeleteDocumentAsync(int documentId);
        Task<bool> ValidateFileAsync(IFormFile file);
        Task<string> SaveEncryptedFileAsync(IFormFile file, string uploadPath);
        Task<byte[]> DecryptFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
    }
}
