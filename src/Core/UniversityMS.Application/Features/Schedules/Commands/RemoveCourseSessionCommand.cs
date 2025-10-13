using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Schedules.Commands;

public record RemoveCourseSessionCommand(
    Guid ScheduleId,
    Guid SessionId
) : IRequest<Result>;