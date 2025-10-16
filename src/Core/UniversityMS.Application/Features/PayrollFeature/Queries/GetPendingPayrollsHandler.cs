using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Ödenmemiş/İşlenmeyen bordroları getir
/// </summary>
public class GetPendingPayrollsHandler : IRequestHandler<GetPendingPayrollsQuery, Result<List<PayrollDto>>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IMapper _mapper;

    public GetPendingPayrollsHandler(IRepository<Payroll> payrollRepository, IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PayrollDto>>> Handle(GetPendingPayrollsQuery request, CancellationToken cancellationToken)
    {
        var allPayrolls = await _payrollRepository.GetAllAsync(cancellationToken);

        var pending = allPayrolls
            .Where(p => p.Status.ToString() == "Draft" || p.Status.ToString() == "Calculated")
            .AsEnumerable();

        if (request.Month.HasValue)
            pending = pending.Where(p => p.Month == request.Month);

        if (request.Year.HasValue)
            pending = pending.Where(p => p.Year == request.Year);

        var orderedPending = pending
            .OrderBy(p => p.Year)
            .ThenBy(p => p.Month)
            .ToList();

        var dtos = _mapper.Map<List<PayrollDto>>(orderedPending);
        return Result<List<PayrollDto>>.Success(dtos);
    }
}