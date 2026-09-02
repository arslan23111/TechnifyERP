namespace TechnifyERP.Domain.Entities;
public sealed class CodingSubmission
{
 public int Id{get;set;} public int ChallengeId{get;set;} public string StudentUserId{get;set;}=string.Empty; public string Language{get;set;}=string.Empty; public string SourceCode{get;set;}=string.Empty; public DateTime SubmittedAtUtc{get;set;}=DateTime.UtcNow;
 public CodingChallenge Challenge{get;set;}=null!;
}
