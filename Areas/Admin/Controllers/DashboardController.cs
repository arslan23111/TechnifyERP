using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Admin;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public class DashboardController(IAdminDirectoryService directoryService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await directoryService.GetDashboardAsync(cancellationToken));
}
