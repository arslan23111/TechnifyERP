namespace TechnifyERP.Application.Courses;

public interface ICourseService
{
    Task<IReadOnlyList<CourseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CourseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SaveCourseResult> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);
    Task<SaveCourseResult> UpdateAsync(UpdateCourseRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
