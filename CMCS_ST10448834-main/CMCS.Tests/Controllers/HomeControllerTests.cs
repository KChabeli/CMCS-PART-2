using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CMCS.Controllers;
using CMCS.Models;
using CMCS.Services;

namespace CMCS.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IClaimService> _mockClaimService;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockClaimService = new Mock<IClaimService>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockClaimService.Object, _mockDocumentService.Object, _mockLogger.Object);

            // Setup TempData
            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new Microsoft.AspNetCore.Http.DefaultHttpContext(),
                new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataProvider());
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithDashboardData()
        {
            // Arrange
            _mockClaimService.Setup(s => s.GetClaimCountByStatusAsync("Pending")).ReturnsAsync(5);
            _mockClaimService.Setup(s => s.GetClaimCountByStatusAsync("Approved")).ReturnsAsync(10);
            _mockClaimService.Setup(s => s.GetClaimCountByStatusAsync("Rejected")).ReturnsAsync(2);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(5, _controller.ViewBag.PendingCount);
            Assert.Equal(10, _controller.ViewBag.ApprovedCount);
            Assert.Equal(2, _controller.ViewBag.RejectedCount);
        }

        [Fact]
        public void SubmitClaim_Get_ShouldReturnViewWithViewModel()
        {
            // Act
            var result = _controller.SubmitClaim();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ClaimSubmissionViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task SubmitClaim_Post_WithValidModel_ShouldRedirectToTrackClaims()
        {
            // Arrange
            var model = new ClaimSubmissionViewModel
            {
                HoursWorked = 40,
                HourlyRate = 25.50m,
                Notes = "Test claim"
            };

            var claim = new Claim
            {
                ClaimId = 1,
                LecturerId = 1,
                HoursWorked = model.HoursWorked,
                HourlyRate = model.HourlyRate,
                Notes = model.Notes
            };

            _mockClaimService.Setup(s => s.SubmitClaimAsync(It.IsAny<Claim>())).ReturnsAsync(claim);

            // Act
            var result = await _controller.SubmitClaim(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("TrackClaims", redirectResult.ActionName);
        }

        [Fact]
        public async Task SubmitClaim_Post_WithInvalidModel_ShouldReturnView()
        {
            // Arrange
            var model = new ClaimSubmissionViewModel
            {
                HoursWorked = -1, // Invalid
                HourlyRate = 25.50m
            };

            _controller.ModelState.AddModelError("HoursWorked", "Hours must be greater than 0");

            // Act
            var result = await _controller.SubmitClaim(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task VerifyClaims_ShouldReturnViewWithPendingClaims()
        {
            // Arrange
            var pendingClaims = new List<Claim>
            {
                new Claim { ClaimId = 1, LecturerId = 1, HoursWorked = 40, HourlyRate = 25.50m, Status = "Pending" },
                new Claim { ClaimId = 2, LecturerId = 2, HoursWorked = 35, HourlyRate = 30.00m, Status = "Pending" }
            };

            _mockClaimService.Setup(s => s.GetPendingClaimsAsync()).ReturnsAsync(pendingClaims);

            // Act
            var result = await _controller.VerifyClaims();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Claim>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task ApproveClaim_ShouldRedirectToVerifyClaims()
        {
            // Arrange
            var claimId = 1;
            var claim = new Claim { ClaimId = claimId, Status = "Approved" };
            _mockClaimService.Setup(s => s.ApproveClaimAsync(claimId, "System Admin")).ReturnsAsync(claim);

            // Act
            var result = await _controller.ApproveClaim(claimId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("VerifyClaims", redirectResult.ActionName);
        }

        [Fact]
        public async Task RejectClaim_WithValidReason_ShouldRedirectToVerifyClaims()
        {
            // Arrange
            var claimId = 1;
            var rejectionReason = "Insufficient documentation";
            var claim = new Claim { ClaimId = claimId, Status = "Rejected", RejectionReason = rejectionReason };
            _mockClaimService.Setup(s => s.RejectClaimAsync(claimId, "System Admin", rejectionReason)).ReturnsAsync(claim);

            // Act
            var result = await _controller.RejectClaim(claimId, rejectionReason);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("VerifyClaims", redirectResult.ActionName);
        }

        [Fact]
        public async Task RejectClaim_WithEmptyReason_ShouldRedirectToVerifyClaims()
        {
            // Arrange
            var claimId = 1;
            var rejectionReason = "";

            // Act
            var result = await _controller.RejectClaim(claimId, rejectionReason);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("VerifyClaims", redirectResult.ActionName);
        }

        [Fact]
        public async Task TrackClaims_ShouldReturnViewWithLecturerClaims()
        {
            // Arrange
            var lecturerId = 1;
            var claims = new List<Claim>
            {
                new Claim { ClaimId = 1, LecturerId = lecturerId, HoursWorked = 40, HourlyRate = 25.50m, Status = "Pending" },
                new Claim { ClaimId = 2, LecturerId = lecturerId, HoursWorked = 35, HourlyRate = 30.00m, Status = "Approved" }
            };

            _mockClaimService.Setup(s => s.GetClaimsByLecturerIdAsync(lecturerId)).ReturnsAsync(claims);

            // Act
            var result = await _controller.TrackClaims(lecturerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Claim>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task UploadDocuments_WithValidClaimId_ShouldReturnView()
        {
            // Arrange
            var claimId = 1;
            var claim = new Claim { ClaimId = claimId, LecturerId = 1, HoursWorked = 40, HourlyRate = 25.50m };
            var documents = new List<Document>();

            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId)).ReturnsAsync(claim);
            _mockDocumentService.Setup(s => s.GetDocumentsByClaimIdAsync(claimId)).ReturnsAsync(documents);

            // Act
            var result = await _controller.UploadDocuments(claimId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(claim, _controller.ViewBag.Claim);
            Assert.Equal(documents, _controller.ViewBag.Documents);
        }

        [Fact]
        public async Task UploadDocuments_WithInvalidClaimId_ShouldRedirectToIndex()
        {
            // Arrange
            var claimId = 999;
            _mockClaimService.Setup(s => s.GetClaimByIdAsync(claimId)).ReturnsAsync((Claim?)null);

            // Act
            var result = await _controller.UploadDocuments(claimId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}
