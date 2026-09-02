using TechnifyERP.Domain.Enums;
namespace TechnifyERP.Domain.Entities;
public sealed class Notice
{
 public int Id{get;set;} public string CreatedByUserId{get;set;}=string.Empty; public string Title{get;set;}=string.Empty; public string Message{get;set;}=string.Empty;
 public NoticeAudience Audience{get;set;} public int? CourseId{get;set;} public string? StudentUserId{get;set;} public string? ImagePath{get;set;} public DateTime CreatedAtUtc{get;set;}=DateTime.UtcNow;
 public Course? Course{get;set;}
}
