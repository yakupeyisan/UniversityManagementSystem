using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

public class GetMonthlyPayrollReportQueryHandler : IRequestHandler<GetMonthlyPayrollReportQuery, Result<PayrollReportDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;

    public GetMonthlyPayrollReportQueryHandler(IRepository<Payroll> payrollRepository)
    {
        _payrollRepository = payrollRepository;
    }

    public async Task<Result<PayrollReportDto>> Handle(
        GetMonthlyPayrollReportQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new PayrollByPeriodSpecification(request.Month, request.Year);
        var payrolls = await _payrollRepository.ListAsync(spec, cancellationToken);

        var report = new PayrollReportDto
        {
            Month = request.Month,
            Year = request.Year,
            TotalEmployees = payrolls.Count,
            TotalGrossSalary = payrolls.Sum(p => p.TotalEarnings.Amount),
            TotalDeductions = payrolls.Sum(p => p.TotalDeductions.Amount),
            TotalNetSalary = payrolls.Sum(p => p.NetSalary.Amount),
            AverageGrossSalary = payrolls.Any() ? payrolls.Average(p => p.TotalEarnings.Amount) : 0,
            GeneratedDate = DateTime.UtcNow
        };

        return Result<PayrollReportDto>.Success(report);
    }
}