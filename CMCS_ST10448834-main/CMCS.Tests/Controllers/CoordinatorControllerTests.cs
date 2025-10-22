using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CMCS.Controllers;
using CMCS.Models;
using CMCS.Services;

namespace CMCS.Tests.Controllers
{
    public class CoordinatorControllerTests
    {
        private readonly Mock<IClaimService> _mockClaimService;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<ILogger<CoordinatorController>> _mockLogger;
        private readonly CoordinatorController _controller;

        public CoordinatorControllerTests()
        {
            _mockClaimService = new Mock<IClaimService>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockLogger = new Mock<ILogger<CoordinatorController>>();
            _controller = new CoordinatorController(_mockClaimService.Object, _mockDocumentService.Object, _mockLogger.Object);

            // Setup TempData
            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataProvider());
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithPendingClaims()
        {
            // Arrange
            var pendingClaims = new List<Claim>
            {
                new Claim { ClaimId = 1, LecturerId = 1, HoursWorked = 40, HourlyRate = 25.50m, Status = "Pending" },
                new Claim { ClaimId = 2, LecturerId = 2, HoursWorked = 35, HourlyRate = 30.00m, Status = "Pending" }
            };

            _mockClaimService.Setup(s => s.GetPendingClaimsAsync()).ReturnsAsync(pendingClaims);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Claim>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task VerifyClaim_ShouldRedirectToIndex()
        {
            // Arrange
            var claimId = 1;
            var claim = new Claim { ClaimId = claimId, Status = "Approved" };
            _mockClaimService.Setup(s => s.ApproveClaimAsync(claimId, "Programme Coordinator")).ReturnsAsync(claim);

            // Act
            var result = await _controller.VerifyClaim(claimId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task RejectClaim_WithValidReason_ShouldRedirectToIndex()
        {
            // Arrange
            var claimId = 1;
            var rejectionReason = "Insufficient documentation";
            var claim = new Claim { ClaimId = claimId, Status = "Rejected", RejectionReason = rejectionReason };
            _mockClaimService.Setup(s => s.RejectClaimAsync(claimId, "Programme Coordinator", rejectionReason)).ReturnsAsync(claim);

            // Act
            var result = await _controller.RejectClaim(claimId, rejectionReason);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task RejectClaim_WithEmptyReason_ShouldRedirectToIndex()
        {
            // Arrange
            var claimId = 1;
            var rejectionReason = "";

            // Act
            var result = await _controller.RejectClaim(claimId, rejectionReason);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DownloadDocument_WithValidDocument_ShouldReturnFile()
        {
            // Arrange
            var documentId = 1;
            var document = new Document { DocumentId = documentId, FileName = "test.pdf", ContentType = "application/pdf", FilePath = "test/path" };
            var decryptedData = new byte[] { 1, 2, 3, 4, 5 };

            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId)).ReturnsAsync(document);
            _mockDocumentService.Setup(s => s.DecryptFileAsync(document.FilePath)).ReturnsAsync(decryptedData);

            // Act
            var result = await _controller.DownloadDocument(documentId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal(decryptedData, fileResult.FileContents);
            Assert.Equal(document.ContentType, fileResult.ContentType);
            Assert.Equal(document.FileName, fileResult.FileDownloadName);
        }

        [Fact]
        public async Task DownloadDocument_WithInvalidDocument_ShouldReturnNotFound()
        {
            // Arrange
            var documentId = 999;
            _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(documentId)).ReturnsAsync((Document?)null);

            // Act
            var result = await _controller.DownloadDocument(documentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
