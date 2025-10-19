namespace UniversityMS.Application.Features.Staff.DTOs;

public class LeaveBalanceDto
{
    public int AnnualLeaveDays { get; set; }
    public int SickLeaveDays { get; set; }
    public int RemainingAnnualDays { get; set; }
    public int RemainingSickDays { get; set; }
}