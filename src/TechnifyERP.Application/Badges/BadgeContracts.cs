namespace TechnifyERP.Application.Badges;
public sealed record BadgeDto(int Id,string Name,string Description,string? IconPath,DateTime CreatedAtUtc);
public sealed record StudentBadgeDto(int Id,string BadgeName,string BadgeDescription,string? IconPath,string AwardedBy,DateTime AwardedAtUtc);
public sealed record AwardableStudentDto(string UserId,string FullName,string? RegistrationNumber,string CourseCode);
public enum BadgeActionResult{Success,NotFound,Forbidden,Invalid,Duplicate}
public interface IBadgeService{Task<IReadOnlyList<BadgeDto>>GetAllAsync(CancellationToken ct=default);Task<BadgeActionResult>CreateAsync(string name,string description,string? iconPath,CancellationToken ct=default);Task<BadgeActionResult>DeleteAsync(int id,CancellationToken ct=default);Task<IReadOnlyList<AwardableStudentDto>>GetAwardableStudentsAsync(string facultyId,CancellationToken ct=default);Task<BadgeActionResult>AwardAsync(string facultyId,int badgeId,string studentId,CancellationToken ct=default);Task<IReadOnlyList<StudentBadgeDto>>GetStudentBadgesAsync(string studentId,CancellationToken ct=default);}
