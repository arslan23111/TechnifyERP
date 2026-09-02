namespace TechnifyERP.Application.Profiles;
public sealed record StudentProfileDto(string FullName,string Email,string? RegistrationNumber,string? PhoneNumber,string? Address,string? Gender,DateOnly? DateOfBirth,string? ProfilePicturePath);
public sealed record UpdateStudentProfileRequest(string FullName,string? PhoneNumber,string? Address,string? Gender,DateOnly? DateOfBirth,string? ProfilePicturePath);
public enum ProfileActionResult{Success,NotFound,Invalid}
public interface IProfileService{Task<StudentProfileDto?>GetStudentAsync(string userId,CancellationToken ct=default);Task<ProfileActionResult>UpdateStudentAsync(string userId,UpdateStudentProfileRequest request,CancellationToken ct=default);}
