using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;


public class CreatePayrollCommand : IRequest<Result<PayrollDto>>
{
    public Guid EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BaseSalary { get; set; }
    public int WorkingDays { get; set; }
    public int ActualWorkDays { get; set; }
    public decimal OvertimeHours { get; set; }
    public string PaymentMethod { get; set; } = "BankTransfer";
}