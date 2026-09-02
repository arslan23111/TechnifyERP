using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Fees;

namespace TechnifyERP.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student")]
public sealed class FeesController(IFeeService feeService, IWebHostEnvironment environment) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated user id is missing.");
        return View(await feeService.GetStudentReportAsync(userId, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReceipt(int feeRecordId, decimal amount, IFormFile? receipt, CancellationToken cancellationToken)
    {
        if (receipt is null || receipt.Length == 0 || receipt.Length > 5 * 1024 * 1024)
        {
            TempData["ErrorMessage"] = "Receipt required hai aur maximum size 5 MB honi chahiye.";
            return RedirectToAction(nameof(Index));
        }

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(receipt.FileName);
        if (!allowed.Contains(extension))
        {
            TempData["ErrorMessage"] = "Sirf PDF, JPG, JPEG ya PNG receipt allowed hai.";
            return RedirectToAction(nameof(Index));
        }

        var folder = Path.Combine(environment.WebRootPath, "uploads", "fee-receipts");
        Directory.CreateDirectory(folder);
        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        await using (var stream = System.IO.File.Create(Path.Combine(folder, fileName)))
            await receipt.CopyToAsync(stream, cancellationToken);

        var result = await feeService.SubmitReceiptAsync(UserId, feeRecordId, amount, $"/uploads/fee-receipts/{fileName}", cancellationToken);
        TempData[result == FeeActionResult.Success ? "SuccessMessage" : "ErrorMessage"] = result == FeeActionResult.Success
            ? "Receipt submit ho gayi. Admin approval ka wait karein."
            : "Receipt submit nahi hui. Fee record ya amount check karein.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestInstallment(int feeRecordId, decimal amount, string reason, CancellationToken cancellationToken)
    {
        var result = await feeService.RequestInstallmentAsync(UserId, feeRecordId, amount, reason, cancellationToken);
        TempData[result == FeeActionResult.Success ? "SuccessMessage" : "ErrorMessage"] = result switch
        {
            FeeActionResult.Success => "Installment request admin ko bhej di gayi hai.",
            FeeActionResult.AlreadyProcessed => "Is fee ke liye pending request pehle se mojood hai.",
            _ => "Installment amount/reason valid nahi hai. Amount total fee se kam hona chahiye."
        };
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Installments(CancellationToken cancellationToken) =>
        View(await feeService.GetInstallmentRequestsAsync(UserId, cancellationToken));

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Authenticated user id is missing.");
}
