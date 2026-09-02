using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Admin;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class FacultyController(IAdminDirectoryService directoryService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await directoryService.GetFacultyAsync(cancellationToken));

    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        var faculty = await directoryService.GetFacultyMemberAsync(id, cancellationToken);
        return faculty is null ? NotFound() : View(faculty);
    }
}
