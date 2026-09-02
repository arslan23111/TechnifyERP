namespace TechnifyERP.Application.Courses;

public sealed record CourseDto(
    int Id,
    string CourseCode,
    string Name,
    int CreditHours,
    decimal MonthlyFee);
