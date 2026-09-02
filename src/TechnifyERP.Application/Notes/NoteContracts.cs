namespace TechnifyERP.Application.Notes;
public sealed record StudentNoteDto(int Id,int? CourseId,string? CourseCode,string Title,string Content,DateTime UpdatedAtUtc);
public sealed record NoteCourseDto(int Id,string Code,string Name);
public enum NoteActionResult{Success,NotFound,Forbidden,Invalid}
public interface INoteService{Task<IReadOnlyList<StudentNoteDto>>GetAsync(string studentId,CancellationToken ct=default);Task<IReadOnlyList<NoteCourseDto>>GetCoursesAsync(string studentId,CancellationToken ct=default);Task<NoteActionResult>CreateAsync(string studentId,int? courseId,string title,string content,CancellationToken ct=default);Task<NoteActionResult>UpdateAsync(string studentId,int id,int? courseId,string title,string content,CancellationToken ct=default);Task<NoteActionResult>DeleteAsync(string studentId,int id,CancellationToken ct=default);}
