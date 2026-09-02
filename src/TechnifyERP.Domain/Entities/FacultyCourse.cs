namespace TechnifyERP.Domain.Entities;

public sealed class FacultyCourse
{
    public int Id { get; set; }
    public string FacultyUserId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    public Course Course { get; set; } = null!;
}
