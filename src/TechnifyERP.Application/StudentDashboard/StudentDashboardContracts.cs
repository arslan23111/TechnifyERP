namespace TechnifyERP.Application.StudentDashboard;
public sealed record StudentDashboardDto(string StudentName,string? RegistrationNumber,int ApprovedCourses,int AttendanceLogs,int PendingAssignments,int UpcomingTests,int UnreadNotices,int EarnedBadges,IReadOnlyList<UpcomingTaskDto> UpcomingTasks,IReadOnlyList<RecentActivityDto> RecentActivities,IReadOnlyList<CourseProgressDto> CourseProgress,IReadOnlyList<LeaderboardEntryDto> Leaderboard);
public sealed record UpcomingTaskDto(string Type,string CourseCode,string Title,DateTime DueAtUtc);
public sealed record RecentActivityDto(string Type,string Description,DateTime OccurredAtUtc);
public sealed record CourseProgressDto(string CourseCode,string CourseName,int CompletedItems,int TotalItems,decimal Percentage);
public sealed record LeaderboardEntryDto(string StudentName,string? RegistrationNumber,decimal Percentage,bool IsCurrentStudent);
public interface IStudentDashboardService{Task<StudentDashboardDto?>GetAsync(string studentId,CancellationToken ct=default);}
