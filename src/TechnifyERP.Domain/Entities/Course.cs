namespace TechnifyERP.Domain.Entities;

public sealed class Course
{
    public int Id { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int CreditHours { get; set; }
    public decimal MonthlyFee { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
    public ICollection<FacultyCourse> FacultyAssignments { get; set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = [];
    public ICollection<FeeRecord> FeeRecords { get; set; } = [];
    public ICollection<CourseAssignment> Assignments { get; set; } = [];
    public ICollection<CourseTest> Tests { get; set; } = [];
    public ICollection<StudyMaterial> StudyMaterials { get; set; } = [];
    public ICollection<RecordedLecture> RecordedLectures { get; set; } = [];
    public ICollection<Notice> Notices { get; set; } = [];
    public ICollection<ClassSchedule> ClassSchedules { get; set; } = [];
    public ICollection<CodingChallenge> CodingChallenges { get; set; } = [];
    public ICollection<StudentDoubt> StudentDoubts { get; set; } = [];
    public ICollection<CourseCertificate> Certificates { get; set; } = [];
    public ICollection<StudentNote> StudentNotes { get; set; } = [];
}
