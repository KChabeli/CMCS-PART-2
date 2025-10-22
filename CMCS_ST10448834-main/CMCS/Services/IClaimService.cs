using CMCS.Models;

namespace CMCS.Services
{
    public interface IClaimService
    {
        Task<List<Claim>> GetAllClaimsAsync();
        Task<List<Claim>> GetClaimsByLecturerIdAsync(int lecturerId);
        Task<List<Claim>> GetPendingClaimsAsync();
        Task<Claim?> GetClaimByIdAsync(int claimId);
        Task<Claim> SubmitClaimAsync(Claim claim);
        Task<Claim> ApproveClaimAsync(int claimId, string processedBy);
        Task<Claim> RejectClaimAsync(int claimId, string processedBy, string rejectionReason);
        Task<List<Claim>> GetClaimsByStatusAsync(string status);
        Task<decimal> GetTotalAmountByLecturerAsync(int lecturerId);
        Task<int> GetClaimCountByStatusAsync(string status);
    }
}
