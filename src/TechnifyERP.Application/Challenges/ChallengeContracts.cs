using TechnifyERP.Domain.Enums;
namespace TechnifyERP.Application.Challenges;
public sealed record SaveChallengeRequest(int CourseId,string Title,string Description,ChallengeDifficulty Difficulty,string? InputFormat,string? OutputFormat,string? SampleInput,string? SampleOutput,DateTime? DueAtUtc);
public sealed record ChallengeDto(int Id,string CourseCode,string CourseName,string Title,string Description,ChallengeDifficulty Difficulty,string? InputFormat,string? OutputFormat,string? SampleInput,string? SampleOutput,DateTime? DueAtUtc,int SubmissionCount);
public sealed record CodingSubmissionDto(int Id,string StudentName,string? RegistrationNumber,string Language,string SourceCode,DateTime SubmittedAtUtc);
public enum ChallengeActionResult{Success,NotFound,Forbidden,Invalid,Closed}
public interface IChallengeService
{
 Task<IReadOnlyList<ChallengeDto>>GetFacultyChallengesAsync(string facultyId,CancellationToken ct=default); Task<ChallengeActionResult>CreateAsync(string facultyId,SaveChallengeRequest request,CancellationToken ct=default); Task<ChallengeActionResult>DeleteAsync(string facultyId,int id,CancellationToken ct=default);
 Task<IReadOnlyList<CodingSubmissionDto>?>GetSubmissionsAsync(string facultyId,int challengeId,CancellationToken ct=default); Task<IReadOnlyList<ChallengeDto>>GetStudentChallengesAsync(string studentId,CancellationToken ct=default); Task<ChallengeActionResult>SubmitAsync(string studentId,int challengeId,string language,string sourceCode,CancellationToken ct=default); Task<IReadOnlyList<CodingSubmissionDto>>GetStudentSubmissionsAsync(string studentId,int challengeId,CancellationToken ct=default);
}
