namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Güncelleme Input DTO
/// </summary>
public class UpdatePayrollDto
{
    public Guid PayrollId { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal? Bonus { get; set; }
    public int ActualWorkDays { get; set; }
    public int LeaveDays { get; set; }
    public int AbsentDays { get; set; }
    public decimal OvertimeHours { get; set; }
}