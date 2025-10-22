using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class ClaimSubmissionViewModel
    {
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Hours worked must be greater than 0")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hourly rate must be greater than 0")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount => HoursWorked * HourlyRate;

        public List<IFormFile>? SupportingDocuments { get; set; }
    }
}
