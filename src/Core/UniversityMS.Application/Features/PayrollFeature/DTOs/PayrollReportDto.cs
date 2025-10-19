namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

public class PayrollReportDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalEmployees { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetSalary { get; set; }
    public decimal AverageGrossSalary { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalSGKContribution { get; set; }
    public DateTime GeneratedDate { get; set; }
}