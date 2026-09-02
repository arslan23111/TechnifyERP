using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Attendance;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public sealed class AttendanceController(IAttendanceService attendanceService) : Controller
{
    public async Task<IActionResult> Index(int? courseId, CancellationToken cancellationToken) =>
        View(await attendanceService.GetAdminReportAsync(courseId, cancellationToken));
}
