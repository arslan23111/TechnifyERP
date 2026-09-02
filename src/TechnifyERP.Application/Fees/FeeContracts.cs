using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Application.Fees;

public sealed record FeeRecordDto(
    int Id,
    string StudentUserId,
    string StudentName,
    string? RegistrationNumber,
    string CourseCode,
    string CourseName,
    int Month,
    int Year,
    decimal AmountDue,
    decimal AmountPaid,
    DateOnly DueDate,
    FeeStatus Status,
    decimal? SubmittedAmount,
    string? PaymentReceiptPath,
    string? AdminRemarks);

public sealed record InstallmentRequestDto(
    int Id,
    int FeeRecordId,
    string StudentName,
    string? RegistrationNumber,
    string CourseCode,
    int Month,
    int Year,
    decimal FeeAmount,
    decimal RequestedAmount,
    string Reason,
    InstallmentRequestStatus Status,
    string? AdminRemarks,
    DateTime RequestedAtUtc);

public sealed record FeeReportDto(
    int Month,
    int Year,
    decimal TotalDue,
    decimal TotalPaid,
    decimal TotalPending,
    IReadOnlyList<FeeRecordDto> Records);

public enum MarkFeePaidResult { Success, NotFound, InvalidAmount }
public enum FeeActionResult { Success, NotFound, Invalid, AlreadyProcessed }

public interface IFeeService
{
    Task<FeeReportDto> GetAdminReportAsync(int month, int year, CancellationToken cancellationToken = default);
    Task<FeeReportDto> GetStudentReportAsync(string studentUserId, CancellationToken cancellationToken = default);
    Task<MarkFeePaidResult> MarkPaidAsync(int feeRecordId, decimal amount, CancellationToken cancellationToken = default);
    Task<FeeActionResult> SubmitReceiptAsync(string studentUserId, int feeRecordId, decimal amount, string receiptPath, CancellationToken cancellationToken = default);
    Task<FeeActionResult> ReviewReceiptAsync(int feeRecordId, bool approve, string? remarks, CancellationToken cancellationToken = default);
    Task<FeeActionResult> RequestInstallmentAsync(string studentUserId, int feeRecordId, decimal amount, string reason, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InstallmentRequestDto>> GetInstallmentRequestsAsync(string? studentUserId = null, CancellationToken cancellationToken = default);
    Task<FeeActionResult> ReviewInstallmentAsync(int requestId, bool approve, string? remarks, CancellationToken cancellationToken = default);
}
