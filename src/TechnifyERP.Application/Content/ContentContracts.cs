namespace TechnifyERP.Application.Content;
public sealed record StudyMaterialDto(int Id,int CourseId,string CourseCode,string CourseName,string Title,string? Description,string FilePath,string OriginalFileName,string FacultyName,DateTime UploadedAtUtc);
public sealed record RecordedLectureDto(int Id,int CourseId,string CourseCode,string CourseName,int LectureNumber,string Title,string? Description,string VideoUrl,string FacultyName,DateTime CreatedAtUtc);
public sealed record SaveMaterialRequest(int CourseId,string Title,string? Description,string FilePath,string OriginalFileName);
public sealed record SaveLectureRequest(int CourseId,int LectureNumber,string Title,string? Description,string VideoUrl);
public enum ContentActionResult{Success,NotFound,Forbidden,Invalid,Duplicate}
public interface IContentService
{
 Task<IReadOnlyList<StudyMaterialDto>> GetFacultyMaterialsAsync(string facultyId,CancellationToken ct=default);
 Task<ContentActionResult> AddMaterialAsync(string facultyId,SaveMaterialRequest request,CancellationToken ct=default);
 Task<ContentActionResult> DeleteMaterialAsync(string facultyId,int id,CancellationToken ct=default);
 Task<IReadOnlyList<RecordedLectureDto>> GetFacultyLecturesAsync(string facultyId,CancellationToken ct=default);
 Task<ContentActionResult> AddLectureAsync(string facultyId,SaveLectureRequest request,CancellationToken ct=default);
 Task<ContentActionResult> DeleteLectureAsync(string facultyId,int id,CancellationToken ct=default);
 Task<IReadOnlyList<StudyMaterialDto>> GetStudentMaterialsAsync(string studentId,CancellationToken ct=default);
 Task<IReadOnlyList<RecordedLectureDto>> GetStudentLecturesAsync(string studentId,CancellationToken ct=default);
}
