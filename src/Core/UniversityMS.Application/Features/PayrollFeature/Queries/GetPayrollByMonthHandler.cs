using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Belirli ay için tüm bordroları getir
/// </summary>
public class GetPayrollByMonthHandler : IRequestHandler<GetPayrollByMonthQuery, Result<List<PayrollDto>>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IMapper _mapper;

    public GetPayrollByMonthHandler(IRepository<Payroll> payrollRepository, IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PayrollDto>>> Handle(GetPayrollByMonthQuery request, CancellationToken cancellationToken)
    {
        var payrolls = await _payrollRepository.GetAllAsync(cancellationToken);

        var filtered = payrolls
            .Where(p => p.Year == request.Year && p.Month == request.Month)
            .AsEnumerable();

        if (!string.IsNullOrEmpty(request.Status))
            filtered = filtered.Where(p => p.Status.ToString() == request.Status);

        var dtos = _mapper.Map<List<PayrollDto>>(filtered.ToList());
        return Result<List<PayrollDto>>.Success(dtos);
    }
}