using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Application.Admin;

public sealed record AdminDashboardDto(
    int ActiveStudents,
    int ActiveFaculty,
    int Courses,
    int PendingApprovals);

public sealed record StudentListItemDto(
    string UserId,
    string? RegistrationNumber,
    string FullName,
    string Email,
    string? PhoneNumber,
    AccountStatus AccountStatus,
    IReadOnlyList<string> Courses);

public sealed record StudentDetailsDto(
    string UserId,
    string? RegistrationNumber,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? Gender,
    string? Cnic,
    DateOnly? DateOfBirth,
    AccountStatus AccountStatus,
    IReadOnlyList<StudentEnrollmentDto> Enrollments);

public sealed record StudentEnrollmentDto(
    int EnrollmentId,
    string CourseCode,
    string CourseName,
    EnrollmentStatus Status,
    DateTime RequestedAtUtc,
    DateTime? ReviewedAtUtc);

public sealed record FacultyListItemDto(
    string UserId,
    string? EmployeeCode,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Designation,
    string? Department,
    AccountStatus AccountStatus);

public sealed record FacultyDetailsDto(
    string UserId,
    string? EmployeeCode,
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? Gender,
    string? Cnic,
    DateOnly? DateOfBirth,
    string? Designation,
    string? Department,
    DateOnly? JoiningDate,
    AccountStatus AccountStatus);

public sealed record EnrollmentListItemDto(
    int EnrollmentId,
    string? RegistrationNumber,
    string StudentName,
    string CourseCode,
    string CourseName,
    EnrollmentStatus Status,
    DateTime RequestedAtUtc,
    DateTime? ReviewedAtUtc);

public interface IAdminDirectoryService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentListItemDto>> GetStudentsAsync(CancellationToken cancellationToken = default);
    Task<StudentDetailsDto?> GetStudentAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FacultyListItemDto>> GetFacultyAsync(CancellationToken cancellationToken = default);
    Task<FacultyDetailsDto?> GetFacultyMemberAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EnrollmentListItemDto>> GetEnrollmentsAsync(CancellationToken cancellationToken = default);
}
