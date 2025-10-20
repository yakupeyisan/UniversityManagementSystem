using UniversityMS.Application.Features.StaffFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Çalışan liste DTO'su
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = null!;
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime HireDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BaseSalary { get; set; }
    public DateTime CreatedAt { get; set; }
}
/// <summary>
/// Çalışan detay DTO'su
/// </summary>
public class EmployeeDetailDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = null!;
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BaseSalary { get; set; }
    public int AnnualLeaveBalance { get; set; }
    public int TenureYears { get; set; }
    public List<ContractDto> Contracts { get; set; } = new();
    public List<LeaveRequestDto> Leaves { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
/// <summary>
/// İzin talep DTO'su
/// </summary>
public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string LeaveType { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public string Status { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
/// <summary>
/// İzin detay DTO'su
/// </summary>
public class LeaveDetailDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string LeaveType { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public string Status { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string? RejectionReason { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? DocumentPath { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Sözleşme DTO'su
/// </summary>
public class ContractDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string ContractNumber { get; set; } = null!;
    public string ContractType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Vardiya DTO'su
/// </summary>
public class ShiftDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string ShiftPattern { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal? OvertimeHours { get; set; }
    public DateTime CreatedAt { get; set; }
}