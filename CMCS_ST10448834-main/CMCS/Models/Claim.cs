using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }

        [Required]
        public int LecturerId { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Hours worked must be greater than 0")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hourly rate must be greater than 0")]
        public decimal HourlyRate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public DateTime? ProcessedDate { get; set; }

        public string? ProcessedBy { get; set; }

        public string? RejectionReason { get; set; }

        public decimal TotalAmount => HoursWorked * HourlyRate;

        // Navigation properties
        public Lecturer? Lecturer { get; set; }
        public List<Document>? Documents { get; set; } = new List<Document>();
    }
}
