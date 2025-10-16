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