namespace UniversityMS.Application.Features.Grades.DTOs;

public class GradeObjectionDto
{
    public Guid Id { get; set; }
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ObjectionDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewNotes { get; set; }
}