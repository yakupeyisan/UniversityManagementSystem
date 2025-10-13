using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Schedules.Commands;

public record PublishScheduleCommand(Guid ScheduleId) : IRequest<Result>;