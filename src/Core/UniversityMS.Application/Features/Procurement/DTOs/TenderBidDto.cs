namespace UniversityMS.Application.Features.Procurement.DTOs;

/// <summary>
/// İhale Teklifi DTO
/// </summary>
public class TenderBidDto
{
    public Guid Id { get; set; }
    public Guid TenderId { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = null!;
    public DateTime BidDate { get; set; }
    public decimal BidAmount { get; set; }
    public string Status { get; set; } = null!; // Submitted, Approved, Rejected, Winner
    public string? BidDocument { get; set; }
    public int Rank { get; set; }
    public bool IsWinner { get; set; }
}