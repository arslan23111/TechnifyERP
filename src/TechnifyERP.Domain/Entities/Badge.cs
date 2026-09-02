namespace TechnifyERP.Domain.Entities;
public sealed class Badge { public int Id{get;set;} public string Name{get;set;}=string.Empty; public string Description{get;set;}=string.Empty; public string? IconPath{get;set;} public DateTime CreatedAtUtc{get;set;}=DateTime.UtcNow; public ICollection<StudentBadge> StudentBadges{get;set;}=[]; }
