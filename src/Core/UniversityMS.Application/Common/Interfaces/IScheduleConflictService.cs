using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Common.Interfaces;

public interface IScheduleConflictService
{
    Task<List<ScheduleConflictDto>> CheckConflictsAsync(Guid scheduleId, CancellationToken cancellationToken);
    Task<bool> HasInstructorConflictAsync(Guid instructorId, DayOfWeek day, TimeSpan start, TimeSpan end, CancellationToken cancellationToken);
    Task<bool> HasClassroomConflictAsync(Guid classroomId, DayOfWeek day, TimeSpan start, TimeSpan end, CancellationToken cancellationToken);
}