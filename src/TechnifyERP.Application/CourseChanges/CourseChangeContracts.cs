using TechnifyERP.Domain.Enums;
namespace TechnifyERP.Application.CourseChanges;
public sealed record CourseChangeDto(int Id,string StudentUserId,string? StudentName,string? RegistrationNumber,int FromCourseId,string FromCourseCode,string FromCourseName,decimal FromFee,int ToCourseId,string ToCourseCode,string ToCourseName,decimal ToFee,string Reason,CourseChangeStatus Status,string? AdminRemarks,DateTime RequestedAtUtc);
public enum CourseChangeActionResult { Success, NotFound, InvalidRequest, AlreadyReviewed }
public interface ICourseChangeService { Task<IReadOnlyList<CourseChangeDto>> GetForStudentAsync(string studentId,CancellationToken ct=default); Task<IReadOnlyList<CourseChangeDto>> GetAllAsync(CancellationToken ct=default); Task<CourseChangeActionResult> CreateAsync(string studentId,int fromCourseId,int toCourseId,string reason,CancellationToken ct=default); Task<CourseChangeActionResult> ReviewAsync(int id,bool approve,string? remarks,CancellationToken ct=default); }
