using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Faculty;

namespace TechnifyERP.Areas.Faculty.Controllers;

[Area("Faculty")]
[Authorize(Roles = "Faculty")]
public sealed class DashboardController(IFacultyCourseService courseService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await courseService.GetDashboardAsync(UserId, cancellationToken));

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Authenticated user id is missing.");
}
