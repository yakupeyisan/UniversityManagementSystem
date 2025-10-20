using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.HRFeature.Commands;

/// <summary>
/// Tamamlanan vardiyayı kayıt et Handler
/// </summary>
public class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result<ShiftDto>>
{
    private readonly IRepository<Shift> _shiftRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CompleteShiftCommandHandler> _logger;

    public CompleteShiftCommandHandler(
        IRepository<Shift> shiftRepository,
        IRepository<Employee> employeeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CompleteShiftCommandHandler> logger)
    {
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ShiftDto>> Handle(
        CompleteShiftCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Vardiya tamamlanıyor. ShiftId: {ShiftId}, ActualEndTime: {ActualEndTime}",
                request.ShiftId, request.ActualEndTime);

            // ========== 1. VARDIYA BULMA ==========
            var shift = await _shiftRepository.GetByIdAsync(request.ShiftId, cancellationToken);

            if (shift == null)
            {
                _logger.LogWarning("Vardiya bulunamadı. ShiftId: {ShiftId}", request.ShiftId);
                return Result<ShiftDto>.Failure("Vardiya bulunamadı.");
            }

            // ========== 2. DURUM KONTROLÜ ==========
            if (shift.Status != ShiftStatus.Scheduled)
            {
                _logger.LogWarning(
                    "Vardiya tamamlamaya uygun değil. ShiftId: {ShiftId}, Status: {Status}",
                    request.ShiftId, shift.Status);

                return Result<ShiftDto>.Failure(
                    $"Sadece Scheduled durumundaki vardiyalar tamamlanabilir. Mevcut durum: {shift.Status}");
            }

            // ========== 3. ZAMAN KONTROLÜ ==========
            if (request.ActualEndTime <= shift.StartTime)
            {
                _logger.LogWarning(
                    "Vardiya bitiş saati başlangıç saatinden önce. ShiftId: {ShiftId}",
                    request.ShiftId);

                return Result<ShiftDto>.Failure(
                    "Vardiya bitiş saati, başlangıç saatinden sonra olmalıdır.");
            }

            if (request.ActualEndTime > shift.EndTime.AddHours(2)) // 2 saat tolerans
            {
                _logger.LogWarning(
                    "Vardiya bitiş saati çok gecikmiş. ShiftId: {ShiftId}",
                    request.ShiftId);

                return Result<ShiftDto>.Failure(
                    "Vardiya bitiş saati, planlanan saati 2 saatten fazla aşamaz.");
            }

            // ========== 4. VARDIYA TAMAMLAMA ==========
            shift.Complete(request.ActualEndTime, request.Notes);

            _logger.LogInformation(
                "Vardiya tamamlandı. ShiftId: {ShiftId}, Duration: {Duration}",
                request.ShiftId,
                (request.ActualEndTime.ToTimeSpan() - shift.StartTime.ToTimeSpan()).TotalHours);

            // ========== 5. VERITABANINA KAYDETME ==========
            await _shiftRepository.UpdateAsync(shift, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Vardiya tamamlaması başarıyla kaydedildi. ShiftId: {ShiftId}", request.ShiftId);

            // ========== 6. DTO HAZIRLAMA VE DÖNÜŞ ==========
            var dto = _mapper.Map<ShiftDto>(shift);

            return Result<ShiftDto>.Success(
                dto,
                "Vardiya başarıyla tamamlandı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Vardiya tamamlama hatası. ShiftId: {ShiftId}", request.ShiftId);
            return Result<ShiftDto>.Failure($"Vardiya tamamlama hatası: {ex.Message}");
        }
    }
}

