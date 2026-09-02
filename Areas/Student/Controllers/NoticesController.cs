using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using TechnifyERP.Application.Notices;
namespace TechnifyERP.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles="Student")]
public sealed class NoticesController(INoticeService notices):Controller{public async Task<IActionResult>Index(CancellationToken ct)=>View(await notices.GetStudentNoticesAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!,ct));}
