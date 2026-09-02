using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechnifyERP.ViewModels.Faculty;

public sealed class AssignmentFormViewModel
{
    public int Id { get; set; }
    [Range(1, int.MaxValue)] public int CourseId { get; set; }
    [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
    [Required, StringLength(4000)] public string Description { get; set; } = string.Empty;
    [Required, DataType(DataType.DateTime)] public DateTime DueAt { get; set; } = DateTime.Now.AddDays(7);
    [Range(0.01, 100000)] public decimal TotalMarks { get; set; } = 100;
    public bool AllowResubmission { get; set; }
    public IFormFile? Attachment { get; set; }
    public string? ExistingAttachmentPath { get; set; }
    public IReadOnlyList<SelectListItem> Courses { get; set; } = [];
}
