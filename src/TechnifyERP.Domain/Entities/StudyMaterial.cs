namespace TechnifyERP.Domain.Entities;
public sealed class StudyMaterial
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string FacultyUserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    public Course Course { get; set; } = null!;
}
