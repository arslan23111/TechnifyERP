using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using TechnifyERP.Application.Tests;
namespace TechnifyERP.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles="Student")]
public sealed class TestsController(ITestService tests):Controller
{
 public async Task<IActionResult>Index(CancellationToken ct)=>View(await tests.GetStudentTestsAsync(UserId,ct));
 public async Task<IActionResult>Take(int id,CancellationToken ct){var m=await tests.GetAttemptAsync(UserId,id,ct);return m is null?RedirectToAction(nameof(Index)):View(m);}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Submit(int testId,Dictionary<int,string> answers,CancellationToken ct){var items=answers.Select(x=>new SubmittedAnswer(x.Key,string.IsNullOrEmpty(x.Value)?null:char.ToUpperInvariant(x.Value[0]))).ToList();var r=await tests.SubmitAsync(UserId,testId,items,ct);return r.Result==TestActionResult.Success?RedirectToAction(nameof(Result),new{id=r.ResultId}):RedirectToAction(nameof(Index));}
 public async Task<IActionResult>Result(int id,CancellationToken ct){var m=await tests.GetResultAsync(UserId,id,ct);return m is null?NotFound():View(m);}
 private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException();
}
