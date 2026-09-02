using Microsoft.EntityFrameworkCore;
using TechnifyERP.Application.Courses;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Infrastructure.Persistence;

namespace TechnifyERP.Infrastructure.Courses;

internal sealed class CourseService(ApplicationDbContext context) : ICourseService
{
    public async Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Courses
            .AsNoTracking()
            .OrderBy(course => course.CourseCode)
            .Select(course => new CourseDto(
                course.Id,
                course.CourseCode,
                course.Name,
                course.CreditHours,
                course.MonthlyFee))
            .ToListAsync(cancellationToken);

    public async Task<CourseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await context.Courses
            .AsNoTracking()
            .Where(course => course.Id == id)
            .Select(course => new CourseDto(
                course.Id,
                course.CourseCode,
                course.Name,
                course.CreditHours,
                course.MonthlyFee))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<SaveCourseResult> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var code = NormalizeCode(request.CourseCode);
        if (await context.Courses.AnyAsync(course => course.CourseCode == code, cancellationToken))
        {
            return SaveCourseResult.DuplicateCourseCode;
        }

        context.Courses.Add(new Course
        {
            CourseCode = code,
            Name = request.Name.Trim(),
            CreditHours = request.CreditHours,
            MonthlyFee = request.MonthlyFee
        });

        await context.SaveChangesAsync(cancellationToken);
        return SaveCourseResult.Success;
    }

    public async Task<SaveCourseResult> UpdateAsync(UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var course = await context.Courses.FindAsync([request.Id], cancellationToken);
        if (course is null)
        {
            return SaveCourseResult.NotFound;
        }

        var code = NormalizeCode(request.CourseCode);
        if (await context.Courses.AnyAsync(item => item.CourseCode == code && item.Id != request.Id, cancellationToken))
        {
            return SaveCourseResult.DuplicateCourseCode;
        }

        course.CourseCode = code;
        course.Name = request.Name.Trim();
        course.CreditHours = request.CreditHours;
        course.MonthlyFee = request.MonthlyFee;
        await context.SaveChangesAsync(cancellationToken);
        return SaveCourseResult.Success;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = await context.Courses.FindAsync([id], cancellationToken);
        if (course is null)
        {
            return false;
        }

        context.Courses.Remove(course);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizeCode(string courseCode) => courseCode.Trim().ToUpperInvariant();

}
