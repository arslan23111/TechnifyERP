using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Attendance;
using TechnifyERP.Application.StudentDashboard;
using TechnifyERP.ViewModels.Student;

namespace TechnifyERP.Areas.Student.Controllers;

[Area("Student")]
[Authorize(Roles = "Student")]
public sealed class DashboardController(
    IAttendanceService attendanceService,
    IStudentDashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated user id is missing.");
        var attendance = await attendanceService.GetStudentReportAsync(userId, cancellationToken);
        var dashboard = await dashboardService.GetAsync(userId, cancellationToken);
        return attendance is null || dashboard is null
            ? NotFound()
            : View(new StudentDashboardPageViewModel(dashboard, attendance));
    }
}
