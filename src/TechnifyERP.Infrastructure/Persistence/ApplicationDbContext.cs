using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechnifyERP.Domain.Entities;
using TechnifyERP.Infrastructure.Identity;

namespace TechnifyERP.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<FacultyProfile> FacultyProfiles => Set<FacultyProfile>();
    public DbSet<FacultyCourse> FacultyCourses => Set<FacultyCourse>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<FeeRecord> FeeRecords => Set<FeeRecord>();
    public DbSet<InstallmentRequest> InstallmentRequests => Set<InstallmentRequest>();
    public DbSet<CourseAssignment> CourseAssignments => Set<CourseAssignment>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<CourseTest> CourseTests => Set<CourseTest>();
    public DbSet<TestQuestion> TestQuestions => Set<TestQuestion>();
    public DbSet<TestResult> TestResults => Set<TestResult>();
    public DbSet<TestAnswer> TestAnswers => Set<TestAnswer>();
    public DbSet<StudyMaterial> StudyMaterials => Set<StudyMaterial>();
    public DbSet<RecordedLecture> RecordedLectures => Set<RecordedLecture>();
    public DbSet<Notice> Notices => Set<Notice>();
    public DbSet<ClassSchedule> ClassSchedules => Set<ClassSchedule>();
    public DbSet<CodingChallenge> CodingChallenges => Set<CodingChallenge>();
    public DbSet<CodingSubmission> CodingSubmissions => Set<CodingSubmission>();
    public DbSet<StudentDoubt> StudentDoubts => Set<StudentDoubt>();
    public DbSet<PrivateMessage> PrivateMessages => Set<PrivateMessage>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<StudentBadge> StudentBadges => Set<StudentBadge>();
    public DbSet<CourseCertificate> CourseCertificates => Set<CourseCertificate>();
    public DbSet<StudentNote> StudentNotes => Set<StudentNote>();
    public DbSet<AcademySettings> AcademySettings => Set<AcademySettings>();
    public DbSet<CourseChangeRequest> CourseChangeRequests => Set<CourseChangeRequest>();
    public DbSet<HomepageContent> HomepageContents => Set<HomepageContent>();
    public DbSet<HomepageStat> HomepageStats => Set<HomepageStat>();
    public DbSet<HomepageFeature> HomepageFeatures => Set<HomepageFeature>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Course>(entity =>
        {
            entity.Property(course => course.CourseCode).HasMaxLength(20).IsRequired();
            entity.Property(course => course.Name).HasMaxLength(150).IsRequired();
            entity.Property(course => course.MonthlyFee).HasPrecision(18, 2);
            entity.HasIndex(course => course.CourseCode).IsUnique();
        });

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(150).IsRequired();
            entity.Property(user => user.Address).HasMaxLength(500);
            entity.Property(user => user.Gender).HasMaxLength(20);
            entity.Property(user => user.Cnic).HasMaxLength(20);
            entity.Property(user => user.ProfilePicturePath).HasMaxLength(500);
            entity.HasIndex(user => user.Cnic).IsUnique().HasFilter("[Cnic] IS NOT NULL");
        });

        builder.Entity<StudentProfile>(entity =>
        {
            entity.Property(profile => profile.UserId).HasMaxLength(450).IsRequired();
            entity.Property(profile => profile.RegistrationNumber).HasMaxLength(30);
            entity.HasIndex(profile => profile.UserId).IsUnique();
            entity.HasIndex(profile => profile.RegistrationNumber).IsUnique().HasFilter("[RegistrationNumber] IS NOT NULL");
            entity.HasOne<ApplicationUser>()
                .WithOne(user => user.StudentProfile)
                .HasForeignKey<StudentProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FacultyProfile>(entity =>
        {
            entity.Property(profile => profile.UserId).HasMaxLength(450).IsRequired();
            entity.Property(profile => profile.EmployeeCode).HasMaxLength(30);
            entity.Property(profile => profile.Designation).HasMaxLength(100);
            entity.Property(profile => profile.Department).HasMaxLength(100);
            entity.HasIndex(profile => profile.UserId).IsUnique();
            entity.HasIndex(profile => profile.EmployeeCode).IsUnique().HasFilter("[EmployeeCode] IS NOT NULL");
            entity.HasOne<ApplicationUser>()
                .WithOne(user => user.FacultyProfile)
                .HasForeignKey<FacultyProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CourseEnrollment>(entity =>
        {
            entity.Property(enrollment => enrollment.StudentUserId).HasMaxLength(450).IsRequired();
            entity.HasIndex(enrollment => new { enrollment.StudentUserId, enrollment.CourseId }).IsUnique();
            entity.HasOne(enrollment => enrollment.Course)
                .WithMany(course => course.Enrollments)
                .HasForeignKey(enrollment => enrollment.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>()
                .WithMany(user => user.CourseEnrollments)
                .HasForeignKey(enrollment => enrollment.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FacultyCourse>(entity =>
        {
            entity.Property(assignment => assignment.FacultyUserId).HasMaxLength(450).IsRequired();
            entity.HasIndex(assignment => new { assignment.FacultyUserId, assignment.CourseId }).IsUnique();
            entity.HasOne(assignment => assignment.Course)
                .WithMany(course => course.FacultyAssignments)
                .HasForeignKey(assignment => assignment.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>()
                .WithMany(user => user.FacultyCourses)
                .HasForeignKey(assignment => assignment.FacultyUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AttendanceRecord>(entity =>
        {
            entity.Property(record => record.StudentUserId).HasMaxLength(450).IsRequired();
            entity.Property(record => record.MarkedByFacultyUserId).HasMaxLength(450).IsRequired();
            entity.HasIndex(record => new { record.CourseId, record.StudentUserId, record.AttendanceDate }).IsUnique();
            entity.HasOne(record => record.Course)
                .WithMany(course => course.AttendanceRecords)
                .HasForeignKey(record => record.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>()
                .WithMany(user => user.StudentAttendanceRecords)
                .HasForeignKey(record => record.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>()
                .WithMany(user => user.MarkedAttendanceRecords)
                .HasForeignKey(record => record.MarkedByFacultyUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<FeeRecord>(entity =>
        {
            entity.Property(record => record.StudentUserId).HasMaxLength(450).IsRequired();
            entity.Property(record => record.AmountDue).HasPrecision(18, 2);
            entity.Property(record => record.AmountPaid).HasPrecision(18, 2);
            entity.Property(record => record.SubmittedAmount).HasPrecision(18, 2);
            entity.Property(record => record.PaymentReceiptPath).HasMaxLength(500);
            entity.Property(record => record.AdminRemarks).HasMaxLength(500);
            entity.HasIndex(record => new { record.EnrollmentId, record.Month, record.Year }).IsUnique();
            entity.HasOne(record => record.Enrollment)
                .WithMany(enrollment => enrollment.FeeRecords)
                .HasForeignKey(record => record.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(record => record.Course)
                .WithMany(course => course.FeeRecords)
                .HasForeignKey(record => record.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(record => record.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<InstallmentRequest>(entity =>
        {
            entity.Property(request => request.StudentUserId).HasMaxLength(450).IsRequired();
            entity.Property(request => request.RequestedAmount).HasPrecision(18, 2);
            entity.Property(request => request.Reason).HasMaxLength(1000).IsRequired();
            entity.Property(request => request.AdminRemarks).HasMaxLength(500);
            entity.HasIndex(request => new { request.FeeRecordId, request.StudentUserId, request.Status });
            entity.HasOne(request => request.FeeRecord)
                .WithMany()
                .HasForeignKey(request => request.FeeRecordId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(request => request.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CourseAssignment>(entity =>
        {
            entity.Property(item => item.FacultyUserId).HasMaxLength(450).IsRequired();
            entity.Property(item => item.Title).HasMaxLength(200).IsRequired();
            entity.Property(item => item.Description).HasMaxLength(4000).IsRequired();
            entity.Property(item => item.TotalMarks).HasPrecision(10, 2);
            entity.Property(item => item.AttachmentPath).HasMaxLength(500);
            entity.HasOne(item => item.Course).WithMany(course => course.Assignments)
                .HasForeignKey(item => item.CourseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(item => item.FacultyUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AssignmentSubmission>(entity =>
        {
            entity.Property(item => item.StudentUserId).HasMaxLength(450).IsRequired();
            entity.Property(item => item.FilePath).HasMaxLength(500).IsRequired();
            entity.Property(item => item.Marks).HasPrecision(10, 2);
            entity.Property(item => item.Feedback).HasMaxLength(2000);
            entity.HasIndex(item => new { item.AssignmentId, item.StudentUserId }).IsUnique();
            entity.HasOne(item => item.Assignment).WithMany(assignment => assignment.Submissions)
                .HasForeignKey(item => item.AssignmentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(item => item.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        builder.Entity<CourseTest>(entity => { entity.Property(x=>x.FacultyUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Title).HasMaxLength(200).IsRequired(); entity.Property(x=>x.TotalMarks).HasPrecision(10,2); entity.HasOne(x=>x.Course).WithMany(x=>x.Tests).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.FacultyUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<TestQuestion>(entity => { entity.Property(x=>x.Text).HasMaxLength(2000).IsRequired(); entity.Property(x=>x.OptionA).HasMaxLength(1000).IsRequired(); entity.Property(x=>x.OptionB).HasMaxLength(1000).IsRequired(); entity.Property(x=>x.OptionC).HasMaxLength(1000).IsRequired(); entity.Property(x=>x.OptionD).HasMaxLength(1000).IsRequired(); entity.Property(x=>x.Marks).HasPrecision(10,2); entity.HasOne(x=>x.Test).WithMany(x=>x.Questions).HasForeignKey(x=>x.TestId).OnDelete(DeleteBehavior.Cascade); });
        builder.Entity<TestResult>(entity => { entity.Property(x=>x.StudentUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Score).HasPrecision(10,2); entity.HasIndex(x=>new{x.TestId,x.StudentUserId}).IsUnique(); entity.HasOne(x=>x.Test).WithMany(x=>x.Results).HasForeignKey(x=>x.TestId).OnDelete(DeleteBehavior.Cascade); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<TestAnswer>(entity => { entity.Property(x=>x.MarksAwarded).HasPrecision(10,2); entity.HasIndex(x=>new{x.TestResultId,x.QuestionId}).IsUnique(); entity.HasOne(x=>x.TestResult).WithMany(x=>x.Answers).HasForeignKey(x=>x.TestResultId).OnDelete(DeleteBehavior.Cascade); entity.HasOne<TestQuestion>().WithMany().HasForeignKey(x=>x.QuestionId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<StudyMaterial>(entity => { entity.Property(x=>x.FacultyUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Title).HasMaxLength(200).IsRequired(); entity.Property(x=>x.Description).HasMaxLength(2000); entity.Property(x=>x.FilePath).HasMaxLength(500).IsRequired(); entity.Property(x=>x.OriginalFileName).HasMaxLength(255).IsRequired(); entity.HasOne(x=>x.Course).WithMany(x=>x.StudyMaterials).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.FacultyUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<RecordedLecture>(entity => { entity.Property(x=>x.FacultyUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Title).HasMaxLength(200).IsRequired(); entity.Property(x=>x.Description).HasMaxLength(2000); entity.Property(x=>x.VideoUrl).HasMaxLength(1000).IsRequired(); entity.HasIndex(x=>new{x.CourseId,x.LectureNumber}).IsUnique(); entity.HasOne(x=>x.Course).WithMany(x=>x.RecordedLectures).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.FacultyUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<Notice>(entity => { entity.Property(x=>x.CreatedByUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.StudentUserId).HasMaxLength(450); entity.Property(x=>x.Title).HasMaxLength(200).IsRequired(); entity.Property(x=>x.Message).HasMaxLength(4000).IsRequired(); entity.Property(x=>x.ImagePath).HasMaxLength(500); entity.HasOne(x=>x.Course).WithMany(x=>x.Notices).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<ClassSchedule>(entity => { entity.Property(x=>x.FacultyUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Room).HasMaxLength(100).IsRequired(); entity.HasIndex(x=>new{x.FacultyUserId,x.DayOfWeek,x.StartTime}); entity.HasOne(x=>x.Course).WithMany(x=>x.ClassSchedules).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.FacultyUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<CodingChallenge>(entity => { entity.Property(x=>x.FacultyUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Title).HasMaxLength(200).IsRequired(); entity.Property(x=>x.Description).HasMaxLength(8000).IsRequired(); entity.Property(x=>x.InputFormat).HasMaxLength(2000); entity.Property(x=>x.OutputFormat).HasMaxLength(2000); entity.Property(x=>x.SampleInput).HasMaxLength(4000); entity.Property(x=>x.SampleOutput).HasMaxLength(4000); entity.HasOne(x=>x.Course).WithMany(x=>x.CodingChallenges).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.FacultyUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<CodingSubmission>(entity => { entity.Property(x=>x.StudentUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Language).HasMaxLength(50).IsRequired(); entity.Property(x=>x.SourceCode).HasMaxLength(30000).IsRequired(); entity.HasIndex(x=>new{x.ChallengeId,x.StudentUserId,x.SubmittedAtUtc}); entity.HasOne(x=>x.Challenge).WithMany(x=>x.Submissions).HasForeignKey(x=>x.ChallengeId).OnDelete(DeleteBehavior.Cascade); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<StudentDoubt>(entity => { entity.Property(x=>x.StudentUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.AnsweredByUserId).HasMaxLength(450); entity.Property(x=>x.Question).HasMaxLength(4000).IsRequired(); entity.Property(x=>x.Answer).HasMaxLength(4000); entity.HasOne(x=>x.Course).WithMany(x=>x.StudentDoubts).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.AnsweredByUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<PrivateMessage>(entity => { entity.Property(x=>x.SenderUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.RecipientUserId).HasMaxLength(450).IsRequired(); entity.Property(x=>x.Subject).HasMaxLength(200).IsRequired(); entity.Property(x=>x.Body).HasMaxLength(4000).IsRequired(); entity.HasIndex(x=>new{x.RecipientUserId,x.ReadAtUtc,x.SentAtUtc}); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.SenderUserId).OnDelete(DeleteBehavior.Restrict); entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.RecipientUserId).OnDelete(DeleteBehavior.Restrict); });
        builder.Entity<Badge>(entity=>{entity.Property(x=>x.Name).HasMaxLength(100).IsRequired();entity.Property(x=>x.Description).HasMaxLength(1000).IsRequired();entity.Property(x=>x.IconPath).HasMaxLength(500);entity.HasIndex(x=>x.Name).IsUnique();});
        builder.Entity<StudentBadge>(entity=>{entity.Property(x=>x.StudentUserId).HasMaxLength(450).IsRequired();entity.Property(x=>x.AwardedByUserId).HasMaxLength(450).IsRequired();entity.HasIndex(x=>new{x.BadgeId,x.StudentUserId}).IsUnique();entity.HasOne(x=>x.Badge).WithMany(x=>x.StudentBadges).HasForeignKey(x=>x.BadgeId).OnDelete(DeleteBehavior.Cascade);entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Restrict);entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.AwardedByUserId).OnDelete(DeleteBehavior.Restrict);});
        builder.Entity<CourseCertificate>(entity=>{entity.Property(x=>x.CertificateCode).HasMaxLength(50).IsRequired();entity.Property(x=>x.StudentUserId).HasMaxLength(450).IsRequired();entity.Property(x=>x.IssuedByUserId).HasMaxLength(450).IsRequired();entity.HasIndex(x=>x.CertificateCode).IsUnique();entity.HasIndex(x=>new{x.StudentUserId,x.CourseId}).IsUnique();entity.HasOne(x=>x.Course).WithMany(x=>x.Certificates).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Restrict);entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Restrict);entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.IssuedByUserId).OnDelete(DeleteBehavior.Restrict);});
        builder.Entity<StudentNote>(entity=>{entity.Property(x=>x.StudentUserId).HasMaxLength(450).IsRequired();entity.Property(x=>x.Title).HasMaxLength(200).IsRequired();entity.Property(x=>x.Content).HasMaxLength(8000).IsRequired();entity.HasIndex(x=>new{x.StudentUserId,x.UpdatedAtUtc});entity.HasOne(x=>x.Course).WithMany(x=>x.StudentNotes).HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.SetNull);entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Restrict);});
        builder.Entity<AcademySettings>(entity=>{entity.Property(x=>x.AcademyName).HasMaxLength(200).IsRequired();entity.Property(x=>x.ShortName).HasMaxLength(50).IsRequired();entity.Property(x=>x.Website).HasMaxLength(500);entity.Property(x=>x.AcademicYear).HasMaxLength(50).IsRequired();entity.Property(x=>x.Email).HasMaxLength(200);entity.Property(x=>x.Phone).HasMaxLength(50);entity.Property(x=>x.Address).HasMaxLength(1000);});
        builder.Entity<CourseChangeRequest>(entity=>{entity.Property(x=>x.StudentUserId).HasMaxLength(450).IsRequired();entity.Property(x=>x.Reason).HasMaxLength(1000).IsRequired();entity.Property(x=>x.AdminRemarks).HasMaxLength(500);entity.HasIndex(x=>new{x.StudentUserId,x.Status});entity.HasOne(x=>x.FromCourse).WithMany().HasForeignKey(x=>x.FromCourseId).OnDelete(DeleteBehavior.Restrict);entity.HasOne(x=>x.ToCourse).WithMany().HasForeignKey(x=>x.ToCourseId).OnDelete(DeleteBehavior.Restrict);entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(x=>x.StudentUserId).OnDelete(DeleteBehavior.Cascade);});
        builder.Entity<HomepageContent>(entity=>{entity.Property(x=>x.HeroTitle).HasMaxLength(200).IsRequired();entity.Property(x=>x.HeroSubtitle).HasMaxLength(1000).IsRequired();entity.Property(x=>x.PrimaryButtonText).HasMaxLength(100).IsRequired();entity.Property(x=>x.PrimaryButtonUrl).HasMaxLength(500);entity.Property(x=>x.FooterText).HasMaxLength(500).IsRequired();});
        builder.Entity<HomepageStat>(entity=>{entity.Property(x=>x.Label).HasMaxLength(100).IsRequired();entity.Property(x=>x.Value).HasMaxLength(100).IsRequired();});
        builder.Entity<HomepageFeature>(entity=>{entity.Property(x=>x.Title).HasMaxLength(150).IsRequired();entity.Property(x=>x.Description).HasMaxLength(1000).IsRequired();});
    }
}
