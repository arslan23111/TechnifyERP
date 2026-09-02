using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.Assignments;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Domain.Enums;
using TechnifyERP.Infrastructure.Persistence;

namespace TechnifyERP.Infrastructure.Assignments;

internal sealed class AssignmentService(ApplicationDbContext context) : IAssignmentService
{
    public async Task<IReadOnlyList<AssignmentCourseDto>> GetFacultyCoursesAsync(string facultyUserId, CancellationToken ct = default) =>
        await context.FacultyCourses.AsNoTracking().Where(x => x.FacultyUserId == facultyUserId)
            .OrderBy(x => x.Course.CourseCode).Select(x => new AssignmentCourseDto(x.CourseId, x.Course.CourseCode, x.Course.Name)).ToListAsync(ct);

    public async Task<IReadOnlyList<AssignmentDto>> GetFacultyAssignmentsAsync(string facultyUserId, CancellationToken ct = default) =>
        await ProjectFacultyAssignments(context.CourseAssignments.AsNoTracking()
            .Where(x => x.FacultyUserId == facultyUserId)
            .OrderByDescending(x => x.CreatedAtUtc))
            .ToListAsync(ct);

    public async Task<AssignmentDto?> GetFacultyAssignmentAsync(string facultyUserId, int id, CancellationToken ct = default) =>
        await ProjectFacultyAssignments(context.CourseAssignments.AsNoTracking()
            .Where(x => x.FacultyUserId == facultyUserId && x.Id == id))
            .FirstOrDefaultAsync(ct);

    public async Task<(AssignmentActionResult Result, int Id)> CreateAsync(string facultyUserId, SaveAssignmentRequest request, CancellationToken ct = default)
    {
        if (!Valid(request)) return (AssignmentActionResult.Invalid, 0);
        if (!await AssignedAsync(facultyUserId, request.CourseId, ct)) return (AssignmentActionResult.Forbidden, 0);
        var assignment = new CourseAssignment { FacultyUserId = facultyUserId };
        Apply(assignment, request);
        context.CourseAssignments.Add(assignment);
        await context.SaveChangesAsync(ct);
        return (AssignmentActionResult.Success, assignment.Id);
    }

    public async Task<AssignmentActionResult> UpdateAsync(string facultyUserId, int id, SaveAssignmentRequest request, CancellationToken ct = default)
    {
        if (!Valid(request)) return AssignmentActionResult.Invalid;
        var assignment = await context.CourseAssignments.FirstOrDefaultAsync(x => x.Id == id && x.FacultyUserId == facultyUserId, ct);
        if (assignment is null) return AssignmentActionResult.NotFound;
        if (!await AssignedAsync(facultyUserId, request.CourseId, ct)) return AssignmentActionResult.Forbidden;
        Apply(assignment, request);
        await context.SaveChangesAsync(ct);
        return AssignmentActionResult.Success;
    }

    public async Task<AssignmentActionResult> DeleteAsync(string facultyUserId, int id, CancellationToken ct = default)
    {
        var assignment = await context.CourseAssignments.FirstOrDefaultAsync(x => x.Id == id && x.FacultyUserId == facultyUserId, ct);
        if (assignment is null) return AssignmentActionResult.NotFound;
        context.CourseAssignments.Remove(assignment);
        await context.SaveChangesAsync(ct);
        return AssignmentActionResult.Success;
    }

    public async Task<IReadOnlyList<StudentAssignmentDto>> GetStudentAssignmentsAsync(string studentUserId, CancellationToken ct = default) =>
        await (from assignment in context.CourseAssignments.AsNoTracking()
               join enrollment in context.CourseEnrollments.AsNoTracking() on assignment.CourseId equals enrollment.CourseId
               where enrollment.StudentUserId == studentUserId && enrollment.Status == EnrollmentStatus.Approved
               let submission = assignment.Submissions.FirstOrDefault(x => x.StudentUserId == studentUserId)
               orderby assignment.DueAtUtc
               select new StudentAssignmentDto(assignment.Id, assignment.Course.CourseCode, assignment.Course.Name,
                   assignment.Title, assignment.Description, assignment.DueAtUtc, assignment.TotalMarks,
                   assignment.AttachmentPath, assignment.AllowResubmission, submission == null ? null : submission.Id,
                   submission == null ? null : submission.FilePath, submission == null ? null : submission.SubmittedAtUtc,
                   submission == null ? 0 : submission.SubmissionNumber, submission == null ? null : submission.Marks,
                   submission == null ? null : submission.Feedback)).ToListAsync(ct);

