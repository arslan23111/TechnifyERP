namespace TechnifyERP.Domain.Entities;

public sealed class AssignmentSubmission
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public string StudentUserId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime SubmittedAtUtc { get; set; } = DateTime.UtcNow;
    public int SubmissionNumber { get; set; } = 1;
    public decimal? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAtUtc { get; set; }
    public CourseAssignment Assignment { get; set; } = null!;
}
