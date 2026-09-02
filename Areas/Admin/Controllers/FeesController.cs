using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Fees;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class FeesController(IFeeService feeService) : Controller
{
    public async Task<IActionResult> Index(int? month, int? year, CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        return View(await feeService.GetAdminReportAsync(month ?? today.Month, year ?? today.Year, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(int id, decimal amount, int month, int year, CancellationToken cancellationToken)
    {
        var result = await feeService.MarkPaidAsync(id, amount, cancellationToken);
        TempData[result == MarkFeePaidResult.Success ? "SuccessMessage" : "ErrorMessage"] = result switch
        {
            MarkFeePaidResult.Success => "Fee payment saved successfully.",
            MarkFeePaidResult.InvalidAmount => "Payment amount must be greater than zero and not exceed amount due.",
            _ => "Fee record not found."
        };
        return RedirectToAction(nameof(Index), new { month, year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewReceipt(int id, bool approve, string? remarks, int month, int year, CancellationToken cancellationToken)
    {
        var result = await feeService.ReviewReceiptAsync(id, approve, remarks, cancellationToken);
        TempData[result == FeeActionResult.Success ? "SuccessMessage" : "ErrorMessage"] = result == FeeActionResult.Success
            ? $"Payment receipt {(approve ? "approve" : "reject")} ho gayi."
            : "Receipt already process ho chuki hai ya record nahi mila.";
        return RedirectToAction(nameof(Index), new { month, year });
    }

    public async Task<IActionResult> Installments(CancellationToken cancellationToken) =>
        View(await feeService.GetInstallmentRequestsAsync(cancellationToken: cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewInstallment(int id, bool approve, string? remarks, CancellationToken cancellationToken)
    {
        var result = await feeService.ReviewInstallmentAsync(id, approve, remarks, cancellationToken);
        TempData[result == FeeActionResult.Success ? "SuccessMessage" : "ErrorMessage"] = result == FeeActionResult.Success
            ? $"Installment request {(approve ? "approve" : "reject")} ho gayi."
            : "Request already process ho chuki hai ya record nahi mila.";
        return RedirectToAction(nameof(Installments));
    }
}
