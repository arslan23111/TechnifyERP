namespace TechnifyERP.Domain.Entities;
public sealed class CourseTest
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string FacultyUserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal TotalMarks { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime EndsAtUtc { get; set; }
    public Course Course { get; set; } = null!;
    public ICollection<TestQuestion> Questions { get; set; } = [];
    public ICollection<TestResult> Results { get; set; } = [];
}
