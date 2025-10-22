using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;

namespace CMCS.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IDocumentService _documentService;
        private readonly ILogger<CoordinatorController> _logger;

        public CoordinatorController(IClaimService claimService, IDocumentService documentService, ILogger<CoordinatorController> logger)
        {
            _claimService = claimService;
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var pendingClaims = await _claimService.GetPendingClaimsAsync();
                return View(pendingClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claims for coordinator");
                TempData["Error"] = "An error occurred while loading claims.";
                return View(new List<Claim>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyClaim(int claimId)
        {
            try
            {
                await _claimService.ApproveClaimAsync(claimId, "Programme Coordinator");
                TempData["Success"] = "Claim verified successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying claim {ClaimId}", claimId);
                TempData["Error"] = "An error occurred while verifying the claim.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectClaim(int claimId, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                TempData["Error"] = "Rejection reason is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _claimService.RejectClaimAsync(claimId, "Programme Coordinator", rejectionReason);
                TempData["Success"] = "Claim rejected successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting claim {ClaimId}", claimId);
                TempData["Error"] = "An error occurred while rejecting the claim.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(documentId);
                if (document == null)
                {
                    return NotFound();
                }

                var decryptedFileBytes = await _documentService.DecryptFileAsync(document.FilePath);
                return File(decryptedFileBytes, document.ContentType, document.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document {DocumentId}", documentId);
                return NotFound();
            }
        }
    }
}
