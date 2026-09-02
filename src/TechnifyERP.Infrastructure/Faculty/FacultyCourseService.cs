using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.Faculty;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Domain.Enums;
using TechnifyERP.Infrastructure.Persistence;

namespace TechnifyERP.Infrastructure.Faculty;

internal sealed class FacultyCourseService(ApplicationDbContext context) : IFacultyCourseService
{
    public async Task<FacultyDashboardDto> GetDashboardAsync(
        string facultyUserId,
        CancellationToken cancellationToken = default)
    {
        var courseIds = context.FacultyCourses
            .Where(assignment => assignment.FacultyUserId == facultyUserId)
            .Select(assignment => assignment.CourseId);

        var assignedCourses = await courseIds.CountAsync(cancellationToken);
        var enrolledStudents = await context.CourseEnrollments
            .Where(enrollment =>
                courseIds.Contains(enrollment.CourseId) &&
                enrollment.Status == EnrollmentStatus.Approved)
            .Select(enrollment => enrollment.StudentUserId)
            .Distinct()
            .CountAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var activeAssignments = await context.CourseAssignments
            .CountAsync(x => courseIds.Contains(x.CourseId) && x.DueAtUtc >= now, cancellationToken);
        var pendingGrading = await context.AssignmentSubmissions
            .CountAsync(x => courseIds.Contains(x.Assignment.CourseId) && x.Marks == null, cancellationToken);
        var upcomingTests = await context.CourseTests
            .CountAsync(x => courseIds.Contains(x.CourseId) && x.EndsAtUtc >= now, cancellationToken);
        var openDoubts = await context.StudentDoubts
            .CountAsync(x => courseIds.Contains(x.CourseId) && x.Answer == null, cancellationToken);

        return new FacultyDashboardDto(assignedCourses, enrolledStudents, activeAssignments, pendingGrading, upcomingTests, openDoubts);
    }

    public async Task<IReadOnlyList<FacultyCourseDto>> GetCoursesAsync(
        string facultyUserId,
        CancellationToken cancellationToken = default) =>
        await context.Courses
            .AsNoTracking()
            .OrderBy(course => course.CourseCode)
            .Select(course => new FacultyCourseDto(
                course.Id,
                course.CourseCode,
                course.Name,
                course.CreditHours,
                course.MonthlyFee,
                course.FacultyAssignments.Any(assignment => assignment.FacultyUserId == facultyUserId),
                course.Enrollments.Count(enrollment => enrollment.Status == EnrollmentStatus.Approved)))
            .ToListAsync(cancellationToken);

    public async Task<FacultyCourseActionResult> AssignAsync(
        string facultyUserId,
        int courseId,
        CancellationToken cancellationToken = default)
    {
        if (!await context.Courses.AnyAsync(course => course.Id == courseId, cancellationToken))
        {
            return FacultyCourseActionResult.CourseNotFound;
        }

        if (await context.FacultyCourses.AnyAsync(
                assignment => assignment.FacultyUserId == facultyUserId && assignment.CourseId == courseId,
                cancellationToken))
        {
            return FacultyCourseActionResult.AlreadyAssigned;
        }

        context.FacultyCourses.Add(new FacultyCourse
        {
            FacultyUserId = facultyUserId,
            CourseId = courseId
        });
        await context.SaveChangesAsync(cancellationToken);
        return FacultyCourseActionResult.Success;
    }

    public async Task<FacultyCourseActionResult> UnassignAsync(
        string facultyUserId,
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var assignment = await context.FacultyCourses.FirstOrDefaultAsync(
            item => item.FacultyUserId == facultyUserId && item.CourseId == courseId,
            cancellationToken);

        if (assignment is null)
        {
            return FacultyCourseActionResult.NotAssigned;
        }

        context.FacultyCourses.Remove(assignment);
        await context.SaveChangesAsync(cancellationToken);
        return FacultyCourseActionResult.Success;
    }

    public async Task<IReadOnlyList<FacultyStudentDto>> GetEnrolledStudentsAsync(
        string facultyUserId,
        int courseId,
        CancellationToken cancellationToken = default)
    {
        var isAssigned = await context.FacultyCourses.AnyAsync(
            assignment => assignment.FacultyUserId == facultyUserId && assignment.CourseId == courseId,
            cancellationToken);

        if (!isAssigned)
        {
            return [];
        }

        return await (
            from enrollment in context.CourseEnrollments.AsNoTracking()
            join user in context.Users.AsNoTracking() on enrollment.StudentUserId equals user.Id
            join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
            where enrollment.CourseId == courseId && enrollment.Status == EnrollmentStatus.Approved
            orderby user.FullName
            select new FacultyStudentDto(
                user.Id,
                profile.RegistrationNumber,
                user.FullName,
                user.Email ?? string.Empty,
                user.PhoneNumber))
            .ToListAsync(cancellationToken);
    }
}
