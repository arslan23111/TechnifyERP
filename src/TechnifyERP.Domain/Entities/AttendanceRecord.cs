using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Domain.Entities;

public sealed class AttendanceRecord
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string StudentUserId { get; set; } = string.Empty;
    public string MarkedByFacultyUserId { get; set; } = string.Empty;
    public DateOnly AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Course Course { get; set; } = null!;
}
