using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public record ActivateScheduleCommand(Guid ScheduleId) : IRequest<Result>;