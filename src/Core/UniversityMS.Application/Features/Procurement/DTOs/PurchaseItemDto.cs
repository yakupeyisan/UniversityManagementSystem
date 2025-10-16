namespace UniversityMS.Application.Features.Procurement.DTOs;

public record PurchaseItemDto(
    string ItemName,
    int Quantity,
    decimal UnitPrice,
    string Unit
);

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

/// <summary>
/// Satın Alma Talebi Kalemi DTO
/// </summary>
public class PurchaseRequestItemDto
{
    public Guid Id { get; set; }
    public string ItemDescription { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal EstimatedUnitPrice { get; set; }
    public decimal EstimatedTotalPrice { get; set; }
    public string Specification { get; set; } = null!;
    public int Priority { get; set; }
}

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

/// <summary>
/// Satın Alma Talebi Kalemi Oluşturma DTO
/// </summary>
public class CreatePurchaseRequestItemDto
{
    public string ItemDescription { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal EstimatedUnitPrice { get; set; }
    public string Specification { get; set; } = null!;
}

/// <summary>
/// Satın Alma Siparişi Output DTO
/// </summary>
public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = null!;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = null!;
    public Guid PurchaseRequestId { get; set; }
    public string PurchaseRequestNumber { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = null!;
    public string PaymentTerms { get; set; } = null!;
    public DateTime? ReceivedDate { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new();
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Satın Alma Siparişi Kalemi DTO
/// </summary>
public class PurchaseOrderItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string Status { get; set; } = null!;
}

/// <summary>
/// Satın Alma Siparişi Oluşturma Input DTO
/// </summary>
public class CreatePurchaseOrderDto
{
    public Guid PurchaseRequestId { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string PaymentTerms { get; set; } = null!;
    public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    public string? Notes { get; set; }
}

/// <summary>
/// Satın Alma Siparişi Kalemi Oluşturma DTO
/// </summary>
public class CreatePurchaseOrderItemDto
{
    public Guid PurchaseRequestItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Tedarikçi Output DTO
/// </summary>
public class SupplierDto
{
    public Guid Id { get; set; }
    public string SupplierName { get; set; } = null!;
    public string SupplierCode { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal Rating { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalPurchaseAmount { get; set; }
}

/// <summary>
/// Tedarikçi Oluşturma Input DTO
/// </summary>
public class CreateSupplierDto
{
    public string SupplierName { get; set; } = null!;
    public string SupplierCode { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;
}

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

/// <summary>
/// Satın Alma Özeti DTO
/// </summary>
public class ProcurementSummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }

    // Request İstatistikleri
    public int TotalRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int PendingRequests { get; set; }
    public decimal TotalRequestAmount { get; set; }

    // Order İstatistikleri
    public int TotalOrders { get; set; }
    public int ReceivedOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalOrderAmount { get; set; }

    // Tender İstatistikleri
    public int TotalTenders { get; set; }
    public int CompletedTenders { get; set; }
    public int OngoingTenders { get; set; }

    // Tedarikçi İstatistikleri
    public int ActiveSuppliers { get; set; }
    public decimal AverageSupplierRating { get; set; }

    // Mali Özet
    public decimal TotalSpent { get; set; }
    public decimal TotalBudgetAllocated { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal BudgetUtilizationRate { get; set; }

    public DateTime GeneratedDate { get; set; }
}