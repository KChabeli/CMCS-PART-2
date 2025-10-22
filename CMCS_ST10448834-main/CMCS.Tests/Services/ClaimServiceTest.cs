using Xunit;
using CMCS.Services;
using CMCS.Models;

namespace CMCS.Tests.Services
{
    public class ClaimServiceTests
    {
        private readonly IClaimService _claimService;

        public ClaimServiceTests()
        {
            _claimService = new ClaimService();
        }

        [Fact]
        public async Task SubmitClaimAsync_ShouldCreateNewClaim()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerId = 1,
                HoursWorked = 40,
                HourlyRate = 25.50m,
                Notes = "Test claim"
            };

            // Act
            var result = await _claimService.SubmitClaimAsync(claim);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ClaimId > 0);
            Assert.Equal("Pending", result.Status);
            Assert.Equal(DateTime.Now.Date, result.SubmittedDate.Date);
        }

        [Fact]
        public async Task ApproveClaimAsync_ShouldUpdateClaimStatus()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerId = 1,
                HoursWorked = 40,
                HourlyRate = 25.50m
            };
            var submittedClaim = await _claimService.SubmitClaimAsync(claim);

            // Act
            var result = await _claimService.ApproveClaimAsync(submittedClaim.ClaimId, "Test Admin");

            // Assert
            Assert.Equal("Approved", result.Status);
            Assert.Equal("Test Admin", result.ProcessedBy);
            Assert.NotNull(result.ProcessedDate);
        }

        [Fact]
        public async Task RejectClaimAsync_ShouldUpdateClaimStatus()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerId = 1,
                HoursWorked = 40,
                HourlyRate = 25.50m
            };
            var submittedClaim = await _claimService.SubmitClaimAsync(claim);
            var rejectionReason = "Insufficient documentation";

            // Act
            var result = await _claimService.RejectClaimAsync(submittedClaim.ClaimId, "Test Admin", rejectionReason);

            // Assert
            Assert.Equal("Rejected", result.Status);
            Assert.Equal("Test Admin", result.ProcessedBy);
            Assert.Equal(rejectionReason, result.RejectionReason);
            Assert.NotNull(result.ProcessedDate);
        }

        [Fact]
        public async Task GetPendingClaimsAsync_ShouldReturnOnlyPendingClaims()
        {
            // Arrange
            var claim1 = new Claim { LecturerId = 1, HoursWorked = 40, HourlyRate = 25.50m };
            var claim2 = new Claim { LecturerId = 2, HoursWorked = 35, HourlyRate = 30.00m };

            await _claimService.SubmitClaimAsync(claim1);
            var submittedClaim2 = await _claimService.SubmitClaimAsync(claim2);
            await _claimService.ApproveClaimAsync(submittedClaim2.ClaimId, "Test Admin");

            // Act
            var pendingClaims = await _claimService.GetPendingClaimsAsync();

            // Assert
            Assert.All(pendingClaims, c => Assert.Equal("Pending", c.Status));
        }

        [Fact]
        public async Task GetClaimsByLecturerIdAsync_ShouldReturnLecturerClaims()
        {
            // Arrange
            var lecturerId = 1;
            var claim1 = new Claim { LecturerId = lecturerId, HoursWorked = 40, HourlyRate = 25.50m };
            var claim2 = new Claim { LecturerId = lecturerId, HoursWorked = 35, HourlyRate = 30.00m };
            var claim3 = new Claim { LecturerId = 2, HoursWorked = 20, HourlyRate = 20.00m };

            await _claimService.SubmitClaimAsync(claim1);
            await _claimService.SubmitClaimAsync(claim2);
            await _claimService.SubmitClaimAsync(claim3);

            // Act
            var lecturerClaims = await _claimService.GetClaimsByLecturerIdAsync(lecturerId);

            // Assert
            Assert.Equal(2, lecturerClaims.Count);
            Assert.All(lecturerClaims, c => Assert.Equal(lecturerId, c.LecturerId));
        }

        [Fact]
        public async Task GetTotalAmountByLecturerAsync_ShouldCalculateCorrectTotal()
        {
            // Arrange
            var lecturerId = 1;
            var claim1 = new Claim { LecturerId = lecturerId, HoursWorked = 40, HourlyRate = 25.50m };
            var claim2 = new Claim { LecturerId = lecturerId, HoursWorked = 35, HourlyRate = 30.00m };
            var claim3 = new Claim { LecturerId = lecturerId, HoursWorked = 20, HourlyRate = 20.00m };

            var submittedClaim1 = await _claimService.SubmitClaimAsync(claim1);
            var submittedClaim2 = await _claimService.SubmitClaimAsync(claim2);
            var submittedClaim3 = await _claimService.SubmitClaimAsync(claim3);

            await _claimService.ApproveClaimAsync(submittedClaim1.ClaimId, "Test Admin");
            await _claimService.ApproveClaimAsync(submittedClaim2.ClaimId, "Test Admin");
            // claim3 remains pending

            // Act
            var totalAmount = await _claimService.GetTotalAmountByLecturerAsync(lecturerId);

            // Assert
            var expectedTotal = (40 * 25.50m) + (35 * 30.00m); // Only approved claims
            Assert.Equal(expectedTotal, totalAmount);
        }

        [Fact]
        public async Task ApproveClaimAsync_WithInvalidId_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _claimService.ApproveClaimAsync(999, "Test Admin"));
        }

        [Fact]
        public async Task RejectClaimAsync_WithInvalidId_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _claimService.RejectClaimAsync(999, "Test Admin", "Test reason"));
        }
    }
}
