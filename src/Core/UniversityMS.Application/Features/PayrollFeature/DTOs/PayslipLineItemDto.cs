namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Payslip Satırı DTO
/// </summary>
public class PayslipLineItemDto
{
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal Quantity { get; set; }
}