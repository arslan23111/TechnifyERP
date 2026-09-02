namespace TechnifyERP.Domain.Entities;
public sealed class TestResult
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string StudentUserId { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
    public CourseTest Test { get; set; } = null!;
    public ICollection<TestAnswer> Answers { get; set; } = [];
}
