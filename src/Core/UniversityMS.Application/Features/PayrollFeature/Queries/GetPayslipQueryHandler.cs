using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

public class GetPayslipQueryHandler : IRequestHandler<GetPayslipQuery, Result<PayslipDto>>
{
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IMapper _mapper;

    public GetPayslipQueryHandler(
        IRepository<Payslip> payslipRepository,
        IMapper mapper)
    {
        _payslipRepository = payslipRepository;
        _mapper = mapper;
    }

    public async Task<Result<PayslipDto>> Handle(
        GetPayslipQuery request,
        CancellationToken cancellationToken)
    {
        var payslip = await _payslipRepository.GetByIdAsync(request.PayslipId, cancellationToken);
        if (payslip == null)
            return Result<PayslipDto>.Failure("Payslip bulunamadı");

        var dto = _mapper.Map<PayslipDto>(payslip);
        return Result<PayslipDto>.Success(dto);
    }
}