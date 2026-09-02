namespace TechnifyERP.Application.Courses;

public sealed record CreateCourseRequest(
    string CourseCode,
    string Name,
    int CreditHours,
    decimal MonthlyFee);

public sealed record UpdateCourseRequest(
    int Id,
    string CourseCode,
    string Name,
    int CreditHours,
    decimal MonthlyFee);

public enum SaveCourseResult
{
    Success,
    DuplicateCourseCode,
    NotFound
}
