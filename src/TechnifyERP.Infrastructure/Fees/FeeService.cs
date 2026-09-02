using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.Fees;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Domain.Enums;
using TechnifyERP.Infrastructure.Persistence;

namespace TechnifyERP.Infrastructure.Fees;

internal sealed class FeeService(ApplicationDbContext context) : IFeeService
{
    public async Task<FeeReportDto> GetAdminReportAsync(int month, int year, CancellationToken cancellationToken = default)
    {
        await EnsureMonthlyRecordsAsync(month, year, cancellationToken);
        await UpdateOverdueAsync(cancellationToken);
        var records = await QueryRecords(month, year, null).ToListAsync(cancellationToken);
        return BuildReport(month, year, records);
    }

    public async Task<FeeReportDto> GetStudentReportAsync(string studentUserId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        await EnsureMonthlyRecordsAsync(today.Month, today.Year, cancellationToken);
        await UpdateOverdueAsync(cancellationToken);
        var records = await QueryRecords(null, null, studentUserId).ToListAsync(cancellationToken);
        return BuildReport(today.Month, today.Year, records);
    }

    public async Task<MarkFeePaidResult> MarkPaidAsync(int feeRecordId, decimal amount, CancellationToken cancellationToken = default)
    {
        var record = await context.FeeRecords.FirstOrDefaultAsync(item => item.Id == feeRecordId, cancellationToken);
        if (record is null) return MarkFeePaidResult.NotFound;
        if (amount <= 0 || amount > record.AmountDue) return MarkFeePaidResult.InvalidAmount;
        record.AmountPaid = amount;
        record.Status = amount >= record.AmountDue ? FeeStatus.Paid : FeeStatus.Pending;
        record.PaidAtUtc = record.Status == FeeStatus.Paid ? DateTime.UtcNow : null;
        await context.SaveChangesAsync(cancellationToken);
        return MarkFeePaidResult.Success;
    }

    public async Task<FeeActionResult> SubmitReceiptAsync(
        string studentUserId, int feeRecordId, decimal amount, string receiptPath,
        CancellationToken cancellationToken = default)
    {
        var record = await context.FeeRecords.FirstOrDefaultAsync(
            item => item.Id == feeRecordId && item.StudentUserId == studentUserId, cancellationToken);
        if (record is null) return FeeActionResult.NotFound;
        if (record.Status is FeeStatus.Paid or FeeStatus.Submitted) return FeeActionResult.AlreadyProcessed;
        if (amount <= 0 || amount > record.AmountDue || string.IsNullOrWhiteSpace(receiptPath)) return FeeActionResult.Invalid;

        record.SubmittedAmount = amount;
        record.PaymentReceiptPath = receiptPath;
        record.SubmittedAtUtc = DateTime.UtcNow;
        record.AdminRemarks = null;
        record.Status = FeeStatus.Submitted;
        await context.SaveChangesAsync(cancellationToken);
        return FeeActionResult.Success;
    }

    public async Task<FeeActionResult> ReviewReceiptAsync(
        int feeRecordId, bool approve, string? remarks, CancellationToken cancellationToken = default)
    {
        var record = await context.FeeRecords.FirstOrDefaultAsync(item => item.Id == feeRecordId, cancellationToken);
        if (record is null) return FeeActionResult.NotFound;
        if (record.Status != FeeStatus.Submitted || record.SubmittedAmount is null) return FeeActionResult.AlreadyProcessed;

        record.AdminRemarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();
        if (approve)
        {
            record.AmountPaid = record.SubmittedAmount.Value;
            record.Status = record.AmountPaid >= record.AmountDue ? FeeStatus.Paid : FeeStatus.Pending;
            record.PaidAtUtc = DateTime.UtcNow;
        }
        else
        {
            record.Status = record.DueDate < DateOnly.FromDateTime(DateTime.Today) ? FeeStatus.Overdue : FeeStatus.Pending;
        }

        await context.SaveChangesAsync(cancellationToken);
        return FeeActionResult.Success;
    }

    public async Task<FeeActionResult> RequestInstallmentAsync(
        string studentUserId, int feeRecordId, decimal amount, string reason,
        CancellationToken cancellationToken = default)
    {
        var fee = await context.FeeRecords.AsNoTracking().FirstOrDefaultAsync(
            item => item.Id == feeRecordId && item.StudentUserId == studentUserId, cancellationToken);
        if (fee is null) return FeeActionResult.NotFound;
        if (fee.Status == FeeStatus.Paid || amount <= 0 || amount >= fee.AmountDue || string.IsNullOrWhiteSpace(reason)) return FeeActionResult.Invalid;
        var pending = await context.InstallmentRequests.AnyAsync(
            item => item.FeeRecordId == feeRecordId && item.StudentUserId == studentUserId && item.Status == InstallmentRequestStatus.Pending,
            cancellationToken);
        if (pending) return FeeActionResult.AlreadyProcessed;

        context.InstallmentRequests.Add(new InstallmentRequest
        {
            FeeRecordId = feeRecordId,
            StudentUserId = studentUserId,
            RequestedAmount = amount,
            Reason = reason.Trim()
        });
        await context.SaveChangesAsync(cancellationToken);
        return FeeActionResult.Success;
    }

