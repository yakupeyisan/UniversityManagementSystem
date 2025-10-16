namespace UniversityMS.Application.Features.Procurement.DTOs;

/// <summary>
/// İhale Teklifi Oluşturma Input DTO
/// </summary>
public class CreateTenderBidDto
{
    public Guid TenderId { get; set; }
    public Guid SupplierId { get; set; }
    public decimal BidAmount { get; set; }
    public string? BidDocument { get; set; }
}