namespace TechnifyERP.Application.Settings;
public sealed record AcademySettingsDto(string AcademyName,string ShortName,string? Website,string AcademicYear,string? Email,string? Phone,string? Address);
public enum SettingsActionResult{Success,Invalid}
public interface IAcademySettingsService{Task<AcademySettingsDto>GetAsync(CancellationToken ct=default);Task<SettingsActionResult>SaveAsync(AcademySettingsDto settings,CancellationToken ct=default);}
