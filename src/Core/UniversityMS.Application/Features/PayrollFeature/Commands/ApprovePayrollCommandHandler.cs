using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

public class ApprovePayrollCommandHandler : IRequestHandler<ApprovePayrollCommand, Result<PayrollDto>>
{
    private readonly IRepository<Payroll> _payrollRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public ApprovePayrollCommandHandler(
        IRepository<Payroll> payrollRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper, 
        ICurrentUserService currentUserService)
    {
        _payrollRepository = payrollRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PayrollDto>> Handle(
        ApprovePayrollCommand request,
        CancellationToken cancellationToken)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId, cancellationToken);
        if (payroll is null)
            return Result<PayrollDto>.Failure("Bordro bulunamadı");
        var approverId = _currentUserService.UserId;

        if (approverId == Guid.Empty)
            return Result<PayrollDto>.Failure("Kullanıcı bilgisi alınamadı");

        // ✅ Validasyon: Bordro taslak mı?
        if (payroll.Status != PayrollStatus.Draft)
            return Result<PayrollDto>.Failure("Sadece taslak bordrolar onaylanabilir");

        payroll.Approve(approverId);

        await _payrollRepository.UpdateAsync(payroll, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PayrollDto>(payroll);
        return Result<PayrollDto>.Success(dto);
    }
}