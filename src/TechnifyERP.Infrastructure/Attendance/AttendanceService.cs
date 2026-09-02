using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.Attendance;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Domain.Enums;
using TechnifyERP.Infrastructure.Persistence;

namespace TechnifyERP.Infrastructure.Attendance;

internal sealed class AttendanceService(ApplicationDbContext context) : IAttendanceService
{
    private const decimal MinimumPercentage = 75m;
    public async Task<IReadOnlyList<AttendanceCourseDto>> GetAssignedCoursesAsync(
        string facultyUserId,
        CancellationToken cancellationToken = default) =>
        await context.FacultyCourses
            .AsNoTracking()
            .Where(assignment => assignment.FacultyUserId == facultyUserId)
            .OrderBy(assignment => assignment.Course.CourseCode)
            .Select(assignment => new AttendanceCourseDto(
                assignment.CourseId,
                assignment.Course.CourseCode,
                assignment.Course.Name))
            .ToListAsync(cancellationToken);

    public async Task<AttendanceSheetDto?> GetSheetAsync(
        string facultyUserId,
        int courseId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var course = await context.FacultyCourses
            .AsNoTracking()
            .Where(assignment => assignment.FacultyUserId == facultyUserId && assignment.CourseId == courseId)
            .Select(assignment => new AttendanceCourseDto(
                assignment.CourseId,
                assignment.Course.CourseCode,
                assignment.Course.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return null;
        }

        var students = await (
            from enrollment in context.CourseEnrollments.AsNoTracking()
            join user in context.Users.AsNoTracking() on enrollment.StudentUserId equals user.Id
            join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
            where enrollment.CourseId == courseId && enrollment.Status == EnrollmentStatus.Approved
            orderby user.FullName
            select new
            {
                UserId = user.Id,
                profile.RegistrationNumber,
                user.FullName
            })
            .ToListAsync(cancellationToken);

        var existing = await context.AttendanceRecords
            .AsNoTracking()
            .Where(record => record.CourseId == courseId && record.AttendanceDate == date)
            .ToDictionaryAsync(record => record.StudentUserId, record => record.Status, cancellationToken);

        return new AttendanceSheetDto(
            course.CourseId,
            course.CourseCode,
            course.CourseName,
            date,
            students.Select(student => new AttendanceStudentDto(
                student.UserId,
                student.RegistrationNumber,
                student.FullName,
                existing.GetValueOrDefault(student.UserId)))
                .ToList());
    }

    public async Task<SaveAttendanceResult> SaveAsync(
        string facultyUserId,
        int courseId,
        DateOnly date,
        IReadOnlyList<SaveAttendanceItem> items,
        CancellationToken cancellationToken = default)
    {
        var isAssigned = await context.FacultyCourses.AnyAsync(
            assignment => assignment.FacultyUserId == facultyUserId && assignment.CourseId == courseId,
            cancellationToken);
        if (!isAssigned)
        {
            return SaveAttendanceResult.CourseNotAssigned;
        }

        if (items.Count == 0)
        {
            return SaveAttendanceResult.NoStudents;
        }

        var submitted = items
            .GroupBy(item => item.StudentUserId)
            .Select(group => group.Last())
            .ToList();

        if (submitted.Any(item => !Enum.IsDefined(item.Status)))
        {
            return SaveAttendanceResult.InvalidStudent;
        }

        var enrolledIds = await context.CourseEnrollments
            .Where(enrollment => enrollment.CourseId == courseId && enrollment.Status == EnrollmentStatus.Approved)
            .Select(enrollment => enrollment.StudentUserId)
            .ToListAsync(cancellationToken);

        if (submitted.Any(item => !enrolledIds.Contains(item.StudentUserId)))
        {
            return SaveAttendanceResult.InvalidStudent;
        }

        var studentIds = submitted.Select(item => item.StudentUserId).ToList();
        var records = await context.AttendanceRecords
            .Where(record =>
                record.CourseId == courseId &&
                record.AttendanceDate == date &&
                studentIds.Contains(record.StudentUserId))
            .ToDictionaryAsync(record => record.StudentUserId, cancellationToken);

        foreach (var item in submitted)
        {
            if (records.TryGetValue(item.StudentUserId, out var record))
            {
                record.Status = item.Status;
                record.MarkedByFacultyUserId = facultyUserId;
                record.UpdatedAtUtc = DateTime.UtcNow;
            }
            else
            {
                context.AttendanceRecords.Add(new AttendanceRecord
                {
                    CourseId = courseId,
                    StudentUserId = item.StudentUserId,
                    MarkedByFacultyUserId = facultyUserId,
                    AttendanceDate = date,
                    Status = item.Status
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return SaveAttendanceResult.Success;
    }

    public async Task<StudentAttendanceReportDto?> GetStudentReportAsync(
        string studentUserId,
        CancellationToken cancellationToken = default)
    {
        var student = await (
            from user in context.Users.AsNoTracking()
            join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
            where user.Id == studentUserId
            select new { user.FullName, profile.RegistrationNumber })
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null)
        {
            return null;
        }

        var courses = await context.CourseEnrollments
            .AsNoTracking()
            .Where(enrollment => enrollment.StudentUserId == studentUserId && enrollment.Status == EnrollmentStatus.Approved)
            .OrderBy(enrollment => enrollment.Course.CourseCode)
            .Select(enrollment => new AttendanceCourseDto(
                enrollment.CourseId,
                enrollment.Course.CourseCode,
                enrollment.Course.Name))
            .ToListAsync(cancellationToken);

        var records = await (
            from record in context.AttendanceRecords.AsNoTracking()
            join marker in context.Users.AsNoTracking() on record.MarkedByFacultyUserId equals marker.Id
            where record.StudentUserId == studentUserId
            orderby record.AttendanceDate descending
            select new
            {
                record.CourseId,
                record.AttendanceDate,
                record.Status,
                MarkedBy = marker.FullName
            })
            .ToListAsync(cancellationToken);

        var reportCourses = courses.Select(course =>
        {
            var courseRecords = records.Where(record => record.CourseId == course.CourseId).ToList();
            return new StudentCourseAttendanceDto(
                course.CourseId,
                course.CourseCode,
                course.CourseName,
                BuildSummary(courseRecords.Select(record => record.Status)),
                courseRecords.Select(record => new AttendanceHistoryDto(
                    record.AttendanceDate,
                    record.Status,
                    record.MarkedBy)).ToList());
        }).ToList();

        return new StudentAttendanceReportDto(student.FullName, student.RegistrationNumber, reportCourses);
    }

    public async Task<AdminAttendanceReportDto> GetAdminReportAsync(
        int? courseId,
        CancellationToken cancellationToken = default)
    {
        var courses = await context.Courses
            .AsNoTracking()
            .OrderBy(course => course.CourseCode)
            .Select(course => new AttendanceCourseDto(course.Id, course.CourseCode, course.Name))
            .ToListAsync(cancellationToken);

        var students = await (
            from enrollment in context.CourseEnrollments.AsNoTracking()
            join user in context.Users.AsNoTracking() on enrollment.StudentUserId equals user.Id
            join profile in context.StudentProfiles.AsNoTracking() on user.Id equals profile.UserId
            where enrollment.Status == EnrollmentStatus.Approved
                && (!courseId.HasValue || enrollment.CourseId == courseId.Value)
            orderby enrollment.Course.CourseCode, user.FullName
            select new
            {
                enrollment.CourseId,
                enrollment.Course.CourseCode,
                CourseName = enrollment.Course.Name,
                StudentUserId = user.Id,
                StudentName = user.FullName,
                profile.RegistrationNumber
            })
            .ToListAsync(cancellationToken);

        var courseIds = students.Select(student => student.CourseId).Distinct().ToList();
        var studentIds = students.Select(student => student.StudentUserId).Distinct().ToList();
        var records = await context.AttendanceRecords
            .AsNoTracking()
            .Where(record => courseIds.Contains(record.CourseId) && studentIds.Contains(record.StudentUserId))
            .Select(record => new { record.CourseId, record.StudentUserId, record.Status })
            .ToListAsync(cancellationToken);

        var rows = students.Select(student => new AdminAttendanceRowDto(
            student.CourseId,
            student.CourseCode,
            student.CourseName,
            student.StudentUserId,
            student.StudentName,
            student.RegistrationNumber,
            BuildSummary(records
                .Where(record => record.CourseId == student.CourseId && record.StudentUserId == student.StudentUserId)
                .Select(record => record.Status))))
            .ToList();

        return new AdminAttendanceReportDto(courseId, courses, rows);
    }

    private static AttendanceSummaryDto BuildSummary(IEnumerable<AttendanceStatus> statuses)
    {
        var statusList = statuses.ToList();
        var present = statusList.Count(status => status == AttendanceStatus.Present);
        var absent = statusList.Count(status => status == AttendanceStatus.Absent);
        var leave = statusList.Count(status => status == AttendanceStatus.Leave);
        var countedClasses = present + absent;
        var percentage = countedClasses == 0 ? 0 : Math.Round(present * 100m / countedClasses, 2);
        var needed = 0;

        while (countedClasses > 0 && percentage < MinimumPercentage)
        {
            needed++;
            percentage = Math.Round((present + needed) * 100m / (countedClasses + needed), 2);
        }

        var actualPercentage = countedClasses == 0 ? 0 : Math.Round(present * 100m / countedClasses, 2);
        return new AttendanceSummaryDto(present, absent, leave, actualPercentage, needed);
    }
}
