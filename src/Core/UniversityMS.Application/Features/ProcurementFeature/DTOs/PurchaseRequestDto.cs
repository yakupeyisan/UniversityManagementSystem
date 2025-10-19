namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Satın Alma Talebi Output DTO
/// </summary>
public class PurchaseRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = null!;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public Guid RequestedById { get; set; }
    public string RequestedByName { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string? Description { get; set; }
    public List<PurchaseRequestItemDto> Items { get; set; } = new();
    public DateTime CreatedDate { get; set; }
}