using Xunit;
using CMCS.Models;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Tests.Models
{
    public class ClaimTests
    {
        [Fact]
        public void TotalAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 25.50m
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(1020.00m, totalAmount);
        }

        [Fact]
        public void TotalAmount_WithZeroHours_ShouldReturnZero()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 0,
                HourlyRate = 25.50m
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(0m, totalAmount);
        }

        [Fact]
        public void TotalAmount_WithZeroRate_ShouldReturnZero()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 0
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(0m, totalAmount);
        }

        [Fact]
        public void Status_ShouldDefaultToPending()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            Assert.Equal("Pending", claim.Status);
        }

        [Fact]
        public void SubmittedDate_ShouldDefaultToCurrentDate()
        {
            // Arrange & Act
            var claim = new Claim();
            var currentDate = DateTime.Now.Date;

            // Assert
            Assert.Equal(currentDate, claim.SubmittedDate.Date);
        }

        [Fact]
        public void Documents_ShouldInitializeAsEmptyList()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            Assert.NotNull(claim.Documents);
            Assert.Empty(claim.Documents);
        }

        [Theory]
        [InlineData(0.1, 25.50, true)]
        [InlineData(0, 25.50, false)]
        [InlineData(-1, 25.50, false)]
        [InlineData(40, 0.01, true)]
        [InlineData(40, 0, false)]
        [InlineData(40, -1, false)]
        public void Validation_ShouldValidateHoursAndRate(decimal hours, decimal rate, bool shouldBeValid)
        {
            // Arrange
            var claim = new Claim
            {
                LecturerId = 1,
                HoursWorked = hours,
                HourlyRate = rate
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.Equal(shouldBeValid, isValid);
        }

        [Fact]
        public void Notes_ShouldAllowNullValue()
        {
            // Arrange & Act
            var claim = new Claim
            {
                LecturerId = 1,
                HoursWorked = 40,
                HourlyRate = 25.50m,
                Notes = null
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void Notes_ShouldValidateMaxLength()
        {
            // Arrange
            var claim = new Claim
            {
                LecturerId = 1,
                HoursWorked = 40,
                HourlyRate = 25.50m,
                Notes = new string('A', 501) // 501 characters
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Notes"));
        }
    }
}
