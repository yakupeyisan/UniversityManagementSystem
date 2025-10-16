using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Tüm bordroları sayfalama ile getir
/// </summary>
public class GetAllPayrollsHandler : IRequestHandler<GetAllPayrollsQuery, Result<PaginatedList<PayrollDto>>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IMapper _mapper;

    public GetAllPayrollsHandler(IRepository<Payroll> payrollRepository, IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<PayrollDto>>> Handle(GetAllPayrollsQuery request, CancellationToken cancellationToken)
    {
        var allPayrolls = await _payrollRepository.GetAllAsync(cancellationToken);

        var filtered = allPayrolls.AsEnumerable();

        if (!string.IsNullOrEmpty(request.Status))
            filtered = filtered.Where(p => p.Status.ToString() == request.Status);

        if (request.Year.HasValue)
            filtered = filtered.Where(p => p.Year == request.Year);

        if (request.Month.HasValue)
            filtered = filtered.Where(p => p.Month == request.Month);

        var orderedPayrolls = filtered
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month);

        var totalCount = orderedPayrolls.Count();
        var pagedPayrolls = orderedPayrolls
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = _mapper.Map<List<PayrollDto>>(pagedPayrolls);

        var result = PaginatedList<PayrollDto>.Create(
            dtos,
            request.PageNumber,
            request.PageSize
        );

        return Result<PaginatedList<PayrollDto>>.Success(result);
    }
}