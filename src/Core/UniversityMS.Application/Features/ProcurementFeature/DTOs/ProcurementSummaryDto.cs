namespace UniversityMS.Application.Features.ProcurementFeature.DTOs;

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