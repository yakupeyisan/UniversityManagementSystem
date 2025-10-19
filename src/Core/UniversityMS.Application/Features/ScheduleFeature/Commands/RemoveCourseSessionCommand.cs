using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public record RemoveCourseSessionCommand(
    Guid ScheduleId,
    Guid SessionId
) : IRequest<Result>;