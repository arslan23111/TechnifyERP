namespace TechnifyERP.Domain.Entities;
public sealed class StudentDoubt
{
 public int Id{get;set;} public int CourseId{get;set;} public string StudentUserId{get;set;}=string.Empty; public string Question{get;set;}=string.Empty; public DateTime AskedAtUtc{get;set;}=DateTime.UtcNow;
 public string? Answer{get;set;} public string? AnsweredByUserId{get;set;} public DateTime? AnsweredAtUtc{get;set;} public Course Course{get;set;}=null!;
}
