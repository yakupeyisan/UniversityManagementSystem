namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

/// <summary>
/// İhale (Tender) Output DTO
/// </summary>
public class TenderDto
{
    public Guid Id { get; set; }
    public string TenderNumber { get; set; } = null!;
    public Guid PurchaseRequestId { get; set; }
    public string Description { get; set; } = null!;
    public DateTime PublishedDate { get; set; }
    public DateTime DeadlineDate { get; set; }
    public decimal EstimatedBudget { get; set; }
    public string Status { get; set; } = null!;
    public int ParticipantCount { get; set; }
    public Guid? WinnerSupplierId { get; set; }
    public string? WinnerSupplierName { get; set; }
    public decimal? WinningBidAmount { get; set; }
    public List<TenderBidDto> Bids { get; set; } = new();
}