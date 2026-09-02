using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Courses;
using TechnifyERP.ViewModels.Admin;

namespace TechnifyERP.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin")]
public class CoursesController(ICourseService courseService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await courseService.GetAllAsync(cancellationToken));

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var course = await courseService.GetByIdAsync(id, cancellationToken);
        return course is null ? NotFound() : View(course);
    }

    public IActionResult Create() => View(new CourseFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await courseService.CreateAsync(
            new CreateCourseRequest(model.CourseCode, model.Name, model.CreditHours, model.MonthlyFee),
            cancellationToken);

        if (result == SaveCourseResult.DuplicateCourseCode)
        {
            ModelState.AddModelError(nameof(model.CourseCode), "This course code is already in use.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Course created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var course = await courseService.GetByIdAsync(id, cancellationToken);
        return course is null
            ? NotFound()
            : View(new CourseFormViewModel
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                Name = course.Name,
                CreditHours = course.CreditHours,
                MonthlyFee = course.MonthlyFee
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await courseService.UpdateAsync(
            new UpdateCourseRequest(model.Id, model.CourseCode, model.Name, model.CreditHours, model.MonthlyFee),
            cancellationToken);

        if (result == SaveCourseResult.NotFound)
        {
            return NotFound();
        }

        if (result == SaveCourseResult.DuplicateCourseCode)
        {
            ModelState.AddModelError(nameof(model.CourseCode), "This course code is already in use.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Course updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var course = await courseService.GetByIdAsync(id, cancellationToken);
        return course is null ? NotFound() : View(course);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        if (!await courseService.DeleteAsync(id, cancellationToken))
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Course deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
