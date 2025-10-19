namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Satın Alma Talebi Oluşturma Input DTO
/// </summary>
public class CreatePurchaseRequestDto
{
    public Guid DepartmentId { get; set; }
    public DateTime? RequiredDate { get; set; }
    public string Priority { get; set; } = null!;
    public string? Description { get; set; }
    public List<CreatePurchaseRequestItemDto> Items { get; set; } = new();
}