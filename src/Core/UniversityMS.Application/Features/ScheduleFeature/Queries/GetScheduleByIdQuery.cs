using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ScheduleFeature.Queries;

public record GetScheduleByIdQuery(Guid Id) : IRequest<Result<ScheduleDto>>;