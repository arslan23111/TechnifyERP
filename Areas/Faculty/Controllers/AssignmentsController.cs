using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnifyERP.Application.Assignments;
using TechnifyERP.ViewModels.Faculty;

namespace TechnifyERP.Areas.Faculty.Controllers;

[Area("Faculty"), Authorize(Roles = "Faculty")]
public sealed class AssignmentsController(IAssignmentService service, IWebHostEnvironment environment) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await service.GetFacultyAssignmentsAsync(UserId, ct));
    public async Task<IActionResult> Create(CancellationToken ct) { var model = new AssignmentFormViewModel(); await Courses(model, ct); return View("Form", model); }
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = await service.GetFacultyAssignmentAsync(UserId, id, ct); if (item is null) return NotFound();
        var model = new AssignmentFormViewModel { Id=item.Id, CourseId=item.CourseId, Title=item.Title, Description=item.Description, DueAt=item.DueAtUtc.ToLocalTime(), TotalMarks=item.TotalMarks, AllowResubmission=item.AllowResubmission, ExistingAttachmentPath=item.AttachmentPath };
        await Courses(model, ct); return View("Form", model);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(AssignmentFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) { await Courses(model, ct); return View("Form", model); }
        var path = model.Attachment is null ? null : await SaveFile(model.Attachment, "assignment-files", ct);
        if (model.Attachment is not null && path is null) { ModelState.AddModelError(nameof(model.Attachment), "PDF, DOC, DOCX, ZIP, JPG ya PNG file max 10 MB allowed hai."); await Courses(model, ct); return View("Form", model); }
        var request = new SaveAssignmentRequest(model.CourseId, model.Title, model.Description, model.DueAt.ToUniversalTime(), model.TotalMarks, model.AllowResubmission, path);
        var result = model.Id == 0 ? (await service.CreateAsync(UserId, request, ct)).Result : await service.UpdateAsync(UserId, model.Id, request, ct);
        if (result != AssignmentActionResult.Success) { ModelState.AddModelError(string.Empty, "Assignment save nahi hua. Course assignment aur fields check karein."); await Courses(model, ct); return View("Form", model); }
        TempData["SuccessMessage"]="Assignment save ho gaya."; return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id, CancellationToken ct) { await service.DeleteAsync(UserId,id,ct); return RedirectToAction(nameof(Index)); }
    public async Task<IActionResult> Submissions(int id, CancellationToken ct) { var assignment=await service.GetFacultyAssignmentAsync(UserId,id,ct); var items=await service.GetSubmissionsAsync(UserId,id,ct); if(assignment is null||items is null)return NotFound(); ViewBag.Assignment=assignment; return View(items); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Grade(int assignmentId,int submissionId,decimal marks,string? feedback,CancellationToken ct){var result=await service.GradeAsync(UserId,submissionId,marks,feedback,ct);TempData[result==AssignmentActionResult.Success?"SuccessMessage":"ErrorMessage"]=result==AssignmentActionResult.Success?"Grade save ho gaya.":"Marks valid range mein nahi hain.";return RedirectToAction(nameof(Submissions),new{id=assignmentId});}
    public async Task<IActionResult> Export(int id,CancellationToken ct){var assignment=await service.GetFacultyAssignmentAsync(UserId,id,ct);var rows=await service.GetSubmissionsAsync(UserId,id,ct);if(assignment is null||rows is null)return NotFound();var csv=new StringBuilder("Registration Number,Student,Marks,Percentage,Feedback\r\n");foreach(var x in rows){var percent=x.Marks.HasValue?x.Marks.Value*100/assignment.TotalMarks:0;csv.AppendLine($"\"{x.RegistrationNumber}\",\"{x.StudentName.Replace("\"","\"\"")}\",{x.Marks},{percent:0.##},\"{(x.Feedback??"").Replace("\"","\"\"")}\"");}return File(Encoding.UTF8.GetBytes(csv.ToString()),"text/csv",$"{assignment.CourseCode}-assignment-{id}-scores.csv");}
    private async Task Courses(AssignmentFormViewModel model,CancellationToken ct)=>model.Courses=(await service.GetFacultyCoursesAsync(UserId,ct)).Select(x=>new SelectListItem($"{x.Code} — {x.Name}",x.Id.ToString())).ToList();
    private async Task<string?> SaveFile(IFormFile file,string folderName,CancellationToken ct){if(file.Length<=0||file.Length>10*1024*1024)return null;var ext=Path.GetExtension(file.FileName);var allowed=new[]{".pdf",".doc",".docx",".zip",".jpg",".jpeg",".png"};if(!allowed.Contains(ext,StringComparer.OrdinalIgnoreCase))return null;var folder=Path.Combine(environment.WebRootPath,"uploads",folderName);Directory.CreateDirectory(folder);var name=$"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";await using var stream=System.IO.File.Create(Path.Combine(folder,name));await file.CopyToAsync(stream,ct);return $"/uploads/{folderName}/{name}";}
    private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException("User id missing.");
}
