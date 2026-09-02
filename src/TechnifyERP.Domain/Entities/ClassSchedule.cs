namespace TechnifyERP.Domain.Entities;
public sealed class ClassSchedule
{
 public int Id{get;set;} public int CourseId{get;set;} public string FacultyUserId{get;set;}=string.Empty; public DayOfWeek DayOfWeek{get;set;}
 public TimeOnly StartTime{get;set;} public TimeOnly EndTime{get;set;} public string Room{get;set;}=string.Empty; public DateTime CreatedAtUtc{get;set;}=DateTime.UtcNow;
 public Course Course{get;set;}=null!;
}
