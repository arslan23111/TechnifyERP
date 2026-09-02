using TechnifyERP.Application.Attendance;
using TechnifyERP.Application.StudentDashboard;

namespace TechnifyERP.ViewModels.Student;

public sealed record StudentDashboardPageViewModel(
    StudentDashboardDto Dashboard,
    StudentAttendanceReportDto Attendance);
