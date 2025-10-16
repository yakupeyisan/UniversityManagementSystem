using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Ay için bordro özeti getir
/// </summary>
public class GetPayrollSummaryHandler : IRequestHandler<GetPayrollSummaryQuery, Result<PayrollSummaryDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;

    public GetPayrollSummaryHandler(IRepository<Payroll> payrollRepository)
    {
        _payrollRepository = payrollRepository;
    }

    public async Task<Result<PayrollSummaryDto>> Handle(GetPayrollSummaryQuery request, CancellationToken cancellationToken)
    {
        var payrolls = await _payrollRepository.GetAllAsync(cancellationToken);

        var monthPayrolls = payrolls
            .Where(p => p.Year == request.Year && p.Month == request.Month)
            .ToList();

        var summary = new PayrollSummaryDto
        {
            Year = request.Year,
            Month = request.Month,
            Period = $"{request.Month:D2}/{request.Year}",
            TotalEmployees = monthPayrolls.Count(),
            ProcessedPayrolls = monthPayrolls.Count(p => p.Status.ToString() == "Processed"),
            DraftPayrolls = monthPayrolls.Count(p => p.Status.ToString() == "Draft"),
            ApprovedPayrolls = monthPayrolls.Count(p => p.Status.ToString() == "Approved"),
            PaidPayrolls = monthPayrolls.Count(p => p.Status.ToString() == "Paid"),

            TotalBaseSalary = monthPayrolls.Sum(p => p.BaseSalary.Amount),
            TotalEarnings = monthPayrolls.Sum(p => p.TotalEarnings.Amount),
            TotalDeductions = monthPayrolls.Sum(p => p.TotalDeductions.Amount),
            TotalNetSalary = monthPayrolls.Sum(p => p.NetSalary.Amount),

            GeneratedDate = DateTime.UtcNow
        };

        return Result<PayrollSummaryDto>.Success(summary);
    }
}