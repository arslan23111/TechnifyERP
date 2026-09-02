using TechnifyERP.Domain.Enums;

namespace TechnifyERP.Application.Accounts;

public sealed record PendingAccountDto(
    string UserId,
    string FullName,
    string Email,
    string? PhoneNumber,
    UserType UserType,
    DateTime CreatedAtUtc,
    string? RequestedCourse);

public enum AccountReviewResult
{
    Success,
    NotFound,
    AlreadyReviewed
}

public interface IAccountApprovalService
{
    Task<IReadOnlyList<PendingAccountDto>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<AccountReviewResult> ApproveAsync(string userId, CancellationToken cancellationToken = default);
    Task<AccountReviewResult> RejectAsync(string userId, CancellationToken cancellationToken = default);
}
