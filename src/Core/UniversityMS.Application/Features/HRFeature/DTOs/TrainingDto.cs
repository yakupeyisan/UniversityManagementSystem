namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Eğitim programı DTO'su
/// </summary>
public class TrainingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string TrainingType { get; set; } = null!;
    public string Provider { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration { get; set; } // Saat cinsinden
    public string Location { get; set; } = null!;
    public decimal Cost { get; set; }
    public string Status { get; set; } = null!;
    public int ParticipantCount { get; set; }
    public int MaxParticipants { get; set; }
    public string? Instructor { get; set; }
    public string? CertificationNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}