using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using TechnifyERP.Application.Challenges;
namespace TechnifyERP.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles="Student")]
public sealed class ChallengesController(IChallengeService service):Controller
{
 public async Task<IActionResult>Index(CancellationToken ct)=>View(await service.GetStudentChallengesAsync(UserId,ct));
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Submit(int challengeId,string language,string sourceCode,CancellationToken ct){var r=await service.SubmitAsync(UserId,challengeId,language,sourceCode,ct);TempData[r==ChallengeActionResult.Success?"SuccessMessage":"ErrorMessage"]=r switch{ChallengeActionResult.Success=>"Code submit ho gaya.",ChallengeActionResult.Closed=>"Challenge deadline close ho chuki hai.",_=>"Submission valid nahi."};return RedirectToAction(nameof(Index));}
 public async Task<IActionResult>History(int id,CancellationToken ct){ViewBag.ChallengeId=id;return View(await service.GetStudentSubmissionsAsync(UserId,id,ct));}
 private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException();
}
