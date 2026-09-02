using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnifyERP.Application.Accounts;
using TechnifyERP.Application.Courses;
using TechnifyERP.ViewModels.Account;

namespace TechnifyERP.Controllers;

[AllowAnonymous]
public sealed class AccountController(
    IRegistrationService registrationService,
    ICourseService courseService) : Controller
{
    public IActionResult Register() => View();

    public async Task<IActionResult> RegisterStudent(CancellationToken cancellationToken)
    {
        var model = new StudentRegistrationViewModel();
        await PopulateCoursesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterStudent(
        StudentRegistrationViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCoursesAsync(model, cancellationToken);
            return View(model);
        }

        var result = await registrationService.RegisterStudentAsync(
            new RegisterStudentRequest(
                model.FullName,
                model.Email,
                model.PhoneNumber,
                model.Address,
                model.Gender,
                model.Cnic,
                model.DateOfBirth,
                model.CourseId,
                model.Password),
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await PopulateCoursesAsync(model, cancellationToken);
            return View(model);
        }

        return RedirectToAction(nameof(RegistrationSubmitted));
    }

    public IActionResult RegisterFaculty() => View(new FacultyRegistrationViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterFaculty(
        FacultyRegistrationViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await registrationService.RegisterFacultyAsync(
            new RegisterFacultyRequest(
                model.FullName,
                model.Email,
                model.PhoneNumber,
                model.Address,
                model.Gender,
                model.Cnic,
                model.DateOfBirth,
                model.Designation,
                model.Department,
                model.JoiningDate,
                model.Password),
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        return RedirectToAction(nameof(RegistrationSubmitted));
    }

    public IActionResult RegistrationSubmitted() => View();

    private async Task PopulateCoursesAsync(
        StudentRegistrationViewModel model,
        CancellationToken cancellationToken)
    {
        var courses = await courseService.GetAllAsync(cancellationToken);
        model.Courses = courses
            .Select(course => new SelectListItem(
                $"{course.CourseCode} — {course.Name} (Rs. {course.MonthlyFee:N2}/month)",
                course.Id.ToString()))
            .ToList();
    }
}
