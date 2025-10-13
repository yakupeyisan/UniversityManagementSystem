using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Schedules.DTOs;

namespace UniversityMS.Application.Features.Schedules.Queries;

public record GetScheduleByIdQuery(Guid Id) : IRequest<Result<ScheduleDto>>;