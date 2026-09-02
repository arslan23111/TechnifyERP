namespace TechnifyERP.Domain.Entities;

public sealed class CourseAssignment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string FacultyUserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueAtUtc { get; set; }
    public decimal TotalMarks { get; set; }
    public string? AttachmentPath { get; set; }
    public bool AllowResubmission { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Course Course { get; set; } = null!;
    public ICollection<AssignmentSubmission> Submissions { get; set; } = [];
}
