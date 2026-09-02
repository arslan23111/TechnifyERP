using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using TechnifyERP.Application.Profiles;
namespace TechnifyERP.Areas.Student.Controllers;
[Area("Student"),Authorize(Roles="Student")]
public sealed class ProfileController(IProfileService service,IWebHostEnvironment env):Controller
{
 public async Task<IActionResult>Index(CancellationToken ct){var p=await service.GetStudentAsync(Id,ct);return p is null?NotFound():View(p);}
 [HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult>Update(string fullName,string? phoneNumber,string? address,string? gender,DateOnly? dateOfBirth,IFormFile? picture,CancellationToken ct){string?path=null;if(picture is not null){if(picture.Length<=0||picture.Length>5*1024*1024){TempData["ErrorMessage"]="Picture max 5 MB honi chahiye.";return RedirectToAction(nameof(Index));}var ext=Path.GetExtension(picture.FileName);if(!new[]{".jpg",".jpeg",".png",".webp"}.Contains(ext,StringComparer.OrdinalIgnoreCase)){TempData["ErrorMessage"]="JPG, PNG ya WEBP image use karein.";return RedirectToAction(nameof(Index));}var dir=Path.Combine(env.WebRootPath,"uploads","profiles");Directory.CreateDirectory(dir);var name=$"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";await using var stream=System.IO.File.Create(Path.Combine(dir,name));await picture.CopyToAsync(stream,ct);path=$"/uploads/profiles/{name}";}var r=await service.UpdateStudentAsync(Id,new(fullName,phoneNumber,address,gender,dateOfBirth,path),ct);TempData[r==ProfileActionResult.Success?"SuccessMessage":"ErrorMessage"]=r==ProfileActionResult.Success?"Profile update ho gayi.":"Profile details valid nahi.";return RedirectToAction(nameof(Index));}
 private string Id=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
