using Microsoft.AspNetCore.Identity;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public AccountStatus AccountStatus { get; set; } = AccountStatus.Pending;
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string? Cnic { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? ProfilePicturePath { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public StudentProfile? StudentProfile { get; set; }
    public FacultyProfile? FacultyProfile { get; set; }
    public ICollection<CourseEnrollment> CourseEnrollments { get; set; } = [];
    public ICollection<FacultyCourse> FacultyCourses { get; set; } = [];
    public ICollection<AttendanceRecord> StudentAttendanceRecords { get; set; } = [];
    public ICollection<AttendanceRecord> MarkedAttendanceRecords { get; set; } = [];
}
