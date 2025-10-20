using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public record DeleteScheduleCommand(Guid Id) : IRequest<Result<Guid>>;