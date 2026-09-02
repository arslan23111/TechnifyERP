using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Domain.Entities;

public sealed class CourseEnrollment
{
    public int Id { get; set; }
    public string StudentUserId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAtUtc { get; set; }
    public Course Course { get; set; } = null!;
    public ICollection<FeeRecord> FeeRecords { get; set; } = [];
}
