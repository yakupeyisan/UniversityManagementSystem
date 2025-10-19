namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Oluşturma Input DTO
/// </summary>
public class CreatePayrollDto
{
    public Guid EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal? Bonus { get; set; }
    public List<PayrollDeductionInputDto>? Deductions { get; set; }
    public string? Notes { get; set; }
}
public class BatchPaymentResultDto
{
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public DateTime ProcessedDate { get; set; }
    public List<string> Errors { get; set; } = new();
}