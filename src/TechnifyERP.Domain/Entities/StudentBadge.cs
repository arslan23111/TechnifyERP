namespace TechnifyERP.Domain.Entities;
public sealed class StudentBadge { public int Id{get;set;} public int BadgeId{get;set;} public string StudentUserId{get;set;}=string.Empty; public string AwardedByUserId{get;set;}=string.Empty; public DateTime AwardedAtUtc{get;set;}=DateTime.UtcNow; public Badge Badge{get;set;}=null!; }
