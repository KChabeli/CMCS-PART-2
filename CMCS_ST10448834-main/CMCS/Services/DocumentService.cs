using CMCS.Models;

namespace CMCS.Services
{
    public class DocumentService : IDocumentService
    {
        private static List<Document> _documents = new List<Document>();
        private static int _nextDocumentId = 1;
        private readonly IWebHostEnvironment _environment;
        private readonly IFileEncryptionService _encryptionService;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".xlsx" };

        public DocumentService(IWebHostEnvironment environment, IFileEncryptionService encryptionService)
        {
            _environment = environment;
            _encryptionService = encryptionService;
        }

        public async Task<List<Document>> GetDocumentsByClaimIdAsync(int claimId)
        {
            return await Task.FromResult(_documents.Where(d => d.ClaimId == claimId).ToList());
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            return await Task.FromResult(_documents.FirstOrDefault(d => d.DocumentId == documentId));
        }

        public async Task<Document> UploadDocumentAsync(int claimId, IFormFile file, string? description = null)
        {
            if (!await ValidateFileAsync(file))
                throw new ArgumentException("Invalid file");

            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = await SaveEncryptedFileAsync(file, Path.Combine(uploadPath, fileName));

            var document = new Document
            {
                DocumentId = _nextDocumentId++,
                ClaimId = claimId,
                FileName = file.FileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                Description = description,
                UploadedDate = DateTime.Now
            };

            _documents.Add(document);
            return await Task.FromResult(document);
        }

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            var document = _documents.FirstOrDefault(d => d.DocumentId == documentId);
            if (document == null)
                return false;

            await DeleteFileAsync(document.FilePath);
            _documents.Remove(document);
            return true;
        }

        public async Task<bool> ValidateFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > _maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            return await Task.FromResult(true);
        }

        public async Task<string> SaveEncryptedFileAsync(IFormFile file, string filePath)
        {
            // Read file data
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
            }

            // Encrypt the file data
            var encryptedData = await _encryptionService.EncryptFileAsync(fileData);

            // Save encrypted data to file
            await File.WriteAllBytesAsync(filePath, encryptedData);
            return filePath;
        }

        public async Task<byte[]> DecryptFileAsync(string filePath)
        {
            var encryptedData = await File.ReadAllBytesAsync(filePath);
            return await _encryptionService.DecryptFileAsync(encryptedData);
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return await Task.FromResult(true);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }
    }
}
