namespace TechnifyERP.Domain.Entities;
public sealed class TestQuestion
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public char CorrectOption { get; set; }
    public decimal Marks { get; set; }
    public CourseTest Test { get; set; } = null!;
}
