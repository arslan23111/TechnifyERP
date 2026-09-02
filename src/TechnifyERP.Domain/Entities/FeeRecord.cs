using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Domain.Entities;

public sealed class FeeRecord
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public string StudentUserId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly DueDate { get; set; }
    public FeeStatus Status { get; set; } = FeeStatus.Pending;
    public DateTime? PaidAtUtc { get; set; }
    public decimal? SubmittedAmount { get; set; }
    public string? PaymentReceiptPath { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public string? AdminRemarks { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public CourseEnrollment Enrollment { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