    public async Task<AssignmentActionResult> SubmitAsync(string studentUserId, int assignmentId, string filePath, CancellationToken ct = default)
    {
        var assignment = await context.CourseAssignments.Include(x => x.Submissions)
            .FirstOrDefaultAsync(x => x.Id == assignmentId, ct);
        if (assignment is null) return AssignmentActionResult.NotFound;
        if (!await context.CourseEnrollments.AnyAsync(x => x.StudentUserId == studentUserId && x.CourseId == assignment.CourseId && x.Status == EnrollmentStatus.Approved, ct))
            return AssignmentActionResult.Forbidden;
        if (DateTime.UtcNow > assignment.DueAtUtc) return AssignmentActionResult.Closed;
        var submission = assignment.Submissions.FirstOrDefault(x => x.StudentUserId == studentUserId);
        if (submission is null)
            context.AssignmentSubmissions.Add(new AssignmentSubmission { AssignmentId = assignmentId, StudentUserId = studentUserId, FilePath = filePath });
        else
        {
            if (!assignment.AllowResubmission) return AssignmentActionResult.Closed;
            submission.FilePath = filePath; submission.SubmittedAtUtc = DateTime.UtcNow; submission.SubmissionNumber++;
            submission.Marks = null; submission.Feedback = null; submission.GradedAtUtc = null;
        }
        await context.SaveChangesAsync(ct);
        return AssignmentActionResult.Success;
    }

    public async Task<IReadOnlyList<AssignmentSubmissionDto>?> GetSubmissionsAsync(string facultyUserId, int assignmentId, CancellationToken ct = default)
    {
        if (!await context.CourseAssignments.AnyAsync(x => x.Id == assignmentId && x.FacultyUserId == facultyUserId, ct)) return null;
        return await (from submission in context.AssignmentSubmissions.AsNoTracking()
                      join user in context.Users.AsNoTracking() on submission.StudentUserId equals user.Id
                      join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
                      where submission.AssignmentId == assignmentId orderby user.FullName
                      select new AssignmentSubmissionDto(submission.Id, user.FullName, profile.RegistrationNumber,
                          submission.FilePath, submission.SubmittedAtUtc, submission.SubmissionNumber, submission.Marks, submission.Feedback)).ToListAsync(ct);
    }

    public async Task<AssignmentActionResult> GradeAsync(string facultyUserId, int submissionId, decimal marks, string? feedback, CancellationToken ct = default)
    {
        var submission = await context.AssignmentSubmissions.Include(x => x.Assignment)
            .FirstOrDefaultAsync(x => x.Id == submissionId && x.Assignment.FacultyUserId == facultyUserId, ct);
        if (submission is null) return AssignmentActionResult.NotFound;
        if (marks < 0 || marks > submission.Assignment.TotalMarks) return AssignmentActionResult.Invalid;
        submission.Marks = marks; submission.Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim(); submission.GradedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
        return AssignmentActionResult.Success;
    }

    private static IQueryable<AssignmentDto> ProjectFacultyAssignments(IQueryable<CourseAssignment> query) => query
        .Select(x => new AssignmentDto(x.Id, x.CourseId, x.Course.CourseCode, x.Course.Name, x.Title, x.Description,
            x.DueAtUtc, x.TotalMarks, x.AttachmentPath, x.AllowResubmission, x.Submissions.Count, x.CreatedAtUtc));
    private Task<bool> AssignedAsync(string userId, int courseId, CancellationToken ct) => context.FacultyCourses.AnyAsync(x => x.FacultyUserId == userId && x.CourseId == courseId, ct);
    private static bool Valid(SaveAssignmentRequest x) => x.CourseId > 0 && !string.IsNullOrWhiteSpace(x.Title) && !string.IsNullOrWhiteSpace(x.Description) && x.TotalMarks > 0;
    private static void Apply(CourseAssignment x, SaveAssignmentRequest r) { x.CourseId = r.CourseId; x.Title = r.Title.Trim(); x.Description = r.Description.Trim(); x.DueAtUtc = r.DueAtUtc; x.TotalMarks = r.TotalMarks; x.AllowResubmission = r.AllowResubmission; if (r.AttachmentPath is not null) x.AttachmentPath = r.AttachmentPath; }
}
