using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnifyERP.Application.Assignments;

namespace TechnifyERP.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles="Student")]
public sealed class AssignmentsController(IAssignmentService service,IWebHostEnvironment environment):Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await service.GetStudentAssignmentsAsync(UserId,ct));
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Submit(int assignmentId,IFormFile? file,CancellationToken ct){if(file is null||file.Length<=0||file.Length>10*1024*1024){TempData["ErrorMessage"]="Submission file required hai aur max 10 MB honi chahiye.";return RedirectToAction(nameof(Index));}var ext=Path.GetExtension(file.FileName);var allowed=new[]{".pdf",".doc",".docx",".zip",".jpg",".jpeg",".png"};if(!allowed.Contains(ext,StringComparer.OrdinalIgnoreCase)){TempData["ErrorMessage"]="File type allowed nahi hai.";return RedirectToAction(nameof(Index));}var folder=Path.Combine(environment.WebRootPath,"uploads","assignment-submissions");Directory.CreateDirectory(folder);var name=$"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";await using(var stream=System.IO.File.Create(Path.Combine(folder,name)))await file.CopyToAsync(stream,ct);var result=await service.SubmitAsync(UserId,assignmentId,$"/uploads/assignment-submissions/{name}",ct);TempData[result==AssignmentActionResult.Success?"SuccessMessage":"ErrorMessage"]=result switch{AssignmentActionResult.Success=>"Assignment submit ho gaya.",AssignmentActionResult.Closed=>"Deadline close hai ya resubmission allowed nahi.",_=>"Submission save nahi hui."};return RedirectToAction(nameof(Index));}
    private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("User id missing.");
}
