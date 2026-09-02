namespace TechnifyERP.Application.Doubts;
public sealed record DoubtCourseDto(int Id,string Code,string Name);
public sealed record DoubtDto(int Id,int CourseId,string CourseCode,string CourseName,string StudentName,string? RegistrationNumber,string Question,DateTime AskedAtUtc,string? Answer,string? AnsweredBy,DateTime? AnsweredAtUtc);
public enum DoubtActionResult{Success,NotFound,Forbidden,Invalid}
public interface IDoubtService
{
 Task<IReadOnlyList<DoubtCourseDto>>GetStudentCoursesAsync(string studentId,CancellationToken ct=default); Task<IReadOnlyList<DoubtDto>>GetStudentDoubtsAsync(string studentId,CancellationToken ct=default); Task<DoubtActionResult>AskAsync(string studentId,int courseId,string question,CancellationToken ct=default);
 Task<IReadOnlyList<DoubtDto>>GetFacultyDoubtsAsync(string facultyId,CancellationToken ct=default); Task<IReadOnlyList<DoubtDto>>GetAllAsync(CancellationToken ct=default); Task<DoubtActionResult>AnswerFacultyAsync(string facultyId,int doubtId,string answer,CancellationToken ct=default); Task<DoubtActionResult>AnswerAdminAsync(string adminId,int doubtId,string answer,CancellationToken ct=default);
}
