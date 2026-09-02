using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Admin;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class EnrollmentsController(IAdminDirectoryService directoryService, TechnifyERP.Application.Enrollments.IEnrollmentService enrollmentService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await directoryService.GetEnrollmentsAsync(cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(int id, bool approve, CancellationToken cancellationToken)
    {
        var result = await enrollmentService.ReviewAsync(id, approve, cancellationToken);
        TempData[result == TechnifyERP.Application.Enrollments.EnrollmentActionResult.Success ? "SuccessMessage" : "ErrorMessage"] = result switch
        {
            TechnifyERP.Application.Enrollments.EnrollmentActionResult.Success => approve ? "Enrollment approve ho gaya aur fee record generate ho gaya." : "Enrollment reject ho gaya.",
            TechnifyERP.Application.Enrollments.EnrollmentActionResult.AlreadyReviewed => "Enrollment pehle se review ho chuka hai.",
            _ => "Enrollment request nahi mili."
        };
        return RedirectToAction(nameof(Index));
    }
}
