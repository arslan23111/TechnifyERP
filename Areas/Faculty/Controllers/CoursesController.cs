using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Faculty;

namespace TechnifyERP.Areas.Faculty.Controllers;

[Area("Faculty")]
[Authorize(Roles = "Faculty")]
public sealed class CoursesController(IFacultyCourseService courseService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await courseService.GetCoursesAsync(UserId, cancellationToken));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int id, CancellationToken cancellationToken)
    {
        var result = await courseService.AssignAsync(UserId, id, cancellationToken);
        TempData[result == FacultyCourseActionResult.Success ? "SuccessMessage" : "ErrorMessage"] = result switch
        {
            FacultyCourseActionResult.Success => "Course assigned successfully.",
            FacultyCourseActionResult.AlreadyAssigned => "This course is already assigned to you.",
            _ => "Course could not be assigned."
        };
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Students(int id, CancellationToken cancellationToken)
    {
        var courses = await courseService.GetCoursesAsync(UserId, cancellationToken);
        var course = courses.FirstOrDefault(item => item.CourseId == id && item.IsAssigned);
        if (course is null)
        {
            return NotFound();
        }

        ViewBag.Course = course;
        return View(await courseService.GetEnrolledStudentsAsync(UserId, id, cancellationToken));
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("Authenticated user id is missing.");
}
