using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnifyERP.Application.Courses;
using TechnifyERP.Application.Accounts;
using TechnifyERP.Application.Admin;
using TechnifyERP.Infrastructure.Accounts;
using TechnifyERP.Infrastructure.Admin;
using TechnifyERP.Application.Faculty;
using TechnifyERP.Infrastructure.Faculty;
using TechnifyERP.Application.Attendance;
using TechnifyERP.Infrastructure.Attendance;
using TechnifyERP.Infrastructure.Courses;
using TechnifyERP.Infrastructure.Identity;
using TechnifyERP.Infrastructure.Persistence;
using TechnifyERP.Application.Fees;
using TechnifyERP.Infrastructure.Fees;
using TechnifyERP.Application.Assignments;
using TechnifyERP.Infrastructure.Assignments;
using TechnifyERP.Application.Tests;
using TechnifyERP.Infrastructure.Tests;
using TechnifyERP.Application.Content;
using TechnifyERP.Infrastructure.Content;
using TechnifyERP.Application.Notices;
using TechnifyERP.Infrastructure.Notices;
using TechnifyERP.Application.Schedules;
using TechnifyERP.Infrastructure.Schedules;
using TechnifyERP.Application.Challenges;
using TechnifyERP.Infrastructure.Challenges;
using TechnifyERP.Application.Doubts;
using TechnifyERP.Infrastructure.Doubts;
using TechnifyERP.Application.Messages;
using TechnifyERP.Infrastructure.Messages;
using TechnifyERP.Application.Badges;
using TechnifyERP.Infrastructure.Badges;
using TechnifyERP.Application.Certificates;
using TechnifyERP.Infrastructure.Certificates;
using TechnifyERP.Application.Profiles;
using TechnifyERP.Infrastructure.Profiles;
using TechnifyERP.Application.Enrollments;
using TechnifyERP.Infrastructure.Enrollments;
using TechnifyERP.Application.Notes;
using TechnifyERP.Infrastructure.Notes;
using TechnifyERP.Application.Settings;
using TechnifyERP.Infrastructure.Settings;
using TechnifyERP.Application.StudentDashboard;
using TechnifyERP.Infrastructure.StudentDashboard;
using TechnifyERP.Application.CourseChanges;
using TechnifyERP.Infrastructure.CourseChanges;
using TechnifyERP.Application.Website;
using TechnifyERP.Infrastructure.Website;

namespace TechnifyERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<AccountService>();
        services.AddScoped<IRegistrationService>(provider => provider.GetRequiredService<AccountService>());
        services.AddScoped<IAccountApprovalService>(provider => provider.GetRequiredService<AccountService>());
        services.AddScoped<IAdminDirectoryService, AdminDirectoryService>();
        services.AddScoped<IFacultyCourseService, FacultyCourseService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IFeeService, FeeService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<INoticeService, NoticeService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IChallengeService, ChallengeService>();
        services.AddScoped<IDoubtService, DoubtService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IBadgeService, BadgeService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IAcademySettingsService, AcademySettingsService>();
        services.AddScoped<IStudentDashboardService, StudentDashboardService>();
        services.AddScoped<ICourseChangeService, CourseChangeService>();
        services.AddScoped<IHomepageService, HomepageService>();

        return services;
    }
}
