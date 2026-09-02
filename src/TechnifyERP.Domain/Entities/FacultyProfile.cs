namespace TechnifyERP.Domain.Entities;

public sealed class FacultyProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public string? Designation { get; set; }
    public string? Department { get; set; }
    public DateOnly? JoiningDate { get; set; }
}
