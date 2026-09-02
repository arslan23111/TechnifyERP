using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.StudentDashboard;
using TechnifyERP.Domain.Enums;
using TechnifyERP.Infrastructure.Persistence;

namespace TechnifyERP.Infrastructure.StudentDashboard;

internal sealed class StudentDashboardService(ApplicationDbContext db) : IStudentDashboardService
{
    public async Task<StudentDashboardDto?> GetAsync(string studentId, CancellationToken cancellationToken = default)
    {
        var user = await (from u in db.Users.AsNoTracking()
                          join p in db.StudentProfiles.AsNoTracking() on u.Id equals p.UserId
                          where u.Id == studentId
                          select new { u.FullName, p.RegistrationNumber }).FirstOrDefaultAsync(cancellationToken);
        if (user is null) return null;

        var approvedEnrollments = db.CourseEnrollments.Where(x => x.StudentUserId == studentId && x.Status == EnrollmentStatus.Approved);
        var courseIds = approvedEnrollments.Select(x => x.CourseId);
        var now = DateTime.UtcNow;
        var courseCount = await courseIds.CountAsync(cancellationToken);
        var attendance = await db.AttendanceRecords.CountAsync(x => x.StudentUserId == studentId, cancellationToken);
        var pendingAssignments = await db.CourseAssignments.CountAsync(x => courseIds.Contains(x.CourseId) && x.DueAtUtc > now && !x.Submissions.Any(y => y.StudentUserId == studentId), cancellationToken);
        var upcomingTests = await db.CourseTests.CountAsync(x => courseIds.Contains(x.CourseId) && x.EndsAtUtc >= now && !x.Results.Any(y => y.StudentUserId == studentId), cancellationToken);
        var recentNotices = await db.Notices.CountAsync(x => x.CreatedAtUtc >= now.AddDays(-30) && (x.Audience == NoticeAudience.AllStudents || (x.Audience == NoticeAudience.Student && x.StudentUserId == studentId) || (x.Audience == NoticeAudience.Course && x.CourseId.HasValue && courseIds.Contains(x.CourseId.Value))), cancellationToken);
        var badges = await db.StudentBadges.CountAsync(x => x.StudentUserId == studentId, cancellationToken);

        var assignments = await db.CourseAssignments.AsNoTracking().Where(x => courseIds.Contains(x.CourseId) && x.DueAtUtc >= now && !x.Submissions.Any(y => y.StudentUserId == studentId)).OrderBy(x => x.DueAtUtc).Take(5).Select(x => new UpcomingTaskDto("Assignment", x.Course.CourseCode, x.Title, x.DueAtUtc)).ToListAsync(cancellationToken);
        var tests = await db.CourseTests.AsNoTracking().Where(x => courseIds.Contains(x.CourseId) && x.EndsAtUtc >= now && !x.Results.Any(y => y.StudentUserId == studentId)).OrderBy(x => x.StartsAtUtc).Take(5).Select(x => new UpcomingTaskDto("Test", x.Course.CourseCode, x.Title, x.StartsAtUtc)).ToListAsync(cancellationToken);

        var activities = new List<RecentActivityDto>();
        activities.AddRange(await db.AttendanceRecords.AsNoTracking().Where(x => x.StudentUserId == studentId).OrderByDescending(x => x.UpdatedAtUtc).Take(5).Select(x => new RecentActivityDto("Attendance", x.Course.CourseCode + " - " + x.Status, x.UpdatedAtUtc)).ToListAsync(cancellationToken));
        activities.AddRange(await db.AssignmentSubmissions.AsNoTracking().Where(x => x.StudentUserId == studentId).OrderByDescending(x => x.SubmittedAtUtc).Take(5).Select(x => new RecentActivityDto("Assignment", x.Assignment.Title + " submitted", x.SubmittedAtUtc)).ToListAsync(cancellationToken));
        activities.AddRange(await db.TestResults.AsNoTracking().Where(x => x.StudentUserId == studentId).OrderByDescending(x => x.SubmittedAtUtc).Take(5).Select(x => new RecentActivityDto("Test", x.Test.Title + " completed", x.SubmittedAtUtc)).ToListAsync(cancellationToken));

        var courseRows = await approvedEnrollments.AsNoTracking().Select(x => new { x.CourseId, x.Course.CourseCode, x.Course.Name }).ToListAsync(cancellationToken);
        var progress = new List<CourseProgressDto>();
        foreach (var course in courseRows)
        {
            var assignmentTotal = await db.CourseAssignments.CountAsync(x => x.CourseId == course.CourseId, cancellationToken);
            var assignmentDone = await db.AssignmentSubmissions.CountAsync(x => x.StudentUserId == studentId && x.Assignment.CourseId == course.CourseId, cancellationToken);
            var testTotal = await db.CourseTests.CountAsync(x => x.CourseId == course.CourseId, cancellationToken);
            var testDone = await db.TestResults.CountAsync(x => x.StudentUserId == studentId && x.Test.CourseId == course.CourseId, cancellationToken);
            var total = assignmentTotal + testTotal;
            var completed = Math.Min(total, assignmentDone + testDone);
            progress.Add(new CourseProgressDto(course.CourseCode, course.Name, completed, total, total == 0 ? 0 : Math.Round(completed * 100m / total, 2)));
        }

        var candidateIds = await db.CourseEnrollments.Where(x => x.Status == EnrollmentStatus.Approved && courseIds.Contains(x.CourseId)).Select(x => x.StudentUserId).Distinct().ToListAsync(cancellationToken);
        var scores = await db.TestResults.AsNoTracking().Where(x => candidateIds.Contains(x.StudentUserId) && courseIds.Contains(x.Test.CourseId)).Select(x => new { x.StudentUserId, x.Score, x.Test.TotalMarks }).ToListAsync(cancellationToken);
        var names = await db.StudentProfiles.AsNoTracking().Where(x => candidateIds.Contains(x.UserId)).Join(db.Users.AsNoTracking(), p => p.UserId, u => u.Id, (p, u) => new { p.UserId, u.FullName, p.RegistrationNumber }).ToListAsync(cancellationToken);
        var leaderboard = scores.GroupBy(x => x.StudentUserId).Select(g => new { StudentId = g.Key, Percentage = g.Sum(x => x.TotalMarks) == 0 ? 0 : Math.Round(g.Sum(x => x.Score) * 100m / g.Sum(x => x.TotalMarks), 2) }).Join(names, x => x.StudentId, x => x.UserId, (score, name) => new LeaderboardEntryDto(name.FullName, name.RegistrationNumber, score.Percentage, score.StudentId == studentId)).OrderByDescending(x => x.Percentage).Take(5).ToList();

        return new StudentDashboardDto(user.FullName, user.RegistrationNumber, courseCount, attendance, pendingAssignments, upcomingTests, recentNotices, badges, assignments.Concat(tests).OrderBy(x => x.DueAtUtc).Take(5).ToList(), activities.OrderByDescending(x => x.OccurredAtUtc).Take(10).ToList(), progress, leaderboard);
    }
}
