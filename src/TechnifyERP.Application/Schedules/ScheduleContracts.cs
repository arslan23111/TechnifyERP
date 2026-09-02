namespace TechnifyERP.Application.Schedules;
public sealed record FacultyCourseOptionDto(string FacultyUserId,string FacultyName,int CourseId,string CourseCode,string CourseName);
public sealed record ClassScheduleDto(int Id,int CourseId,string CourseCode,string CourseName,string FacultyUserId,string FacultyName,DayOfWeek DayOfWeek,TimeOnly StartTime,TimeOnly EndTime,string Room);
public sealed record SaveScheduleRequest(int CourseId,string FacultyUserId,DayOfWeek DayOfWeek,TimeOnly StartTime,TimeOnly EndTime,string Room);
public enum ScheduleActionResult{Success,NotFound,Invalid,FacultyNotAssigned,Conflict}
public interface IScheduleService
{
 Task<IReadOnlyList<FacultyCourseOptionDto>> GetOptionsAsync(CancellationToken ct=default);
 Task<IReadOnlyList<ClassScheduleDto>> GetAllAsync(CancellationToken ct=default);
 Task<ScheduleActionResult> CreateAsync(SaveScheduleRequest request,CancellationToken ct=default);
 Task<ScheduleActionResult> DeleteAsync(int id,CancellationToken ct=default);
 Task<IReadOnlyList<ClassScheduleDto>> GetFacultyScheduleAsync(string facultyId,CancellationToken ct=default);
 Task<IReadOnlyList<ClassScheduleDto>> GetStudentScheduleAsync(string studentId,CancellationToken ct=default);
}
