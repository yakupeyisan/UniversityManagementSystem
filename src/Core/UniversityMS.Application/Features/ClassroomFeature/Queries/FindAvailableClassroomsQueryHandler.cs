using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
namespace UniversityMS.Application.Features.ClassroomFeature.Queries;

public class FindAvailableClassroomsQueryHandler : IRequestHandler<FindAvailableClassroomsQuery, Result<List<ClassroomDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<FindAvailableClassroomsQueryHandler> _logger;

    public FindAvailableClassroomsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILogger<FindAvailableClassroomsQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<ClassroomDto>>> Handle(FindAvailableClassroomsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Parse times
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
                return Result<List<ClassroomDto>>.Failure("Geçersiz başlangıç saati.");

            if (!TimeSpan.TryParse(request.EndTime, out var endTime))
                return Result<List<ClassroomDto>>.Failure("Geçersiz bitiş saati.");

            var timeSlot = Domain.ValueObjects.TimeSlot.Create(startTime, endTime);

            // Get active schedule
            var schedule = await _context.Schedules
                .Include(s => s.CourseSessions)
                .FirstOrDefaultAsync(s =>
                        s.AcademicYear == request.AcademicYear &&
                        s.Semester == request.Semester &&
                        !s.IsDeleted,
                    cancellationToken);

            if (schedule == null)
                return Result<List<ClassroomDto>>.Failure("Program bulunamadı.");

            // Find occupied classroom IDs at this time
            var occupiedClassroomIds = schedule.CourseSessions
                .Where(cs =>
                    cs.DayOfWeek == request.DayOfWeek &&
                    cs.TimeSlot.ConflictsWith(timeSlot) &&
                    !cs.IsDeleted)
                .Select(cs => cs.ClassroomId)
                .ToList();

            // Find available classrooms
            var query = _context.Classrooms
                .Where(c =>
                    c.IsActive &&
                    !c.IsDeleted &&
                    c.Capacity >= request.RequiredCapacity &&
                    !occupiedClassroomIds.Contains(c.Id));

            // Apply feature filters
            if (request.NeedsProjector)
                query = query.Where(c => c.HasProjector);

            if (request.NeedsComputers)
                query = query.Where(c => c.HasComputers);

            if (request.PreferredType.HasValue)
                query = query.Where(c => c.Type == request.PreferredType.Value);

            var availableClassrooms = await query
                .OrderBy(c => c.Capacity)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<ClassroomDto>>(availableClassrooms);

            return Result<List<ClassroomDto>>.Success(dtos, $"{dtos.Count} uygun derslik bulundu.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding available classrooms");
            return Result<List<ClassroomDto>>.Failure("Uygun derslikler bulunurken hata oluştu.");
        }
    }
}