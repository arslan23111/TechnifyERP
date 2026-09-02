using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechnifyERP.Domain.Enums;

namespace TechnifyERP.ViewModels.Faculty;

public sealed class AttendancePageViewModel
{
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [DataType(DataType.Date)]
    public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public string? CourseCode { get; set; }
    public string? CourseName { get; set; }
    public IReadOnlyList<SelectListItem> Courses { get; set; } = [];
    public List<AttendanceItemViewModel> Items { get; set; } = [];
    public bool SheetLoaded { get; set; }
}

public sealed class AttendanceItemViewModel
{
    public string StudentUserId { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public AttendanceStatus? Status { get; set; }
}
