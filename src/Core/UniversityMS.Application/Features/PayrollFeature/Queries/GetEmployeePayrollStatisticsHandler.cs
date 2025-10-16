using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Çalışan bordro istatistikleri
/// </summary>
public class GetEmployeePayrollStatisticsHandler :
    IRequestHandler<GetEmployeePayrollStatisticsQuery, Result<EmployeePayrollStatisticsDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;

    public GetEmployeePayrollStatisticsHandler(IRepository<Payroll> payrollRepository)
    {
        _payrollRepository = payrollRepository;
    }

    public async Task<Result<EmployeePayrollStatisticsDto>> Handle(
        GetEmployeePayrollStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var payrolls = await _payrollRepository.GetAllAsync(cancellationToken);

        var employeePayrolls = payrolls
            .Where(p => p.EmployeeId == request.EmployeeId && p.Year == request.Year)
            .ToList();

        if (!employeePayrolls.Any())
            return Result<EmployeePayrollStatisticsDto>.Failure("Bordro kaydı bulunamadı");

        var stats = new EmployeePayrollStatisticsDto
        {
            EmployeeId = request.EmployeeId,
            EmployeeFullName = $"{employeePayrolls.First().Employee.Person.FirstName} {employeePayrolls.First().Employee.Person.LastName}",
            AverageSalary = employeePayrolls.Average(p => p.NetSalary.Amount),
            TotalSalaryYTD = employeePayrolls.Sum(p => p.TotalEarnings.Amount),
            TotalDeductionsYTD = employeePayrolls.Sum(p => p.TotalDeductions.Amount),
            TotalNetYTD = employeePayrolls.Sum(p => p.NetSalary.Amount),
            MonthsProcessed = employeePayrolls.Count(p => p.Status.ToString() != "Draft"),
            MonthsPending = employeePayrolls.Count(p => p.Status.ToString() == "Draft"),
            LastPaymentDate = employeePayrolls.Where(p => p.PaidDate.HasValue)
                .OrderByDescending(p => p.PaidDate)
                .FirstOrDefault()?.PaidDate ?? DateTime.MinValue
        };

        return Result<EmployeePayrollStatisticsDto>.Success(stats);
    }
}