namespace TechnifyERP.Application.Faculty;

public sealed record FacultyDashboardDto(int AssignedCourses, int EnrolledStudents, int ActiveAssignments, int PendingGrading, int UpcomingTests, int OpenDoubts);

public sealed record FacultyCourseDto(
    int CourseId,
    string CourseCode,
    string CourseName,
    int CreditHours,
    decimal MonthlyFee,
    bool IsAssigned,
    int EnrolledStudents);

public sealed record FacultyStudentDto(
    string UserId,
    string? RegistrationNumber,
    string FullName,
    string Email,
    string? PhoneNumber);

public enum FacultyCourseActionResult
{
    Success,
    CourseNotFound,
    AlreadyAssigned,
    NotAssigned
}

public interface IFacultyCourseService
{
    Task<FacultyDashboardDto> GetDashboardAsync(string facultyUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FacultyCourseDto>> GetCoursesAsync(string facultyUserId, CancellationToken cancellationToken = default);
    Task<FacultyCourseActionResult> AssignAsync(string facultyUserId, int courseId, CancellationToken cancellationToken = default);
    Task<FacultyCourseActionResult> UnassignAsync(string facultyUserId, int courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FacultyStudentDto>> GetEnrolledStudentsAsync(string facultyUserId, int courseId, CancellationToken cancellationToken = default);
}
