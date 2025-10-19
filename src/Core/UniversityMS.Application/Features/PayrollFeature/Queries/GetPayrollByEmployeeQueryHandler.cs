using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

public class GetPayrollByEmployeeQueryHandler : IRequestHandler<GetPayrollByEmployeeQuery, Result<List<PayrollDto>>>
{
    private readonly IRepository<Payroll> _payrollRepository;

    public GetPayrollByEmployeeQueryHandler(IRepository<Payroll> payrollRepository)
    {
        _payrollRepository = payrollRepository;
    }

    public async Task<Result<List<PayrollDto>>> Handle(
        GetPayrollByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new PayrollByEmployeeSpecification(
            request.EmployeeId,
            request.Month,
            request.Year
        );

        var payrolls = await _payrollRepository.ListAsync(spec, cancellationToken);

        var dtos = payrolls.Select(p => new PayrollDto
        {
            Id = p.Id,
            PayrollNumber = p.PayrollNumber,
            Month = p.Month,
            Year = p.Year,
            Status = p.Status.ToString(),
            BaseSalary = p.BaseSalary.Amount,
            TotalEarnings = p.TotalEarnings.Amount,
            TotalDeductions = p.TotalDeductions.Amount,
            NetSalary = p.NetSalary.Amount
        }).ToList();

        return Result<List<PayrollDto>>.Success(dtos);
    }
}