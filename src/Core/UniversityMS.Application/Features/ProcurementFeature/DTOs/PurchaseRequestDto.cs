using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// Satın Alma Talebi Output DTO
/// </summary>
public class PurchaseRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProcurementStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime RequiredDate { get; set; }
    public string Priority { get; set; } = string.Empty;
    public Guid RequestedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}