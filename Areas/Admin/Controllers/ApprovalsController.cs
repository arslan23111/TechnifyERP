using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Accounts;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class ApprovalsController(IAccountApprovalService approvalService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await approvalService.GetPendingAsync(cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(string id, CancellationToken cancellationToken)
    {
        var result = await approvalService.ApproveAsync(id, cancellationToken);
        TempData[result == AccountReviewResult.Success ? "SuccessMessage" : "ErrorMessage"] =
            result == AccountReviewResult.Success ? "Account approved successfully." : "Account could not be approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(string id, CancellationToken cancellationToken)
    {
        var result = await approvalService.RejectAsync(id, cancellationToken);
        TempData[result == AccountReviewResult.Success ? "SuccessMessage" : "ErrorMessage"] =
            result == AccountReviewResult.Success ? "Account rejected." : "Account could not be rejected.";
        return RedirectToAction(nameof(Index));
    }
}
