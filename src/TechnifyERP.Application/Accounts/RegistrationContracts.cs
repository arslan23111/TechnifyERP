namespace TechnifyERP.Application.Accounts;

public sealed record RegisterStudentRequest(
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? Gender,
    string? Cnic,
    DateOnly? DateOfBirth,
    int CourseId,
    string Password);

public sealed record RegisterFacultyRequest(
    string FullName,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? Gender,
    string? Cnic,
    DateOnly? DateOfBirth,
    string Designation,
    string Department,
    DateOnly? JoiningDate,
    string Password);

public sealed record RegistrationResult(bool Succeeded, IReadOnlyList<string> Errors)
{
    public static RegistrationResult Success() => new(true, []);
    public static RegistrationResult Failure(params string[] errors) => new(false, errors);
}

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterStudentAsync(RegisterStudentRequest request, CancellationToken cancellationToken = default);
    Task<RegistrationResult> RegisterFacultyAsync(RegisterFacultyRequest request, CancellationToken cancellationToken = default);
}
