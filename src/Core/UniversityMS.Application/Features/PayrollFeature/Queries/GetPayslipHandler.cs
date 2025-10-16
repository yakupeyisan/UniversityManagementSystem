using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Bordro raporunu (Payslip) getir
/// </summary>
public class GetPayslipHandler : IRequestHandler<GetPayslipQuery, Result<PayslipDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IMapper _mapper;

    public GetPayslipHandler(IRepository<Payroll> payrollRepository, IMapper mapper)
    {
        _payrollRepository = payrollRepository;
        _mapper = mapper;
    }

    public async Task<Result<PayslipDto>> Handle(GetPayslipQuery request, CancellationToken cancellationToken)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);

        if (payroll is null)
            return Result<PayslipDto>.Failure("Bordro bulunamadı");

        var dto = _mapper.Map<PayslipDto>(payroll);
        return Result<PayslipDto>.Success(dto);
    }
}