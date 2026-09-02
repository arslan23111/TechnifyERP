using TechnifyERP.Domain.Enums;
namespace TechnifyERP.Domain.Entities;
public sealed class CodingChallenge
{
 public int Id{get;set;} public int CourseId{get;set;} public string FacultyUserId{get;set;}=string.Empty; public string Title{get;set;}=string.Empty; public string Description{get;set;}=string.Empty;
 public ChallengeDifficulty Difficulty{get;set;} public string? InputFormat{get;set;} public string? OutputFormat{get;set;} public string? SampleInput{get;set;} public string? SampleOutput{get;set;} public DateTime? DueAtUtc{get;set;} public DateTime CreatedAtUtc{get;set;}=DateTime.UtcNow;
 public Course Course{get;set;}=null!; public ICollection<CodingSubmission> Submissions{get;set;}=[];
}
