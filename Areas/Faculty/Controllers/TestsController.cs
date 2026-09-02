using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Mvc.Rendering;using TechnifyERP.Application.Assignments;using TechnifyERP.Application.Tests;
namespace TechnifyERP.Areas.Faculty.Controllers;
[Area("Faculty"),Authorize(Roles="Faculty")]
public sealed class TestsController(ITestService tests,IAssignmentService assignments):Controller
{
 public async Task<IActionResult> Index(CancellationToken ct)=>View(await tests.GetFacultyTestsAsync(UserId,ct));
 public async Task<IActionResult> Create(CancellationToken ct){ViewBag.Courses=(await assignments.GetFacultyCoursesAsync(UserId,ct)).Select(x=>new SelectListItem($"{x.Code} — {x.Name}",x.Id.ToString()));return View();}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Create(int courseId,string title,int durationMinutes,DateTime startsAt,DateTime endsAt,CancellationToken ct){var r=await tests.CreateAsync(UserId,new(courseId,title,durationMinutes,startsAt.ToUniversalTime(),endsAt.ToUniversalTime()),ct);if(r.Result==TestActionResult.Success)return RedirectToAction(nameof(Questions),new{id=r.Id});TempData["ErrorMessage"]="Test fields check karein.";return RedirectToAction(nameof(Create));}
 public async Task<IActionResult>Questions(int id,CancellationToken ct){var test=await tests.GetFacultyTestAsync(UserId,id,ct);var items=await tests.GetQuestionsAsync(UserId,id,ct);if(test is null||items is null)return NotFound();ViewBag.Test=test;return View(items);}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>AddQuestion(int testId,string text,string optionA,string optionB,string optionC,string optionD,char correctOption,decimal marks,CancellationToken ct){await tests.AddQuestionAsync(UserId,testId,new(text,optionA,optionB,optionC,optionD,correctOption,marks),ct);return RedirectToAction(nameof(Questions),new{id=testId});}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>DeleteQuestion(int testId,int questionId,CancellationToken ct){await tests.DeleteQuestionAsync(UserId,questionId,ct);return RedirectToAction(nameof(Questions),new{id=testId});}
 private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException();
}
