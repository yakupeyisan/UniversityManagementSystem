namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// İhale Oluşturma Input DTO
/// </summary>
public class CreateTenderDto
{
    public Guid PurchaseRequestId { get; set; }
    public string Description { get; set; } = null!;
    public DateTime DeadlineDate { get; set; }
    public decimal EstimatedBudget { get; set; }
    public List<Guid>? InvitedSupplierIds { get; set; }
}