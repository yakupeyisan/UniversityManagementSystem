namespace UniversityMS.Application.Features.GradeFeature.DTOs;

public class GradeObjectionDto
{
    public Guid Id { get; set; }
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReviewNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}