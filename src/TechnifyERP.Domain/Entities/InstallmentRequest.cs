using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Domain.Entities;

public sealed class InstallmentRequest
{
    public int Id { get; set; }
    public int FeeRecordId { get; set; }
    public string StudentUserId { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public InstallmentRequestStatus Status { get; set; } = InstallmentRequestStatus.Pending;
    public string? AdminRemarks { get; set; }
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAtUtc { get; set; }
    public FeeRecord FeeRecord { get; set; } = null!;
}
