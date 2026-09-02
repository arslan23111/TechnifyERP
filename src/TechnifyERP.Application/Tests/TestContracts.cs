namespace TechnifyERP.Application.Tests;
public sealed record SaveTestRequest(int CourseId,string Title,int DurationMinutes,DateTime StartsAtUtc,DateTime EndsAtUtc);
public sealed record SaveQuestionRequest(string Text,string OptionA,string OptionB,string OptionC,string OptionD,char CorrectOption,decimal Marks);
public sealed record TestDto(int Id,int CourseId,string CourseCode,string CourseName,string Title,int DurationMinutes,decimal TotalMarks,DateTime StartsAtUtc,DateTime EndsAtUtc,int QuestionCount,int ResultCount);
public sealed record QuestionDto(int Id,string Text,string OptionA,string OptionB,string OptionC,string OptionD,char CorrectOption,decimal Marks);
public sealed record StudentTestDto(int Id,string CourseCode,string Title,int DurationMinutes,decimal TotalMarks,DateTime StartsAtUtc,DateTime EndsAtUtc,bool Attempted,decimal? Score);
public sealed record TestAttemptDto(int Id,string Title,int DurationMinutes,DateTime EndsAtUtc,IReadOnlyList<QuestionDto> Questions);
public sealed record SubmittedAnswer(int QuestionId,char? SelectedOption);
public sealed record TestResultDto(string TestTitle,string StudentName,decimal Score,decimal TotalMarks,decimal Percentage,DateTime SubmittedAtUtc);
public enum TestActionResult{Success,NotFound,Forbidden,Invalid,NotActive,AlreadyAttempted}
public interface ITestService
{
 Task<IReadOnlyList<TestDto>> GetFacultyTestsAsync(string facultyId,CancellationToken ct=default);
 Task<(TestActionResult Result,int Id)> CreateAsync(string facultyId,SaveTestRequest request,CancellationToken ct=default);
 Task<TestDto?> GetFacultyTestAsync(string facultyId,int id,CancellationToken ct=default);
 Task<IReadOnlyList<QuestionDto>?> GetQuestionsAsync(string facultyId,int testId,CancellationToken ct=default);
 Task<TestActionResult> AddQuestionAsync(string facultyId,int testId,SaveQuestionRequest request,CancellationToken ct=default);
 Task<TestActionResult> DeleteQuestionAsync(string facultyId,int questionId,CancellationToken ct=default);
 Task<IReadOnlyList<StudentTestDto>> GetStudentTestsAsync(string studentId,CancellationToken ct=default);
 Task<TestAttemptDto?> GetAttemptAsync(string studentId,int testId,CancellationToken ct=default);
 Task<(TestActionResult Result,int ResultId)> SubmitAsync(string studentId,int testId,IReadOnlyList<SubmittedAnswer> answers,CancellationToken ct=default);
 Task<TestResultDto?> GetResultAsync(string studentId,int resultId,CancellationToken ct=default);
}
