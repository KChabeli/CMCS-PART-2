using CMCS.Models;

namespace CMCS.Services
{
    public class ClaimService : IClaimService
    {
        private static List<Claim> _claims = new List<Claim>();
        private static int _nextClaimId = 1;

        public async Task<List<Claim>> GetAllClaimsAsync()
        {
            return await Task.FromResult(_claims.ToList());
        }

        public async Task<List<Claim>> GetClaimsByLecturerIdAsync(int lecturerId)
        {
            return await Task.FromResult(_claims.Where(c => c.LecturerId == lecturerId).ToList());
        }

        public async Task<List<Claim>> GetPendingClaimsAsync()
        {
            return await Task.FromResult(_claims.Where(c => c.Status == "Pending").ToList());
        }

        public async Task<Claim?> GetClaimByIdAsync(int claimId)
        {
            return await Task.FromResult(_claims.FirstOrDefault(c => c.ClaimId == claimId));
        }

        public async Task<Claim> SubmitClaimAsync(Claim claim)
        {
            claim.ClaimId = _nextClaimId++;
            claim.Status = "Pending";
            claim.SubmittedDate = DateTime.Now;

            _claims.Add(claim);
            return await Task.FromResult(claim);
        }

        public async Task<Claim> ApproveClaimAsync(int claimId, string processedBy)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
                throw new ArgumentException("Claim not found");

            claim.Status = "Approved";
            claim.ProcessedDate = DateTime.Now;
            claim.ProcessedBy = processedBy;
            claim.RejectionReason = null;

            return await Task.FromResult(claim);
        }

        public async Task<Claim> RejectClaimAsync(int claimId, string processedBy, string rejectionReason)
        {
            var claim = _claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
                throw new ArgumentException("Claim not found");

            claim.Status = "Rejected";
            claim.ProcessedDate = DateTime.Now;
            claim.ProcessedBy = processedBy;
            claim.RejectionReason = rejectionReason;

            return await Task.FromResult(claim);
        }

        public async Task<List<Claim>> GetClaimsByStatusAsync(string status)
        {
            return await Task.FromResult(_claims.Where(c => c.Status == status).ToList());
        }

        public async Task<decimal> GetTotalAmountByLecturerAsync(int lecturerId)
        {
            var total = _claims
                .Where(c => c.LecturerId == lecturerId && c.Status == "Approved")
                .Sum(c => c.TotalAmount);

            return await Task.FromResult(total);
        }

        public async Task<int> GetClaimCountByStatusAsync(string status)
        {
            return await Task.FromResult(_claims.Count(c => c.Status == status));
        }
    }
}
