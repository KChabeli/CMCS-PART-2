using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CMCS.Models;
using CMCS.Services;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IDocumentService _documentService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IClaimService claimService, IDocumentService documentService, ILogger<HomeController> logger)
        {
            _claimService = claimService;
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var pendingCount = await _claimService.GetClaimCountByStatusAsync("Pending");
                var approvedCount = await _claimService.GetClaimCountByStatusAsync("Approved");
                var rejectedCount = await _claimService.GetClaimCountByStatusAsync("Rejected");

                ViewBag.PendingCount = pendingCount;
                ViewBag.ApprovedCount = approvedCount;
                ViewBag.RejectedCount = rejectedCount;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View(new ClaimSubmissionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(ClaimSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // For demo purposes, using lecturer ID 1
                var claim = new Claim
                {
                    LecturerId = 1,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Notes = model.Notes
                };

                var submittedClaim = await _claimService.SubmitClaimAsync(claim);

                // Handle file uploads if any
                if (model.SupportingDocuments != null && model.SupportingDocuments.Any())
                {
                    foreach (var file in model.SupportingDocuments)
                    {
                        if (file.Length > 0)
                        {
                            await _documentService.UploadDocumentAsync(submittedClaim.ClaimId, file);
                        }
                    }
                }

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(TrackClaims));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting claim");
                ModelState.AddModelError("", "An error occurred while submitting the claim. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerifyClaims()
        {
            try
            {
                var pendingClaims = await _claimService.GetPendingClaimsAsync();
                return View(pendingClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claims for verification");
                TempData["Error"] = "An error occurred while loading claims.";
                return View(new List<Claim>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            try
            {
                await _claimService.ApproveClaimAsync(claimId, "System Admin");
                TempData["Success"] = "Claim approved successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving claim {ClaimId}", claimId);
                TempData["Error"] = "An error occurred while approving the claim.";
            }

            return RedirectToAction(nameof(VerifyClaims));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectClaim(int claimId, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                TempData["Error"] = "Rejection reason is required.";
                return RedirectToAction(nameof(VerifyClaims));
            }

            try
            {
                await _claimService.RejectClaimAsync(claimId, "System Admin", rejectionReason);
                TempData["Success"] = "Claim rejected successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting claim {ClaimId}", claimId);
                TempData["Error"] = "An error occurred while rejecting the claim.";
            }

            return RedirectToAction(nameof(VerifyClaims));
        }

        [HttpGet]
        public async Task<IActionResult> UploadDocuments(int claimId)
        {
            try
            {
                var claim = await _claimService.GetClaimByIdAsync(claimId);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }

                var documents = await _documentService.GetDocumentsByClaimIdAsync(claimId);
                ViewBag.Claim = claim;
                ViewBag.Documents = documents;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading documents for claim {ClaimId}", claimId);
                TempData["Error"] = "An error occurred while loading documents.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocuments(int claimId, List<IFormFile> files, List<string> descriptions)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    TempData["Error"] = "Please select at least one file to upload.";
                    return RedirectToAction(nameof(UploadDocuments), new { claimId });
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    var description = i < descriptions.Count ? descriptions[i] : null;

                    if (file.Length > 0)
                    {
                        await _documentService.UploadDocumentAsync(claimId, file, description);
                    }
                }

                TempData["Success"] = "Documents uploaded successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading documents for claim {ClaimId}", claimId);
                TempData["Error"] = "An error occurred while uploading documents.";
            }

            return RedirectToAction(nameof(UploadDocuments), new { claimId });
        }

        [HttpGet]
        public async Task<IActionResult> TrackClaims(int lecturerId = 1)
        {
            try
            {
                var claims = await _claimService.GetClaimsByLecturerIdAsync(lecturerId);
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claims for tracking");
                TempData["Error"] = "An error occurred while loading claims.";
                return View(new List<Claim>());
            }
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

                var filePath = document.FilePath;
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                // Decrypt the file before returning
                var decryptedFileBytes = await _documentService.DecryptFileAsync(filePath);
                return File(decryptedFileBytes, document.ContentType, document.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document {DocumentId}", documentId);
                return NotFound();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
