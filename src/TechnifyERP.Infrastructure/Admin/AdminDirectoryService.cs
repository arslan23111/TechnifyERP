using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.Admin;
using TechnifyERP.Domain.Enums;
using TechnifyERP.Infrastructure.Persistence;

namespace TechnifyERP.Infrastructure.Admin;

internal sealed class AdminDirectoryService(ApplicationDbContext context) : IAdminDirectoryService
{
    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var activeStudents = await context.Users.CountAsync(
            user => user.UserType == UserType.Student && user.AccountStatus == AccountStatus.Active,
            cancellationToken);
        var activeFaculty = await context.Users.CountAsync(
            user => user.UserType == UserType.Faculty && user.AccountStatus == AccountStatus.Active,
            cancellationToken);
        var courses = await context.Courses.CountAsync(cancellationToken);
        var pendingApprovals = await context.Users.CountAsync(
            user => user.AccountStatus == AccountStatus.Pending,
            cancellationToken);

        return new AdminDashboardDto(activeStudents, activeFaculty, courses, pendingApprovals);
    }

    public async Task<IReadOnlyList<StudentListItemDto>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        var students = await context.Users
            .AsNoTracking()
            .Where(user => user.UserType == UserType.Student)
            .Include(user => user.StudentProfile)
            .Include(user => user.CourseEnrollments)
                .ThenInclude(enrollment => enrollment.Course)
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);

        return students.Select(user => new StudentListItemDto(
            user.Id,
            user.StudentProfile?.RegistrationNumber,
            user.FullName,
            user.Email ?? string.Empty,
            user.PhoneNumber,
            user.AccountStatus,
            user.CourseEnrollments
                .Where(enrollment => enrollment.Status == EnrollmentStatus.Approved)
                .Select(enrollment => $"{enrollment.Course.CourseCode} — {enrollment.Course.Name}")
                .ToList()))
            .ToList();
    }

    public async Task<StudentDetailsDto?> GetStudentAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .AsNoTracking()
            .Where(item => item.Id == userId && item.UserType == UserType.Student)
            .Include(item => item.StudentProfile)
            .Include(item => item.CourseEnrollments)
                .ThenInclude(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? null
            : new StudentDetailsDto(
                user.Id,
                user.StudentProfile?.RegistrationNumber,
                user.FullName,
                user.Email ?? string.Empty,
                user.PhoneNumber,
                user.Address,
                user.Gender,
                user.Cnic,
                user.DateOfBirth,
                user.AccountStatus,
                user.CourseEnrollments
                    .OrderByDescending(enrollment => enrollment.RequestedAtUtc)
                    .Select(enrollment => new StudentEnrollmentDto(
                        enrollment.Id,
                        enrollment.Course.CourseCode,
                        enrollment.Course.Name,
                        enrollment.Status,
                        enrollment.RequestedAtUtc,
                        enrollment.ReviewedAtUtc))
                    .ToList());
    }

    public async Task<IReadOnlyList<FacultyListItemDto>> GetFacultyAsync(CancellationToken cancellationToken = default) =>
        await context.Users
            .AsNoTracking()
            .Where(user => user.UserType == UserType.Faculty)
            .OrderBy(user => user.FullName)
            .Select(user => new FacultyListItemDto(
                user.Id,
                user.FacultyProfile == null ? null : user.FacultyProfile.EmployeeCode,
                user.FullName,
                user.Email ?? string.Empty,
                user.PhoneNumber,
                user.FacultyProfile == null ? null : user.FacultyProfile.Designation,
                user.FacultyProfile == null ? null : user.FacultyProfile.Department,
                user.AccountStatus))
            .ToListAsync(cancellationToken);

    public async Task<FacultyDetailsDto?> GetFacultyMemberAsync(
        string userId,
        CancellationToken cancellationToken = default) =>
        await context.Users
            .AsNoTracking()
            .Where(user => user.Id == userId && user.UserType == UserType.Faculty)
            .Select(user => new FacultyDetailsDto(
                user.Id,
                user.FacultyProfile == null ? null : user.FacultyProfile.EmployeeCode,
                user.FullName,
                user.Email ?? string.Empty,
                user.PhoneNumber,
                user.Address,
                user.Gender,
                user.Cnic,
                user.DateOfBirth,
                user.FacultyProfile == null ? null : user.FacultyProfile.Designation,
                user.FacultyProfile == null ? null : user.FacultyProfile.Department,
                user.FacultyProfile == null ? null : user.FacultyProfile.JoiningDate,
                user.AccountStatus))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<EnrollmentListItemDto>> GetEnrollmentsAsync(
        CancellationToken cancellationToken = default) =>
        await (
            from enrollment in context.CourseEnrollments.AsNoTracking()
            join user in context.Users.AsNoTracking() on enrollment.StudentUserId equals user.Id
            join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
            orderby enrollment.RequestedAtUtc descending
            select new EnrollmentListItemDto(
                enrollment.Id,
                profile.RegistrationNumber,
                user.FullName,
                enrollment.Course.CourseCode,
                enrollment.Course.Name,
                enrollment.Status,
                enrollment.RequestedAtUtc,
                enrollment.ReviewedAtUtc))
            .ToListAsync(cancellationToken);
}
