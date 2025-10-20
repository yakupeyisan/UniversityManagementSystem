using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Bordroyu ID'ye göre getir
/// </summary>
public class GetPayrollByIdQueryHandler : IRequestHandler<GetPayrollByIdQuery, Result<PayrollDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IMapper _mapper;

    public GetPayrollByIdQueryHandler(IRepository<Payroll> payrollRepository, IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _mapper = mapper;
    }

    public async Task<Result<PayrollDto>> Handle(GetPayrollByIdQuery request, CancellationToken cancellationToken)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);

        if (payroll is null)
            return Result<PayrollDto>.Failure("Bordro bulunamadı");

        var dto = _mapper.Map<PayrollDto>(payroll);
        return Result<PayrollDto>.Success(dto);
    }
}