using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Schedules.Commands;

public record ActivateScheduleCommand(Guid ScheduleId) : IRequest<Result>;