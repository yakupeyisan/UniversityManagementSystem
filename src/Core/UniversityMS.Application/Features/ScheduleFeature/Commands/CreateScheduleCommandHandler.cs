using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Result<Guid>>
{
    private readonly IRepository<Schedule> _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateScheduleCommandHandler> _logger;

    public CreateScheduleCommandHandler(
        IRepository<Schedule> scheduleRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateScheduleCommandHandler> _logger)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        this._logger = _logger;
    }

    public async Task<Result<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if schedule already exists for this academic year and semester
            var existingSchedule = await _scheduleRepository.FirstOrDefaultAsync(
                s => s.AcademicYear == request.AcademicYear &&
                     s.Semester == request.Semester &&
                     s.DepartmentId == request.DepartmentId &&
                     !s.IsDeleted,
                cancellationToken
            );

            if (existingSchedule != null)
                return Result<Guid>.Failure("Bu dönem için program zaten mevcut.");

            var schedule = Schedule.Create(
                request.AcademicYear,
                request.Semester,
                request.Name,
                request.StartDate,
                request.EndDate,
                request.DepartmentId
            );

            await _scheduleRepository.AddAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Schedule created: {ScheduleId} for {AcademicYear} Semester {Semester}",
                schedule.Id, request.AcademicYear, request.Semester);

            return Result<Guid>.Success(schedule.Id, "Program başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating schedule");
            return Result<Guid>.Failure("Program oluşturulurken hata oluştu. Hata:"+ex.Message);
        }
    }
}