using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Application.Attendance;

public sealed record AttendanceCourseDto(int CourseId, string CourseCode, string CourseName);

public sealed record AttendanceStudentDto(
    string StudentUserId,
    string? RegistrationNumber,
    string FullName,
    AttendanceStatus? Status);

public sealed record AttendanceSheetDto(
    int CourseId,
    string CourseCode,
    string CourseName,
    DateOnly AttendanceDate,
    IReadOnlyList<AttendanceStudentDto> Students);

public sealed record SaveAttendanceItem(string StudentUserId, AttendanceStatus Status);

public sealed record AttendanceSummaryDto(
    int Present,
    int Absent,
    int Leave,
    decimal Percentage,
    int ClassesNeededFor75Percent);

public sealed record AttendanceHistoryDto(
    DateOnly Date,
    AttendanceStatus Status,
    string MarkedBy);

public sealed record StudentCourseAttendanceDto(
    int CourseId,
    string CourseCode,
    string CourseName,
    AttendanceSummaryDto Summary,
    IReadOnlyList<AttendanceHistoryDto> History);

public sealed record StudentAttendanceReportDto(
    string StudentName,
    string? RegistrationNumber,
    IReadOnlyList<StudentCourseAttendanceDto> Courses);

public sealed record AdminAttendanceRowDto(
    int CourseId,
    string CourseCode,
    string CourseName,
    string StudentUserId,
    string StudentName,
    string? RegistrationNumber,
    AttendanceSummaryDto Summary);

public sealed record AdminAttendanceReportDto(
    int? SelectedCourseId,
    IReadOnlyList<AttendanceCourseDto> Courses,
    IReadOnlyList<AdminAttendanceRowDto> Rows);

public enum SaveAttendanceResult
{
    Success,
    CourseNotAssigned,
    InvalidStudent,
    NoStudents
}

public interface IAttendanceService
{
    Task<IReadOnlyList<AttendanceCourseDto>> GetAssignedCoursesAsync(string facultyUserId, CancellationToken cancellationToken = default);
    Task<AttendanceSheetDto?> GetSheetAsync(string facultyUserId, int courseId, DateOnly date, CancellationToken cancellationToken = default);
    Task<SaveAttendanceResult> SaveAsync(
        string facultyUserId,
        int courseId,
        DateOnly date,
        IReadOnlyList<SaveAttendanceItem> items,
        CancellationToken cancellationToken = default);
    Task<StudentAttendanceReportDto?> GetStudentReportAsync(string studentUserId, CancellationToken cancellationToken = default);
    Task<AdminAttendanceReportDto> GetAdminReportAsync(int? courseId, CancellationToken cancellationToken = default);
}
