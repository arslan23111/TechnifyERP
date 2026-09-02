using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.AspNetCore.Mvc.Rendering;using TechnifyERP.Application.Doubts;
namespace TechnifyERP.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles="Student")]
public sealed class DoubtsController(IDoubtService service):Controller
{
 public async Task<IActionResult>Index(CancellationToken ct){ViewBag.Courses=(await service.GetStudentCoursesAsync(UserId,ct)).Select(x=>new SelectListItem($"{x.Code} — {x.Name}",x.Id.ToString()));return View(await service.GetStudentDoubtsAsync(UserId,ct));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Ask(int courseId,string question,CancellationToken ct){var r=await service.AskAsync(UserId,courseId,question,ct);TempData[r==DoubtActionResult.Success?"SuccessMessage":"ErrorMessage"]=r==DoubtActionResult.Success?"Question instructor ko bhej diya gaya.":"Course aur question check karein.";return RedirectToAction(nameof(Index));}
 private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException();
}
