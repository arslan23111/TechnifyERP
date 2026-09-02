using TechnifyERP.Domain.Enums;
namespace TechnifyERP.Application.Notices;
public sealed record NoticeDto(int Id,string Title,string Message,NoticeAudience Audience,int? CourseId,string? CourseCode,string? StudentUserId,string? StudentName,string? ImagePath,string CreatedBy,DateTime CreatedAtUtc);
public sealed record SaveNoticeRequest(string Title,string Message,NoticeAudience Audience,int? CourseId,string? StudentUserId,string? ImagePath);
public enum NoticeActionResult{Success,NotFound,Forbidden,Invalid}
public interface INoticeService
{
 Task<IReadOnlyList<NoticeDto>> GetCreatedAsync(string creatorId,CancellationToken ct=default);
 Task<NoticeActionResult> CreateAdminAsync(string adminId,SaveNoticeRequest request,CancellationToken ct=default);
 Task<NoticeActionResult> CreateFacultyAsync(string facultyId,SaveNoticeRequest request,CancellationToken ct=default);
 Task<NoticeActionResult> DeleteAsync(string creatorId,int id,CancellationToken ct=default);
 Task<IReadOnlyList<NoticeDto>> GetStudentNoticesAsync(string studentId,CancellationToken ct=default);
}
