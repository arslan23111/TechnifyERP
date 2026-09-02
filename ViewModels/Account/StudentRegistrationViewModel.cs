using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechnifyERP.ViewModels.Account;

public sealed class StudentRegistrationViewModel
{
    [Required, StringLength(150)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone, Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(20), Display(Name = "CNIC")]
    public string? Cnic { get; set; }

    [DataType(DataType.Date), Display(Name = "Date of Birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Range(1, int.MaxValue), Display(Name = "Course")]
    public int CourseId { get; set; }

    [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> Courses { get; set; } = [];
}
