using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Mvc.Rendering;using TechnifyERP.Application.Assignments;using TechnifyERP.Application.Challenges;using TechnifyERP.Domain.Enums;
namespace TechnifyERP.Areas.Faculty.Controllers;
[Area("Faculty"),Authorize(Roles="Faculty")]
public sealed class ChallengesController(IChallengeService service,IAssignmentService assignments):Controller
{
 public async Task<IActionResult>Index(CancellationToken ct){ViewBag.Courses=(await assignments.GetFacultyCoursesAsync(UserId,ct)).Select(x=>new SelectListItem($"{x.Code} — {x.Name}",x.Id.ToString()));return View(await service.GetFacultyChallengesAsync(UserId,ct));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Create(int courseId,string title,string description,ChallengeDifficulty difficulty,string? inputFormat,string? outputFormat,string? sampleInput,string? sampleOutput,DateTime? dueAt,CancellationToken ct){var r=await service.CreateAsync(UserId,new(courseId,title,description,difficulty,inputFormat,outputFormat,sampleInput,sampleOutput,dueAt?.ToUniversalTime()),ct);TempData[r==ChallengeActionResult.Success?"SuccessMessage":"ErrorMessage"]=r==ChallengeActionResult.Success?"Challenge create ho gaya.":"Fields ya assigned course check karein.";return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Delete(int id,CancellationToken ct){await service.DeleteAsync(UserId,id,ct);return RedirectToAction(nameof(Index));}
 public async Task<IActionResult>Submissions(int id,CancellationToken ct){var challenge=(await service.GetFacultyChallengesAsync(UserId,ct)).FirstOrDefault(x=>x.Id==id);var rows=await service.GetSubmissionsAsync(UserId,id,ct);if(challenge is null||rows is null)return NotFound();ViewBag.Challenge=challenge;return View(rows);}
 private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException();
}
