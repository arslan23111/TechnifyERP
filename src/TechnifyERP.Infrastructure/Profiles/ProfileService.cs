using Microsoft.EntityFrameworkCore;using TechnifyERP.Application.Profiles;using TechnifyERP.Infrastructure.Persistence;
namespace TechnifyERP.Infrastructure.Profiles;
internal sealed class ProfileService(ApplicationDbContext db):IProfileService
{
 public async Task<StudentProfileDto?>GetStudentAsync(string id,CancellationToken ct=default)=>await(from u in db.Users.AsNoTracking() join p in db.StudentProfiles.AsNoTracking() on u.Id equals p.UserId where u.Id==id select new StudentProfileDto(u.FullName,u.Email??"",p.RegistrationNumber,u.PhoneNumber,u.Address,u.Gender,u.DateOfBirth,u.ProfilePicturePath)).FirstOrDefaultAsync(ct);
 public async Task<ProfileActionResult>UpdateStudentAsync(string id,UpdateStudentProfileRequest r,CancellationToken ct=default){if(string.IsNullOrWhiteSpace(r.FullName)||r.FullName.Trim().Length>150)return ProfileActionResult.Invalid;var u=await db.Users.FirstOrDefaultAsync(x=>x.Id==id,ct);if(u is null)return ProfileActionResult.NotFound;u.FullName=r.FullName.Trim();u.PhoneNumber=Clean(r.PhoneNumber);u.Address=Clean(r.Address);u.Gender=Clean(r.Gender);u.DateOfBirth=r.DateOfBirth;if(r.ProfilePicturePath is not null)u.ProfilePicturePath=r.ProfilePicturePath;await db.SaveChangesAsync(ct);return ProfileActionResult.Success;}
 private static string?Clean(string?x)=>string.IsNullOrWhiteSpace(x)?null:x.Trim();
}
