using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

public class GetEmployeePayslipsQueryHandler : IRequestHandler<GetEmployeePayslipsQuery, Result<List<PayslipDto>>>
{
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IMapper _mapper;

    public GetEmployeePayslipsQueryHandler(
        IRepository<Payslip> payslipRepository,
        IMapper mapper)
    {
        _payslipRepository = payslipRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<PayslipDto>>> Handle(
        GetEmployeePayslipsQuery request,
        CancellationToken cancellationToken)
    {
        var payslips = await _payslipRepository.FindAsync(
            p => p.EmployeeId == request.EmployeeId &&
                 (!request.Year.HasValue || p.Year == request.Year) &&
                 (!request.Month.HasValue || p.Month == request.Month),
            cancellationToken);

        var dtos = _mapper.Map<List<PayslipDto>>(payslips);
        return Result<List<PayslipDto>>.Success(dtos);
    }
}