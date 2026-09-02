using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Admin;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class StudentsController(IAdminDirectoryService directoryService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await directoryService.GetStudentsAsync(cancellationToken));

    public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
    {
        var student = await directoryService.GetStudentAsync(id, cancellationToken);
        return student is null ? NotFound() : View(student);
    }
}
