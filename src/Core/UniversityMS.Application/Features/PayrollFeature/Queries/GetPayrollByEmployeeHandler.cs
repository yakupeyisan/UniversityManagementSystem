using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Çalışana ait tüm bordroları getir
/// </summary>
public class GetPayrollByEmployeeHandler : IRequestHandler<GetPayrollByEmployeeQuery, Result<List<PayrollDto>>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IMapper _mapper;

    public GetPayrollByEmployeeHandler(IRepository<Payroll> payrollRepository, IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PayrollDto>>> Handle(GetPayrollByEmployeeQuery request, CancellationToken cancellationToken)
    {
        var payrolls = await _payrollRepository.GetAllAsync(cancellationToken);

        var filtered = payrolls
            .Where(p => p.EmployeeId == request.EmployeeId)
            .AsEnumerable();

        if (request.Year.HasValue)
            filtered = filtered.Where(p => p.Year == request.Year);

        if (request.Month.HasValue)
            filtered = filtered.Where(p => p.Month == request.Month);

        if (!string.IsNullOrEmpty(request.Status))
            filtered = filtered.Where(p => p.Status.ToString() == request.Status);

        var orderedPayrolls = filtered
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToList();

        var dtos = _mapper.Map<List<PayrollDto>>(orderedPayrolls);
        return Result<List<PayrollDto>>.Success(dtos);
    }
}