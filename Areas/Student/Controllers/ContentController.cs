using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using TechnifyERP.Application.Content;
namespace TechnifyERP.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles="Student")]
public sealed class ContentController(IContentService content):Controller
{
 public async Task<IActionResult>Index(CancellationToken ct){ViewBag.Materials=await content.GetStudentMaterialsAsync(UserId,ct);ViewBag.Lectures=await content.GetStudentLecturesAsync(UserId,ct);return View();}
 private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new InvalidOperationException();
}
