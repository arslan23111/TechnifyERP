namespace TechnifyERP.Application.Assignments;

public sealed record AssignmentCourseDto(int Id, string Code, string Name);
public sealed record AssignmentDto(int Id, int CourseId, string CourseCode, string CourseName, string Title,
    string Description, DateTime DueAtUtc, decimal TotalMarks, string? AttachmentPath, bool AllowResubmission,
    int SubmissionCount, DateTime CreatedAtUtc);
public sealed record SaveAssignmentRequest(int CourseId, string Title, string Description, DateTime DueAtUtc,
    decimal TotalMarks, bool AllowResubmission, string? AttachmentPath);
public sealed record StudentAssignmentDto(int Id, string CourseCode, string CourseName, string Title, string Description,
    DateTime DueAtUtc, decimal TotalMarks, string? AttachmentPath, bool AllowResubmission, int? SubmissionId,
    string? SubmissionPath, DateTime? SubmittedAtUtc, int SubmissionNumber, decimal? Marks, string? Feedback);
public sealed record AssignmentSubmissionDto(int Id, string StudentName, string? RegistrationNumber, string FilePath,
    DateTime SubmittedAtUtc, int SubmissionNumber, decimal? Marks, string? Feedback);
public enum AssignmentActionResult { Success, NotFound, Forbidden, Invalid, Closed }

public interface IAssignmentService
{
    Task<IReadOnlyList<AssignmentCourseDto>> GetFacultyCoursesAsync(string facultyUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssignmentDto>> GetFacultyAssignmentsAsync(string facultyUserId, CancellationToken cancellationToken = default);
    Task<AssignmentDto?> GetFacultyAssignmentAsync(string facultyUserId, int id, CancellationToken cancellationToken = default);
    Task<(AssignmentActionResult Result, int Id)> CreateAsync(string facultyUserId, SaveAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<AssignmentActionResult> UpdateAsync(string facultyUserId, int id, SaveAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<AssignmentActionResult> DeleteAsync(string facultyUserId, int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentAssignmentDto>> GetStudentAssignmentsAsync(string studentUserId, CancellationToken cancellationToken = default);
    Task<AssignmentActionResult> SubmitAsync(string studentUserId, int assignmentId, string filePath, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssignmentSubmissionDto>?> GetSubmissionsAsync(string facultyUserId, int assignmentId, CancellationToken cancellationToken = default);
    Task<AssignmentActionResult> GradeAsync(string facultyUserId, int submissionId, decimal marks, string? feedback, CancellationToken cancellationToken = default);
}
