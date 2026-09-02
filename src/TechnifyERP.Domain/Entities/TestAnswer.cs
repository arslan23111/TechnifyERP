namespace TechnifyERP.Domain.Entities;
public sealed class TestAnswer
{
    public int Id { get; set; }
    public int TestResultId { get; set; }
    public int QuestionId { get; set; }
    public char? SelectedOption { get; set; }
    public bool IsCorrect { get; set; }
    public decimal MarksAwarded { get; set; }
    public TestResult TestResult { get; set; } = null!;
}
