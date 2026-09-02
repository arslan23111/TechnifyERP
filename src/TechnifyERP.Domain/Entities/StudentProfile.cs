namespace TechnifyERP.Domain.Entities;

public sealed class StudentProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
}
