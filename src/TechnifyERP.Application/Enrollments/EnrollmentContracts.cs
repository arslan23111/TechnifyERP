using TechnifyERP.Domain.Enums;
namespace TechnifyERP.Application.Enrollments;
public sealed record AvailableEnrollmentCourseDto(int Id,string Code,string Name,int CreditHours,decimal MonthlyFee);
public sealed record StudentEnrollmentRequestDto(int Id,int CourseId,string CourseCode,string CourseName,int CreditHours,decimal MonthlyFee,EnrollmentStatus Status,DateTime RequestedAtUtc,DateTime? ReviewedAtUtc);
public enum EnrollmentActionResult{Success,NotFound,Invalid,Duplicate,AlreadyReviewed}
public interface IEnrollmentService
{
 Task<IReadOnlyList<AvailableEnrollmentCourseDto>>GetAvailableCoursesAsync(string studentId,CancellationToken ct=default);
 Task<IReadOnlyList<StudentEnrollmentRequestDto>>GetStudentEnrollmentsAsync(string studentId,CancellationToken ct=default);
 Task<EnrollmentActionResult>RequestAsync(string studentId,int courseId,CancellationToken ct=default);
 Task<EnrollmentActionResult>ReviewAsync(int enrollmentId,bool approve,CancellationToken ct=default);
}