    public async Task<IReadOnlyList<InstallmentRequestDto>> GetInstallmentRequestsAsync(
        string? studentUserId = null, CancellationToken cancellationToken = default)
    {
        var requests = context.InstallmentRequests.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(studentUserId))
            requests = requests.Where(item => item.StudentUserId == studentUserId);

        return await (
            from request in requests
            join user in context.Users.AsNoTracking() on request.StudentUserId equals user.Id
            join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
            orderby request.RequestedAtUtc descending
            select new InstallmentRequestDto(
                request.Id, request.FeeRecordId, user.FullName, profile.RegistrationNumber,
                request.FeeRecord.Course.CourseCode, request.FeeRecord.Month, request.FeeRecord.Year,
                request.FeeRecord.AmountDue, request.RequestedAmount, request.Reason, request.Status,
                request.AdminRemarks, request.RequestedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<FeeActionResult> ReviewInstallmentAsync(
        int requestId, bool approve, string? remarks, CancellationToken cancellationToken = default)
    {
        var request = await context.InstallmentRequests.FirstOrDefaultAsync(item => item.Id == requestId, cancellationToken);
        if (request is null) return FeeActionResult.NotFound;
        if (request.Status != InstallmentRequestStatus.Pending) return FeeActionResult.AlreadyProcessed;
        request.Status = approve ? InstallmentRequestStatus.Approved : InstallmentRequestStatus.Rejected;
        request.AdminRemarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();
        request.ReviewedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return FeeActionResult.Success;
    }

    private async Task EnsureMonthlyRecordsAsync(int month, int year, CancellationToken cancellationToken)
    {
        var enrollments = await context.CourseEnrollments.Include(item => item.Course)
            .Where(item => item.Status == EnrollmentStatus.Approved).ToListAsync(cancellationToken);
        var existingIds = await context.FeeRecords.Where(item => item.Month == month && item.Year == year)
            .Select(item => item.EnrollmentId).ToListAsync(cancellationToken);
        foreach (var enrollment in enrollments.Where(item => !existingIds.Contains(item.Id)))
        {
            context.FeeRecords.Add(CreateRecord(enrollment, month, year));
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    internal static FeeRecord CreateRecord(CourseEnrollment enrollment, int month, int year) => new()
    {
        EnrollmentId = enrollment.Id,
        StudentUserId = enrollment.StudentUserId,
        CourseId = enrollment.CourseId,
        Month = month,
        Year = year,
        AmountDue = enrollment.Course.MonthlyFee,
        DueDate = new DateOnly(year, month, 10)
    };

    private async Task UpdateOverdueAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var overdue = await context.FeeRecords.Where(item => item.Status == FeeStatus.Pending && item.DueDate < today).ToListAsync(cancellationToken);
        foreach (var record in overdue) record.Status = FeeStatus.Overdue;
        if (overdue.Count > 0) await context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<FeeRecordDto> QueryRecords(int? month, int? year, string? studentUserId)
    {
        var fees = context.FeeRecords.AsNoTracking().AsQueryable();
        if (month.HasValue) fees = fees.Where(fee => fee.Month == month.Value);
        if (year.HasValue) fees = fees.Where(fee => fee.Year == year.Value);
        if (!string.IsNullOrEmpty(studentUserId)) fees = fees.Where(fee => fee.StudentUserId == studentUserId);

        return
        from fee in fees
        join user in context.Users.AsNoTracking() on fee.StudentUserId equals user.Id
        join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
        orderby fee.Year descending, fee.Month descending, fee.Course.CourseCode, user.FullName
        select new FeeRecordDto(fee.Id, user.Id, user.FullName, profile.RegistrationNumber, fee.Course.CourseCode,
            fee.Course.Name, fee.Month, fee.Year, fee.AmountDue, fee.AmountPaid, fee.DueDate, fee.Status,
            fee.SubmittedAmount, fee.PaymentReceiptPath, fee.AdminRemarks);
    }

    private static FeeReportDto BuildReport(int month, int year, IReadOnlyList<FeeRecordDto> records) =>
        new(month, year, records.Sum(item => item.AmountDue), records.Sum(item => item.AmountPaid),
            records.Sum(item => item.AmountDue - item.AmountPaid), records);
}
