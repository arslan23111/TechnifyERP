using System.ComponentModel.DataAnnotations;

namespace TechnifyERP.ViewModels.Admin;

public sealed class CourseFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Course Code")]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [Display(Name = "Course Name")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 30)]
    [Display(Name = "Credit Hours")]
    public int CreditHours { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    [Display(Name = "Monthly Fee")]
    public decimal MonthlyFee { get; set; }
}
