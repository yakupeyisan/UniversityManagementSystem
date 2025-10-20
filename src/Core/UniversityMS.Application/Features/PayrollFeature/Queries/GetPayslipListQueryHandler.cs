using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;
using UniversityMS.Domain.Entities.PayrollAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Tüm Payslipleri Listele Handler
/// </summary>
public class GetPayslipListQueryHandler : IRequestHandler<GetPayslipListQuery, Result<PaginatedList<PayslipDto>>>
{
    private readonly IRepository<Payslip> _payslipRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPayslipListQueryHandler> _logger;

    public GetPayslipListQueryHandler(
        IRepository<Payslip> payslipRepository,
        IMapper mapper,
        ILogger<GetPayslipListQueryHandler> logger)
    {
        _payslipRepository = payslipRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<PayslipDto>>> Handle(
        GetPayslipListQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Payslip listesi getiriliyor. Sayfa: {Page}, Boyut: {Size}",
                request.PageNumber, request.PageSize);

            // ========== SPECIFICATION OLUŞTURMA ==========
            var spec = new PayslipFilteredSpecification(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                employeeId: request.EmployeeId,
                year: request.Year,
                month: request.Month,
                status: request.Status);

            // ========== PAYSLIPLERI GETIRME ==========
            var payslips = await _payslipRepository.ListAsync(spec, cancellationToken);
            var totalCount = await _payslipRepository.CountAsync(spec, cancellationToken);

            // ========== DTO MAPPING ==========
            var dtos = _mapper.Map<List<PayslipDto>>(payslips);

            // ========== PAGINATED RESULT OLUŞTURMA ==========
            var paginatedResult = PaginatedList<PayslipDto>.Create(
                dtos,
                request.PageNumber,
                request.PageSize
            );

            _logger.LogInformation(
                "Payslip listesi başarıyla getirилді. Toplam: {Total}, Gösterilen: {Shown}",
                totalCount, dtos.Count);

            return Result<PaginatedList<PayslipDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payslip listesi getirme hatası");
            return Result<PaginatedList<PayslipDto>>.Failure($"Hata: {ex.Message}");
        }
    }
}