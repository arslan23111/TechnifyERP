using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.Accounts;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Domain.Enums;
using TechnifyERP.Infrastructure.Identity;
using TechnifyERP.Infrastructure.Persistence;
using TechnifyERP.Infrastructure.Fees;

namespace TechnifyERP.Infrastructure.Accounts;

internal sealed class AccountService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : IRegistrationService, IAccountApprovalService
{
    public async Task<RegistrationResult> RegisterStudentAsync(
        RegisterStudentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await context.Courses.AnyAsync(course => course.Id == request.CourseId, cancellationToken))
        {
            return RegistrationResult.Failure("The selected course is not available.");
        }

        var duplicateError = await GetDuplicateErrorAsync(request.Email, request.Cnic, cancellationToken);
        if (duplicateError is not null)
        {
            return RegistrationResult.Failure(duplicateError);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var user = CreateUser(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Address,
            request.Gender,
            request.Cnic,
            request.DateOfBirth,
            UserType.Student);

        var identityResult = await userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            return new RegistrationResult(false, identityResult.Errors.Select(error => error.Description).ToArray());
        }

        context.StudentProfiles.Add(new StudentProfile { UserId = user.Id });
        context.CourseEnrollments.Add(new CourseEnrollment
        {
            StudentUserId = user.Id,
            CourseId = request.CourseId,
            Status = EnrollmentStatus.Pending
        });
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return RegistrationResult.Success();
    }

    public async Task<RegistrationResult> RegisterFacultyAsync(
        RegisterFacultyRequest request,
        CancellationToken cancellationToken = default)
    {
        var duplicateError = await GetDuplicateErrorAsync(request.Email, request.Cnic, cancellationToken);
        if (duplicateError is not null)
        {
            return RegistrationResult.Failure(duplicateError);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var user = CreateUser(
            request.FullName,
            request.Email,
            request.PhoneNumber,
            request.Address,
            request.Gender,
            request.Cnic,
            request.DateOfBirth,
            UserType.Faculty);

        var identityResult = await userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            return new RegistrationResult(false, identityResult.Errors.Select(error => error.Description).ToArray());
        }

        context.FacultyProfiles.Add(new FacultyProfile
        {
            UserId = user.Id,
            Designation = request.Designation.Trim(),
            Department = request.Department.Trim(),
            JoiningDate = request.JoiningDate
        });
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return RegistrationResult.Success();
    }

    public async Task<IReadOnlyList<PendingAccountDto>> GetPendingAsync(CancellationToken cancellationToken = default) =>
        await context.Users
            .AsNoTracking()
            .Where(user => user.AccountStatus == AccountStatus.Pending)
            .OrderBy(user => user.CreatedAtUtc)
            .Select(user => new PendingAccountDto(
                user.Id,
                user.FullName,
                user.Email ?? string.Empty,
                user.PhoneNumber,
                user.UserType,
                user.CreatedAtUtc,
                user.CourseEnrollments
                    .Where(enrollment => enrollment.Status == EnrollmentStatus.Pending)
                    .Select(enrollment => enrollment.Course.Name)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

    public async Task<AccountReviewResult> ApproveAsync(string userId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var user = await context.Users
            .Include(item => item.StudentProfile)
            .Include(item => item.FacultyProfile)
            .Include(item => item.CourseEnrollments)
                .ThenInclude(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        if (user is null)
        {
            return AccountReviewResult.NotFound;
        }

        if (user.AccountStatus != AccountStatus.Pending)
        {
            return AccountReviewResult.AlreadyReviewed;
        }

        user.AccountStatus = AccountStatus.Active;
        user.EmailConfirmed = true;
        var roleName = user.UserType.ToString();
        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(error => error.Description)));
        }

        if (user.UserType == UserType.Student && user.StudentProfile is not null)
        {
            user.StudentProfile.RegistrationNumber = await NextStudentNumberAsync(cancellationToken);
            foreach (var enrollment in user.CourseEnrollments.Where(item => item.Status == EnrollmentStatus.Pending))
            {
                enrollment.Status = EnrollmentStatus.Approved;
                enrollment.ReviewedAtUtc = DateTime.UtcNow;
                context.FeeRecords.Add(FeeService.CreateRecord(enrollment, DateTime.Today.Month, DateTime.Today.Year));
            }
        }
        else if (user.UserType == UserType.Faculty && user.FacultyProfile is not null)
        {
            user.FacultyProfile.EmployeeCode = await NextFacultyCodeAsync(cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return AccountReviewResult.Success;
    }

    public async Task<AccountReviewResult> RejectAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Users
            .Include(item => item.CourseEnrollments)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        if (user is null)
        {
            return AccountReviewResult.NotFound;
        }

        if (user.AccountStatus != AccountStatus.Pending)
        {
            return AccountReviewResult.AlreadyReviewed;
        }

        user.AccountStatus = AccountStatus.Rejected;
        foreach (var enrollment in user.CourseEnrollments.Where(item => item.Status == EnrollmentStatus.Pending))
        {
            enrollment.Status = EnrollmentStatus.Rejected;
            enrollment.ReviewedAtUtc = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);
        return AccountReviewResult.Success;
    }

    private async Task<string?> GetDuplicateErrorAsync(
        string email,
        string? cnic,
        CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(email.Trim()) is not null)
        {
            return "An account with this email already exists.";
        }

        if (!string.IsNullOrWhiteSpace(cnic) &&
            await context.Users.AnyAsync(user => user.Cnic == cnic.Trim(), cancellationToken))
        {
            return "An account with this CNIC already exists.";
        }

        return null;
    }

    private static ApplicationUser CreateUser(
        string fullName,
        string email,
        string? phoneNumber,
        string? address,
        string? gender,
        string? cnic,
        DateOnly? dateOfBirth,
        UserType userType) => new()
    {
        UserName = email.Trim(),
        Email = email.Trim(),
        PhoneNumber = NullIfWhiteSpace(phoneNumber),
        FullName = fullName.Trim(),
        Address = NullIfWhiteSpace(address),
        Gender = NullIfWhiteSpace(gender),
        Cnic = NullIfWhiteSpace(cnic),
        DateOfBirth = dateOfBirth,
        UserType = userType,
        AccountStatus = AccountStatus.Pending,
        EmailConfirmed = false
    };

    private async Task<string> NextStudentNumberAsync(CancellationToken cancellationToken)
    {
        var sequence = await context.StudentProfiles.CountAsync(
            profile => profile.RegistrationNumber != null,
            cancellationToken) + 1;
        return $"TECH-{DateTime.UtcNow.Year}-{sequence:0000}";
    }

    private async Task<string> NextFacultyCodeAsync(CancellationToken cancellationToken)
    {
        var sequence = await context.FacultyProfiles.CountAsync(
            profile => profile.EmployeeCode != null,
            cancellationToken) + 1;
        return $"FAC-{DateTime.UtcNow.Year}-{sequence:000}";
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
