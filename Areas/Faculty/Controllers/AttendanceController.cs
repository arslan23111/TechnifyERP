using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnifyERP.Application.Attendance;
using TechnifyERP.ViewModels.Faculty;

namespace TechnifyERP.Areas.Faculty.Controllers;

[Area("Faculty")]
[Authorize(Roles = "Faculty")]
public sealed class AttendanceController(IAttendanceService attendanceService) : Controller
{
    public async Task<IActionResult> Index(
        int? courseId,
        DateOnly? date,
        CancellationToken cancellationToken)
    {
        var model = new AttendancePageViewModel
        {
            CourseId = courseId ?? 0,
            AttendanceDate = date ?? DateOnly.FromDateTime(DateTime.Today)
        };
        await PopulateCoursesAsync(model, cancellationToken);

        if (courseId is > 0)
        {
            var sheet = await attendanceService.GetSheetAsync(UserId, courseId.Value, model.AttendanceDate, cancellationToken);
            if (sheet is null)
            {
                return NotFound();
            }

            model.SheetLoaded = true;
            model.CourseCode = sheet.CourseCode;
            model.CourseName = sheet.CourseName;
            model.Items = sheet.Students.Select(student => new AttendanceItemViewModel
            {
                StudentUserId = student.StudentUserId,
                RegistrationNumber = student.RegistrationNumber,
                FullName = student.FullName,
                Status = student.Status
            }).ToList();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(AttendancePageViewModel model, CancellationToken cancellationToken)
    {
        if (model.Items.Count == 0 || model.Items.Any(item => item.Status is null))
        {
            TempData["ErrorMessage"] = "Please mark Present, Absent, or Leave for every student.";
            return RedirectToAction(nameof(Index), new { courseId = model.CourseId, date = model.AttendanceDate.ToString("yyyy-MM-dd") });
        }

        var result = await attendanceService.SaveAsync(
            UserId,
            model.CourseId,
            model.AttendanceDate,
            model.Items.Select(item => new SaveAttendanceItem(item.StudentUserId, item.Status!.Value)).ToList(),
            cancellationToken);

        TempData[result == SaveAttendanceResult.Success ? "SuccessMessage" : "ErrorMessage"] = result switch
        {
            SaveAttendanceResult.Success => "Attendance saved successfully.",
            SaveAttendanceResult.CourseNotAssigned => "You are not assigned to this course.",
            SaveAttendanceResult.InvalidStudent => "The submitted attendance contains an invalid student.",
            _ => "No attendance records were submitted."
        };

        return RedirectToAction(nameof(Index), new { courseId = model.CourseId, date = model.AttendanceDate.ToString("yyyy-MM-dd") });
    }

    private async Task PopulateCoursesAsync(AttendancePageViewModel model, CancellationToken cancellationToken)
    {
        var courses = await attendanceService.GetAssignedCoursesAsync(UserId, cancellationToken);
        model.Courses = courses.Select(course => new SelectListItem(
            $"{course.CourseCode} — {course.CourseName}",
            course.CourseId.ToString())).ToList();
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Authenticated user id is missing.");
}
