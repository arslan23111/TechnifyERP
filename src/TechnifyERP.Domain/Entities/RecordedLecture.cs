namespace TechnifyERP.Domain.Entities;
public sealed class RecordedLecture
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string FacultyUserId { get; set; } = string.Empty;
    public int LectureNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Course Course { get; set; } = null!;
}
