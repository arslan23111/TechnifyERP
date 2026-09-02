using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using TechnifyERP.Application.Schedules;
namespace TechnifyERP.Areas.Admin.Controllers;
[Area("Admin"),Authorize(Roles="SuperAdmin")]
public sealed class ScheduleController(IScheduleService service):Controller
{
 public async Task<IActionResult>Index(CancellationToken ct){ViewBag.Options=await service.GetOptionsAsync(ct);return View(await service.GetAllAsync(ct));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Create(string assignment,DayOfWeek dayOfWeek,TimeOnly startTime,TimeOnly endTime,string room,CancellationToken ct){var parts=(assignment??"").Split('|');if(parts.Length!=2||!int.TryParse(parts[0],out var courseId)){TempData["ErrorMessage"]="Course/Faculty select karein.";return RedirectToAction(nameof(Index));}var r=await service.CreateAsync(new(courseId,parts[1],dayOfWeek,startTime,endTime,room),ct);TempData[r==ScheduleActionResult.Success?"SuccessMessage":"ErrorMessage"]=r switch{ScheduleActionResult.Success=>"Class schedule add ho gaya.",ScheduleActionResult.Conflict=>"Faculty ya room ka schedule is time overlap kar raha hai.",ScheduleActionResult.FacultyNotAssigned=>"Faculty is course ko assigned nahi hai.",_=>"Timing aur fields check karein."};return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Delete(int id,CancellationToken ct){await service.DeleteAsync(id,ct);return RedirectToAction(nameof(Index));}
}
